using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class AuthService : IAuthService
{
    // Mock implementation for development
    public User? CurrentUser { get; private set; }

    public Task<User?> LoginAsync(string username, string password)
    {
        if (username == "admin" && password == "admin")
        {
            CurrentUser = new User 
            { 
                Id = 1, 
                Username = "admin", 
                FullName = "System Administrator", 
                Role = UserRole.Admin 
            };
            return Task.FromResult<User?>(CurrentUser);
        }
        return Task.FromResult<User?>(null);
    }

    public Task LogoutAsync()
    {
        CurrentUser = null;
        return Task.CompletedTask;
    }
}
