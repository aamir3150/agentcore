using MyMandiSystem.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace MyMandiSystem.Infrastructure.Services;

public class SystemConfigService : ISystemConfigService
{
    private readonly IConfiguration _configuration;
    private string _currentYear;

    public SystemConfigService(IConfiguration configuration)
    {
        _configuration = configuration;
        _currentYear = _configuration["AppSettings:CurrentFinancialYear"] ?? "2025-26";
    }

    public string GetCurrentFinancialYear() => _currentYear;

    public void SetCurrentFinancialYear(string year)
    {
        _currentYear = year;
        // In a real app, we would write back to appsettings.json or a custom config file
    }

    public string GetDataDirectory()
    {
        return _configuration["AppSettings:DataDirectory"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
    }

    public string GetDatabasePrefix() => _configuration["AppSettings:DatabasePrefix"] ?? "MyMandi";

    public string GetConnectionString(string dbName)
    {
        string connTemplate = _configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=.\\SQLEXPRESS;Database={0};Trusted_Connection=True;TrustServerCertificate=True;";
        return string.Format(connTemplate, dbName);
    }

    public bool IsReadOnly { get; set; }
}
