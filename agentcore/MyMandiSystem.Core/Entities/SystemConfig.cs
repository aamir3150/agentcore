namespace MyMandiSystem.Core.Entities;

public enum ConfigDataType
{
    String = 1,
    Int = 2,
    Bool = 3,
    Decimal = 4
}

public class SystemConfig : AuditBase
{
    public int Id { get; set; }
    public string ConfigKey { get; set; } = string.Empty;
    public string ConfigValue { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ConfigDataType DataType { get; set; }
}
