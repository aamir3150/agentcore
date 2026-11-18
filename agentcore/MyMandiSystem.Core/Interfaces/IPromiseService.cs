using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IPromiseService
{
    Task<IEnumerable<Promise>> GetAllPromisesAsync();
    Task<Promise?> GetPromiseByIdAsync(int id);
    Task AddPromiseAsync(Promise promise);
    Task UpdatePromiseAsync(Promise promise);
    Task DeletePromiseAsync(int id);
    Task ToggleFulfilledAsync(int id);
}
