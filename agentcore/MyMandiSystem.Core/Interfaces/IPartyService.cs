using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IPartyService
{
    Task<Party?> GetPartyByNoAsync(string partyNo);
    Task<IEnumerable<Party>> GetAllPartiesAsync();
    Task<IEnumerable<PartyGroup>> GetPartyGroupsAsync();
    Task AddPartyAsync(Party party);
    Task UpdatePartyAsync(Party party);
    Task DeletePartyAsync(int partyId);

    // Party Groups
    Task AddPartyGroupAsync(PartyGroup group);
    Task UpdatePartyGroupAsync(PartyGroup group);
    Task DeletePartyGroupAsync(int groupId);

    // Salesmen
    Task<IEnumerable<Salesman>> GetAllSalesmenAsync();
    Task AddSalesmanAsync(Salesman salesman);
    Task UpdateSalesmanAsync(Salesman salesman);
    Task DeleteSalesmanAsync(int salesmanId);
}
