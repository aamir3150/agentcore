using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface ICatalogService
{
    // Companies
    Task<IEnumerable<Company>> GetCompaniesAsync();
    Task AddCompanyAsync(Company company);
    Task UpdateCompanyAsync(Company company);
    Task DeleteCompanyAsync(int companyId);

    // Units
    Task<IEnumerable<Unit>> GetUnitsAsync();
    Task AddUnitAsync(Unit unit);
    Task UpdateUnitAsync(Unit unit);
    Task DeleteUnitAsync(int unitId);

    // Towns
    Task<IEnumerable<Town>> GetTownsAsync();
    Task AddTownAsync(Town town);
    Task UpdateTownAsync(Town town);
    Task DeleteTownAsync(int townId);

    // Sectors
    Task<IEnumerable<Sector>> GetSectorsAsync();
    Task AddSectorAsync(Sector sector);
    Task UpdateSectorAsync(Sector sector);
    Task DeleteSectorAsync(int sectorId);
}
