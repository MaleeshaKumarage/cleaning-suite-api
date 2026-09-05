namespace CleaningSuite.Domain.Common;

/// <summary>Common fields on every Marten document.</summary>
public abstract class BaseDocument
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;

    /// <summary>Optimistic concurrency version, maintained by Marten.</summary>
    public int Version { get; set; }
}
