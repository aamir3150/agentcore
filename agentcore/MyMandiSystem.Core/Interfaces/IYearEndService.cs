using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IYearEndService
{
    Task<bool> StartNewYearAsync(string newYearCode);
    Task<IEnumerable<string>> GetAvailableYearsAsync();
    Task<string> GetDatabasePathForYearAsync(string year);
}
