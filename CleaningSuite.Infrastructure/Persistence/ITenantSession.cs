using CleaningSuite.Application.Tenants;
using Marten;

namespace CleaningSuite.Infrastructure.Persistence;

/// <summary>
/// Opens Marten sessions for the current tenant. The tenant id comes from
/// ITenantContext (set by API middleware from JWT realm or URL slug).
/// </summary>
public interface ITenantSession
{
    /// <summary>Session bound to the current request's tenant partition.</summary>
    IDocumentSession Session { get; }

    /// <summary>Read-only session bound to the current tenant partition.</summary>
    IQuerySession Query { get; }
}

public class TenantSessionFactory : ITenantSession
{
    private readonly IDocumentStore _store;
    private readonly ITenantContext _context;

    public TenantSessionFactory(IDocumentStore store, ITenantContext context)
    {
        _store = store;
        _context = context;
    }

    public IDocumentSession Session => _store.LightweightSession(_context.TenantId);
    public IQuerySession Query => _store.QuerySession(_context.TenantId);
}
