using MyMandiSystem.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

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
        PersistYearToAppSettings(year);
    }

    private void PersistYearToAppSettings(string year)
    {
        try
        {
            var targets = new List<string>();
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var binConfig = Path.Combine(baseDir, "appsettings.json");
            targets.Add(binConfig);

            // Also target project directory if in debug development
            var projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
            var projConfig = Path.Combine(projectDir, "appsettings.json");
            if (File.Exists(projConfig))
            {
                targets.Add(projConfig);
            }

            foreach (var cfgPath in targets)
            {
                if (File.Exists(cfgPath))
                {
                    var jsonContent = File.ReadAllText(cfgPath);
                    var node = JsonNode.Parse(jsonContent);
                    if (node != null)
                    {
                        if (node["AppSettings"] is JsonObject appSettings)
                        {
                            appSettings["CurrentFinancialYear"] = year;
                        }
                        else
                        {
                            node["AppSettings"] = new JsonObject
                            {
                                ["CurrentFinancialYear"] = year
                            };
                        }

                        var options = new JsonSerializerOptions { WriteIndented = true };
                        File.WriteAllText(cfgPath, node.ToJsonString(options));
                    }
                }
            }
        }
        catch
        {
            // Suppress IO failure if appsettings is strictly read-only
        }
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
        
        if (!connTemplate.Contains("{0}"))
        {
            return connTemplate;
        }
        return string.Format(connTemplate, dbName);
    }

    public bool IsReadOnly { get; set; }
}
