namespace MyMandiSystem.Printing;

/// <summary>
/// Company information shown on print headers.
/// Populated once at app start / login from SystemConfig or Company entity.
/// </summary>
public sealed class CompanyInfo
{
    public string Name { get; set; } = "City Computers Marot";
    public string NameUrdu { get; set; } = "سٹی کمپیوٹرز مروٹ";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? LogoPath { get; set; }
    public string? NTN { get; set; }
    public string? RegistrationNo { get; set; }
}

/// <summary>
/// Static context available to all print templates.
/// Set once at app start / login. Templates read company info from here
/// so no model needs to carry company data.
/// </summary>
public static class PrintContext
{
    public static CompanyInfo Company { get; set; } = new();
    public static string CurrentUserName { get; set; } = "";
    public static DateTime PrintedAt => DateTime.Now;
}
