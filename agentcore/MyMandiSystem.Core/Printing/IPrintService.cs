namespace MyMandiSystem.Core.Printing;

/// <summary>
/// Supported paper sizes for the Print Engine.
/// WPF units = 1/96 inch.
/// </summary>
public enum PaperKind
{
    /// <summary>A4 (794 × 1123 WPF units, ~210 × 297 mm)</summary>
    A4,
    /// <summary>A5 (559 × 794 WPF units, ~148 × 210 mm)</summary>
    A5,
    /// <summary>Thermal 80 mm roll (302 × content height)</summary>
    Thermal80
}

/// <summary>
/// Options that control how a document prints.
/// Immutable record – callers create via `new PrintOptions(...)` or `with { ... }`.
/// </summary>
public sealed record PrintOptions(
    PaperKind Paper = PaperKind.A4,
    bool Landscape = false,
    int Copies = 1,
    string Title = "Print");

/// <summary>
/// Implement this on a print model to declare its default paper size, copies, etc.
/// If a model does NOT implement this, PrintService uses <see cref="PrintOptions"/> defaults.
/// </summary>
public interface IPrintable
{
    PrintOptions DefaultOptions { get; }
}

/// <summary>
/// Common Print Engine service contract.
/// ViewModel calls Preview or Print with any model; the engine auto-selects the template.
/// </summary>
public interface IPrintService
{
    /// <summary>Show a print preview window for the given model.</summary>
    void Preview(object model, PrintOptions? options = null);

    /// <summary>Send the model directly to the printer (shows PrintDialog first).</summary>
    void Print(object model, PrintOptions? options = null);
}
