using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class UserListItem : ObservableObject
{
    public short UserID { get; set; }
    public string UserName { get; set; } = "";
    public string FullName { get; set; } = "";
    public bool IsAdministrator { get; set; }
    public bool IsEnabled { get; set; }
    public string RoleBadge => IsAdministrator ? "Admin / ایڈمن" : "Standard / عام صارف";
    public string StatusBadge => IsEnabled ? "Active" : "Disabled";
}

public class TaskPermissionItem : ObservableObject
{
    public string TaskKey { get; set; } = "";
    public string TaskName { get; set; } = "";
    public string TaskGroup { get; set; } = "";
    
    private bool _isAssigned;
    public bool IsAssigned
    {
        get => _isAssigned;
        set => SetProperty(ref _isAssigned, value);
    }
}

public class TaskGroupCategory : ObservableObject
{
    public string GroupName { get; set; } = "";
    public ObservableCollection<TaskPermissionItem> Tasks { get; set; } = new();

    private bool _isAllSelected;
    public bool IsAllSelected
    {
        get => _isAllSelected;
        set
        {
            if (SetProperty(ref _isAllSelected, value))
            {
                foreach (var t in Tasks) t.IsAssigned = value;
            }
        }
    }
}

public partial class UsersManagementViewModel : ObservableObject
{
    private readonly ISystemConfigService _configService;

    [ObservableProperty] private ObservableCollection<UserListItem> _users = new();
    [ObservableProperty] private UserListItem? _selectedUser;
    [ObservableProperty] private ObservableCollection<TaskGroupCategory> _groupedTasks = new();

    // User Form Fields
    [ObservableProperty] private short _currentUserId;
    [ObservableProperty] private string _userName = "";
    [ObservableProperty] private string _fullName = "";
    [ObservableProperty] private string _password = "";
    [ObservableProperty] private string _confirmPassword = "";
    [ObservableProperty] private string _narration = "";
    [ObservableProperty] private bool _isAdministrator = true;
    [ObservableProperty] private bool _isEnabled = true;
    [ObservableProperty] private bool _isEditing;

    // Search filters
    [ObservableProperty] private string _userSearchText = "";
    [ObservableProperty] private string _taskSearchText = "";

    public UsersManagementViewModel(ISystemConfigService configService)
    {
        _configService = configService;
    }

    public async Task InitializeAsync()
    {
        await LoadUsersAsync();
        await LoadAllTasksAsync();
        if (Users.Any())
        {
            await SelectUserAsync(Users.First());
        }
        else
        {
            NewUser();
        }
    }

    [RelayCommand]
    public async Task LoadUsersAsync()
    {
        try
        {
            Users.Clear();
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = "SELECT UserID, UserName, ISNULL(FullName, UserName) as FullName, IsAdministrator, IsEnabled FROM Users ORDER BY UserID";
            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Users.Add(new UserListItem
                {
                    UserID = reader.GetInt16(reader.GetOrdinal("UserID")),
                    UserName = reader["UserName"]?.ToString() ?? "",
                    FullName = reader["FullName"]?.ToString() ?? "",
                    IsAdministrator = reader["IsAdministrator"] != DBNull.Value && Convert.ToBoolean(reader["IsAdministrator"]),
                    IsEnabled = reader["IsEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsEnabled"])
                });
            }
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Users");
        }
    }

    private async Task LoadAllTasksAsync()
    {
        try
        {
            GroupedTasks.Clear();
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = "SELECT TaskKey, TaskName, TaskGroup FROM Project_Tasks ORDER BY TaskGroup, TaskName";
            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            var dict = new Dictionary<string, TaskGroupCategory>();

            while (await reader.ReadAsync())
            {
                var group = reader["TaskGroup"]?.ToString() ?? "General";
                var key = reader["TaskKey"]?.ToString() ?? "";
                var name = reader["TaskName"]?.ToString() ?? key;

                if (!dict.TryGetValue(group, out var cat))
                {
                    cat = new TaskGroupCategory { GroupName = group };
                    dict[group] = cat;
                }

                cat.Tasks.Add(new TaskPermissionItem
                {
                    TaskKey = key,
                    TaskName = name,
                    TaskGroup = group,
                    IsAssigned = false
                });
            }

            foreach (var kvp in dict.OrderBy(k => k.Key))
            {
                GroupedTasks.Add(kvp.Value);
            }
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Project Tasks");
        }
    }

    [RelayCommand]
    public async Task SelectUserAsync(UserListItem? user)
    {
        if (user == null) return;
        SelectedUser = user;

        try
        {
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = "SELECT * FROM Users WHERE UserID = @UserID";
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@UserID", user.UserID);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    CurrentUserId = user.UserID;
                    UserName = reader["UserName"]?.ToString() ?? "";
                    FullName = reader["FullName"]?.ToString() ?? UserName;
                    Password = reader["Password"]?.ToString() ?? "";
                    ConfirmPassword = Password;
                    Narration = reader["Narration"]?.ToString() ?? "";
                    IsAdministrator = reader["IsAdministrator"] != DBNull.Value && Convert.ToBoolean(reader["IsAdministrator"]);
                    IsEnabled = reader["IsEnabled"] != DBNull.Value && Convert.ToBoolean(reader["IsEnabled"]);
                    IsEditing = true;
                }
            }

            // Load assigned tasks
            var assignedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var taskSql = "SELECT TaskKey FROM UsersTasks WHERE UserID = @UserID";
            await using (var tCmd = new SqlCommand(taskSql, conn))
            {
                tCmd.Parameters.AddWithValue("@UserID", user.UserID);
                await using var tReader = await tCmd.ExecuteReaderAsync();
                while (await tReader.ReadAsync())
                {
                    assignedKeys.Add(tReader["TaskKey"]?.ToString() ?? "");
                }
            }

            // Update tasks check state
            foreach (var group in GroupedTasks)
            {
                foreach (var t in group.Tasks)
                {
                    t.IsAssigned = IsAdministrator || assignedKeys.Contains(t.TaskKey);
                }
            }
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Select User");
        }
    }

    [RelayCommand]
    public void NewUser()
    {
        SelectedUser = null;
        CurrentUserId = 0;
        UserName = "";
        FullName = "";
        Password = "";
        ConfirmPassword = "";
        Narration = "";
        IsAdministrator = false;
        IsEnabled = true;
        IsEditing = false;

        foreach (var g in GroupedTasks)
        {
            foreach (var t in g.Tasks) t.IsAssigned = false;
        }
    }

    [RelayCommand]
    public void SelectAllTasks()
    {
        foreach (var g in GroupedTasks)
        {
            foreach (var t in g.Tasks) t.IsAssigned = true;
        }
    }

    [RelayCommand]
    public void DeselectAllTasks()
    {
        foreach (var g in GroupedTasks)
        {
            foreach (var t in g.Tasks) t.IsAssigned = false;
        }
    }

    [RelayCommand]
    public async Task SaveUserAsync()
    {
        if (string.IsNullOrWhiteSpace(UserName))
        {
            MessageBox.Show("Baraye meherbani Username darj karein.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Password != ConfirmPassword)
        {
            MessageBox.Show("Password aur Confirm Password aapas mein mutabiqat nahi rakhte!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            short targetUserId = CurrentUserId;

            if (targetUserId == 0)
            {
                // Find next available UserID
                var maxIdSql = "SELECT ISNULL(MAX(UserID), 0) + 1 FROM Users";
                await using var maxCmd = new SqlCommand(maxIdSql, conn);
                targetUserId = Convert.ToInt16(await maxCmd.ExecuteScalarAsync());

                var insertSql = @"
                    INSERT INTO Users (UserID, UserName, FullName, Password, Narration, IsAdministrator, IsEnabled)
                    VALUES (@UserID, @UserName, @FullName, @Password, @Narration, @IsAdministrator, @IsEnabled);";
                await using var iCmd = new SqlCommand(insertSql, conn);
                iCmd.Parameters.AddWithValue("@UserID", targetUserId);
                iCmd.Parameters.AddWithValue("@UserName", UserName.Trim());
                iCmd.Parameters.AddWithValue("@FullName", string.IsNullOrWhiteSpace(FullName) ? UserName.Trim() : FullName.Trim());
                iCmd.Parameters.AddWithValue("@Password", Password);
                iCmd.Parameters.AddWithValue("@Narration", Narration ?? "");
                iCmd.Parameters.AddWithValue("@IsAdministrator", IsAdministrator);
                iCmd.Parameters.AddWithValue("@IsEnabled", IsEnabled);
                await iCmd.ExecuteNonQueryAsync();
            }
            else
            {
                var updateSql = @"
                    UPDATE Users 
                    SET UserName = @UserName, FullName = @FullName, Password = @Password, 
                        Narration = @Narration, IsAdministrator = @IsAdministrator, IsEnabled = @IsEnabled
                    WHERE UserID = @UserID;";
                await using var uCmd = new SqlCommand(updateSql, conn);
                uCmd.Parameters.AddWithValue("@UserID", targetUserId);
                uCmd.Parameters.AddWithValue("@UserName", UserName.Trim());
                uCmd.Parameters.AddWithValue("@FullName", string.IsNullOrWhiteSpace(FullName) ? UserName.Trim() : FullName.Trim());
                uCmd.Parameters.AddWithValue("@Password", Password);
                uCmd.Parameters.AddWithValue("@Narration", Narration ?? "");
                uCmd.Parameters.AddWithValue("@IsAdministrator", IsAdministrator);
                uCmd.Parameters.AddWithValue("@IsEnabled", IsEnabled);
                await uCmd.ExecuteNonQueryAsync();
            }

            // Update UsersTasks
            var delTasksSql = "DELETE FROM UsersTasks WHERE UserID = @UserID";
            await using (var dCmd = new SqlCommand(delTasksSql, conn))
            {
                dCmd.Parameters.AddWithValue("@UserID", targetUserId);
                await dCmd.ExecuteNonQueryAsync();
            }

            if (!IsAdministrator)
            {
                // Bulk insert assigned tasks
                var assignedTasks = GroupedTasks.SelectMany(g => g.Tasks).Where(t => t.IsAssigned).ToList();
                foreach (var t in assignedTasks)
                {
                    var insTaskSql = "INSERT INTO UsersTasks (UserID, TaskKey) VALUES (@UserID, @TaskKey)";
                    await using var tCmd = new SqlCommand(insTaskSql, conn);
                    tCmd.Parameters.AddWithValue("@UserID", targetUserId);
                    tCmd.Parameters.AddWithValue("@TaskKey", t.TaskKey);
                    await tCmd.ExecuteNonQueryAsync();
                }
            }

            MessageBox.Show($"صارف '{UserName}' اور اختیارات کامیابی سے محفوظ ہو گئے!", 
                "کامیابی / Success", MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadUsersAsync();
            var reloaded = Users.FirstOrDefault(u => u.UserID == targetUserId);
            if (reloaded != null) await SelectUserAsync(reloaded);
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Save User");
        }
    }

    [RelayCommand]
    public async Task DeleteUserAsync()
    {
        if (SelectedUser == null) return;

        if (SelectedUser.UserID == 1)
        {
            MessageBox.Show("سسٹم کے بنیادی ایڈمنسٹریٹر (Admin) کو ڈیلیٹ کرنے کی اجازت نہیں ہے!", 
                "منع ہے / Action Prohibited", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"کیا آپ واقعی صارف '{SelectedUser.UserName}' کو حذف کرنا چاہتے ہیں؟", 
            "تصدیق / Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var delTasksSql = "DELETE FROM UsersTasks WHERE UserID = @UserID; DELETE FROM Users WHERE UserID = @UserID;";
            await using var cmd = new SqlCommand(delTasksSql, conn);
            cmd.Parameters.AddWithValue("@UserID", SelectedUser.UserID);
            await cmd.ExecuteNonQueryAsync();

            MessageBox.Show("صارف کامیابی سے حذف کر دیا گیا!", "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadUsersAsync();
            if (Users.Any()) await SelectUserAsync(Users.First());
            else NewUser();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Delete User");
        }
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
