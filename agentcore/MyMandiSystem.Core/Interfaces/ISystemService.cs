using MyMandiSystem.Core.Entities;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface ISystemService
{
    Task<FinancialYear?> GetCurrentYearAsync();
    Task<string> GetNextDocumentNoAsync(DocumentType type, int yearId);
    Task<string> GetConfigValueAsync(string key);
    Task SetConfigValueAsync(string key, string value);
    Task<System.Collections.Generic.List<CropSeason>> GetActiveSeasonsAsync();
}
