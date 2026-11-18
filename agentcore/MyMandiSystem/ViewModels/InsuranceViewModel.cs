using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class InsuranceViewModel : ObservableObject
{
    private readonly IInsuranceService _insuranceService;

    [ObservableProperty] private string _windowTitle = "Insurance Policies Management - انشورنس پالیسی مینیجمنٹ";
    [ObservableProperty] private ObservableCollection<InsurancePolicy> _allPolicies = new();
    [ObservableProperty] private ObservableCollection<InsurancePolicy> _filteredPolicies = new();

    [ObservableProperty] private InsurancePolicy? _selectedPolicy;
    [ObservableProperty] private InsurancePolicy _currentPolicy = new();
    [ObservableProperty] private bool _isFormVisible;

    // Filters
    [ObservableProperty] private bool _isFilterAll = true;
    [ObservableProperty] private bool _isFilterActive;
    [ObservableProperty] private bool _isFilterExpiringSoon;
    [ObservableProperty] private bool _isFilterExpired;
    [ObservableProperty] private string _searchText = string.Empty;

    public Action? RequestClose { get; set; }

    public InsuranceViewModel(IInsuranceService insuranceService)
    {
        _insuranceService = insuranceService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            var list = await _insuranceService.GetAllPoliciesAsync();
            AllPolicies = new ObservableCollection<InsurancePolicy>(list);
            ApplyFilter();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Insurance Policies");
        }
    }

    partial void OnIsFilterAllChanged(bool value) { if (value) ApplyFilter(); }
    partial void OnIsFilterActiveChanged(bool value) { if (value) ApplyFilter(); }
    partial void OnIsFilterExpiringSoonChanged(bool value) { if (value) ApplyFilter(); }
    partial void OnIsFilterExpiredChanged(bool value) { if (value) ApplyFilter(); }
    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var query = AllPolicies.AsEnumerable();

        if (IsFilterActive)
            query = query.Where(p => !p.IsExpired);
        else if (IsFilterExpiringSoon)
            query = query.Where(p => p.IsExpiringSoon);
        else if (IsFilterExpired)
            query = query.Where(p => p.IsExpired);

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.Trim().ToLower();
            query = query.Where(p =>
                (!string.IsNullOrEmpty(p.InsuranceCompanyName) && p.InsuranceCompanyName.ToLower().Contains(s)) ||
                (!string.IsNullOrEmpty(p.CoverNote) && p.CoverNote.ToLower().Contains(s)) ||
                (!string.IsNullOrEmpty(p.PolicyNo) && p.PolicyNo.ToLower().Contains(s)) ||
                (!string.IsNullOrEmpty(p.Category) && p.Category.ToLower().Contains(s))
            );
        }

        FilteredPolicies = new ObservableCollection<InsurancePolicy>(query);
    }

    [RelayCommand]
    private void AddPolicy()
    {
        CurrentPolicy = new InsurancePolicy
        {
            FromDate = DateTime.Today,
            ToDate = DateTime.Today.AddYears(1)
        };
        IsFormVisible = true;
    }

    [RelayCommand]
    private void EditPolicy(InsurancePolicy? policy)
    {
        if (policy == null) return;
        CurrentPolicy = new InsurancePolicy
        {
            SrNo = policy.SrNo,
            InsuranceCompanyName = policy.InsuranceCompanyName,
            CoverNote = policy.CoverNote,
            InsuredValue = policy.InsuredValue,
            NetPremium = policy.NetPremium,
            FromDate = policy.FromDate,
            ToDate = policy.ToDate,
            PolicyNo = policy.PolicyNo,
            Category = policy.Category
        };
        IsFormVisible = true;
    }

    [RelayCommand]
    private async Task SavePolicy()
    {
        if (string.IsNullOrWhiteSpace(CurrentPolicy.InsuranceCompanyName))
        {
            MessageBox.Show("Please enter Insurance Company Name. / کمپنی کا نام درج کریں", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(CurrentPolicy.CoverNote))
        {
            MessageBox.Show("Please enter Cover Note. / کور نوٹ درج کریں", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (CurrentPolicy.SrNo == 0)
            {
                await _insuranceService.AddPolicyAsync(CurrentPolicy);
            }
            else
            {
                await _insuranceService.UpdatePolicyAsync(CurrentPolicy);
            }

            IsFormVisible = false;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Save Insurance Policy");
        }
    }

    [RelayCommand]
    private void CancelPolicy() => IsFormVisible = false;

    [RelayCommand]
    private async Task DeletePolicy(InsurancePolicy? policy)
    {
        if (policy == null) return;
        if (MessageBox.Show($"Delete insurance policy '{policy.PolicyNo ?? policy.CoverNote}' for {policy.InsuranceCompanyName}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            try
            {
                await _insuranceService.DeletePolicyAsync(policy.SrNo);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                FriendlyErrorHandler.ShowError(ex, "Delete Insurance Policy");
            }
        }
    }

    [RelayCommand]
    private void Close() => RequestClose?.Invoke();
}
