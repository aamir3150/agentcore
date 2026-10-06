using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface ISystemConfigService
{
    string GetCurrentFinancialYear();
    void SetCurrentFinancialYear(string year);
    string GetDataDirectory();
    string GetDatabasePrefix();
    string GetConnectionString(string dbName);
    bool IsReadOnly { get; set; }
}
