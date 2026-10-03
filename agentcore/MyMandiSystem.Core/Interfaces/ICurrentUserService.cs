namespace MyMandiSystem.Core.Interfaces;

public interface ICurrentUserService
{
    int GetCurrentUserId();
    string GetCurrentUserName();
}
