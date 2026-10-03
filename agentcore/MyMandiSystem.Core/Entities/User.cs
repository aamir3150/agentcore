using System;

namespace MyMandiSystem.Core.Entities;

public enum UserRole
{
    Admin = 1,
    Operator = 2,
    Viewer = 3
}

public class User : AuditBase
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
}
