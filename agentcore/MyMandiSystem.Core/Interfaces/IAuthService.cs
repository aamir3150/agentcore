using MyMandiSystem.Core.Entities;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IAuthService
{
    Task<User?> LoginAsync(string username, string password);
    Task LogoutAsync();
    User? CurrentUser { get; }
}
