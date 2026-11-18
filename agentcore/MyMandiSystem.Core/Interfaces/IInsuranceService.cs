using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IInsuranceService
{
    Task<IEnumerable<InsurancePolicy>> GetAllPoliciesAsync();
    Task<InsurancePolicy?> GetPolicyBySrNoAsync(int srNo);
    Task AddPolicyAsync(InsurancePolicy policy);
    Task UpdatePolicyAsync(InsurancePolicy policy);
    Task DeletePolicyAsync(int srNo);
}
