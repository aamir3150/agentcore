using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class PortSettingViewModel : ObservableObject
{
    private readonly ISystemConfigService _configService;

    [ObservableProperty] private ObservableCollection<string> _availablePorts = new();
    [ObservableProperty] private string _selectedPort = "COM1";
    [ObservableProperty] private ObservableCollection<string> _availableBaudRates = new() { "110", "300", "600", "1200", "2400", "4800", "9600", "14400", "19200", "28800", "38400", "56000", "115200" };
    [ObservableProperty] private string _selectedBaudRate = "9600";
    [ObservableProperty] private ObservableCollection<string> _availableDataBits = new() { "5", "6", "7", "8" };
    [ObservableProperty] private string _selectedDataBits = "8";
    [ObservableProperty] private ObservableCollection<string> _availableParity = new() { "None", "Even", "Odd", "Mark", "Space" };
    [ObservableProperty] private string _selectedParity = "None";
    [ObservableProperty] private ObservableCollection<string> _availableStopBits = new() { "1", "1.5", "2" };
    [ObservableProperty] private string _selectedStopBits = "1";

    [ObservableProperty] private ObservableCollection<string> _availableDirections = new() { "Left", "Right" };
    [ObservableProperty] private string _selectedDirection = "Left";
    [ObservableProperty] private int _dataLength = 6;
    [ObservableProperty] private int _dataOffset = 0;
    [ObservableProperty] private bool _isReversed;
    [ObservableProperty] private bool _forBales;
    [ObservableProperty] private bool _enableRts;
    [ObservableProperty] private bool _enableBarcodeWeighment;

    [ObservableProperty] private string _rawStreamData = "--- انتظار کنکشن / ڈیٹا موصول نہیں ہوا ---";
    [ObservableProperty] private string _liveWeightDisplay = "0";
    [ObservableProperty] private string _connectionStatus = "Disconnected / غیر منسلک";
    [ObservableProperty] private bool _isConnected;
    [ObservableProperty] private bool _isTesting;

    public PortSettingViewModel(ISystemConfigService configService)
    {
        _configService = configService;
        RefreshAvailablePorts();
        _ = LoadSettingsFromDatabaseAsync();
    }

    [RelayCommand]
    public void RefreshAvailablePorts()
    {
        AvailablePorts.Clear();
        List<string> ports;
        try
        {
            ports = System.IO.Ports.SerialPort.GetPortNames().OrderBy(p => p).ToList();
        }
        catch
        {
            ports = new List<string>();
        }

        if (ports.Count == 0)
        {
            ports = new List<string> { "COM1", "COM2", "COM3", "COM4", "COM5", "COM6" };
        }
        foreach (var p in ports)
        {
            AvailablePorts.Add(p);
        }
        if (!AvailablePorts.Contains(SelectedPort) && AvailablePorts.Any())
        {
            SelectedPort = AvailablePorts.First();
        }
    }

    public async Task LoadSettingsFromDatabaseAsync()
    {
        try
        {
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = "SELECT RegistryKey, RegistryValue FROM PortSettings";
            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var key = reader["RegistryKey"]?.ToString() ?? "";
                var val = reader["RegistryValue"]?.ToString() ?? "";

                switch (key.ToLower())
                {
                    case "portname":
                        if (val.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
                            SelectedPort = val;
                        else if (int.TryParse(val, out int portNum))
                            SelectedPort = $"COM{portNum}";
                        break;
                    case "portsettings":
                        // format: "9600,n,8,1"
                        var parts = val.Split(',');
                        if (parts.Length >= 4)
                        {
                            SelectedBaudRate = parts[0].Trim();
                            SelectedParity = parts[1].Trim().ToLower() switch
                            {
                                "n" => "None",
                                "e" => "Even",
                                "o" => "Odd",
                                "m" => "Mark",
                                "s" => "Space",
                                _ => "None"
                            };
                            SelectedDataBits = parts[2].Trim();
                            SelectedStopBits = parts[3].Trim();
                        }
                        break;
                    case "datadirection":
                        SelectedDirection = val.Equals("Right", StringComparison.OrdinalIgnoreCase) ? "Right" : "Left";
                        break;
                    case "datalength":
                        if (int.TryParse(val, out int len)) DataLength = len;
                        break;
                    case "dataoffset":
                        if (int.TryParse(val, out int off)) DataOffset = off;
                        break;
                    case "isreversed":
                        IsReversed = val == "1" || val.Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "forbales":
                        ForBales = val == "1" || val.Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;
                    case "enablerts":
                        EnableRts = val == "1" || val.Equals("true", StringComparison.OrdinalIgnoreCase);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"PortSettings Load error: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task SaveSettingsAsync(Window? window)
    {
        try
        {
            var year = _configService.GetCurrentFinancialYear();
            var connStr = _configService.GetConnectionString($"MyMandi_{year}");

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            // Extract numeric port if needed, or full COM name
            var portNumberStr = Regex.Match(SelectedPort, @"\d+").Value;
            if (string.IsNullOrEmpty(portNumberStr)) portNumberStr = "1";

            var parityChar = SelectedParity.ToLower() switch
            {
                "even" => "e",
                "odd" => "o",
                "mark" => "m",
                "space" => "s",
                _ => "n"
            };

            var settingsString = $"{SelectedBaudRate},{parityChar},{SelectedDataBits},{SelectedStopBits}";

            var saveSql = @"
MERGE INTO PortSettings AS target
USING (VALUES 
    ('PortName', @PortName, 'Serial Port Name'),
    ('PortSettings', @PortSettings, 'Baud,Parity,Data,Stop'),
    ('DataDirection', @DataDirection, 'Weight Data Direction'),
    ('DataLength', @DataLength, 'Data Length'),
    ('DataOffset', @DataOffset, 'Data Offset'),
    ('IsReversed', @IsReversed, 'Is Reversed Data'),
    ('ForBales', @ForBales, 'For Cotton Bales'),
    ('EnableRTS', @EnableRTS, 'RTS Enabled')
) AS source (RegistryKey, RegistryValue, Description)
ON target.RegistryKey = source.RegistryKey
WHEN MATCHED THEN 
    UPDATE SET target.RegistryValue = source.RegistryValue, target.Description = source.Description
WHEN NOT MATCHED THEN 
    INSERT (RegistryKey, RegistryValue, Description) VALUES (source.RegistryKey, source.RegistryValue, source.Description);";

            await using var cmd = new SqlCommand(saveSql, conn);
            cmd.Parameters.AddWithValue("@PortName", portNumberStr);
            cmd.Parameters.AddWithValue("@PortSettings", settingsString);
            cmd.Parameters.AddWithValue("@DataDirection", SelectedDirection);
            cmd.Parameters.AddWithValue("@DataLength", DataLength.ToString());
            cmd.Parameters.AddWithValue("@DataOffset", DataOffset.ToString());
            cmd.Parameters.AddWithValue("@IsReversed", IsReversed ? "1" : "0");
            cmd.Parameters.AddWithValue("@ForBales", ForBales ? "1" : "0");
            cmd.Parameters.AddWithValue("@EnableRTS", EnableRts ? "1" : "0");

            await cmd.ExecuteNonQueryAsync();

            MessageBox.Show("الیکٹرانک کانٹا (Weighbridge) کی پورٹ سیٹنگز کامیابی سے محفوظ ہو گئیں!",
                "کامیابی / Success", MessageBoxButton.OK, MessageBoxImage.Information);

            window?.Close();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Port Settings Save");
        }
    }

    [RelayCommand]
    public async Task TestPortAsync()
    {
        IsTesting = true;
        ConnectionStatus = $"پورٹ {SelectedPort} سے رابطہ ہو رہا ہے...";

        await Task.Delay(300);

        try
        {
            // Attempt to open actual serial port if present
            bool opened = false;
            string receivedRaw = "";

            try
            {
                using var sp = new System.IO.Ports.SerialPort(SelectedPort)
                {
                    BaudRate = int.TryParse(SelectedBaudRate, out int b) ? b : 9600,
                    DataBits = int.TryParse(SelectedDataBits, out int d) ? d : 8,
                    Parity = SelectedParity.Equals("Even", StringComparison.OrdinalIgnoreCase) ? System.IO.Ports.Parity.Even : System.IO.Ports.Parity.None,
                    StopBits = SelectedStopBits == "2" ? System.IO.Ports.StopBits.Two : System.IO.Ports.StopBits.One,
                    ReadTimeout = 1200,
                    RtsEnable = EnableRts
                };

                sp.Open();
                opened = sp.IsOpen;
                if (opened)
                {
                    // Read line or bytes
                    receivedRaw = sp.ReadLine();
                    sp.Close();
                }
            }
            catch
            {
                // Physical port not open or hardware scale not connected right now
                opened = false;
            }

            if (!opened || string.IsNullOrWhiteSpace(receivedRaw))
            {
                // Fallback simulation mode for testing when scale is off-line
                var rnd = new Random();
                int simKg = rnd.Next(15000, 48500);
                receivedRaw = $"=ST,GS,+{simKg:D6}kg\r\n";
                ConnectionStatus = $"Simulated Connection / ٹیسٹ رابطہ (ہارڈویئر منسلک نہیں)";
            }
            else
            {
                ConnectionStatus = $"Connected Successfully / رابطہ قائم ہو گیا ({SelectedPort})";
            }

            RawStreamData = receivedRaw.Trim();
            LiveWeightDisplay = ParseWeightFromRaw(receivedRaw);
            IsConnected = true;
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"نقص: {ex.Message}";
            RawStreamData = "ERROR";
            LiveWeightDisplay = "0";
            IsConnected = false;
        }
        finally
        {
            IsTesting = false;
        }
    }

    private string ParseWeightFromRaw(string raw)
    {
        try
        {
            // Match digits
            var digitsMatch = Regex.Match(raw, @"\d+");
            if (digitsMatch.Success)
            {
                var valStr = digitsMatch.Value;
                if (IsReversed)
                {
                    valStr = new string(valStr.Reverse().ToArray());
                }
                if (DataLength > 0 && valStr.Length > DataLength)
                {
                    valStr = SelectedDirection == "Left" 
                        ? valStr.Substring(0, DataLength) 
                        : valStr.Substring(valStr.Length - DataLength);
                }
                if (decimal.TryParse(valStr, out decimal parsed))
                {
                    return $"{parsed:N0} KG";
                }
            }
            return $"{raw.Trim()} KG";
        }
        catch
        {
            return "0 KG";
        }
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
