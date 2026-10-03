using MyMandiSystem.Core.Interfaces;

namespace MyMandiSystem.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    // For now, returning a mock user. 
    // In production, this would integrate with the Login/Auth session.
    public int GetCurrentUserId() => 1;
    public string GetCurrentUserName() => "Admin";
}
