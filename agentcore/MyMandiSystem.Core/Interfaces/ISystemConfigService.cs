using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface ISystemConfigService
{
    string GetCurrentFinancialYear();
    void SetCurrentFinancialYear(string year);
    string GetDataDirectory();
    string GetDatabasePrefix();
    bool IsReadOnly { get; set; }
}
