using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Tenon.Repository.EfCore;

public abstract class AuditDbContext : DbContext
{
    private readonly long _nameIdentifier;

    protected AuditDbContext(DbContextOptions options, ClaimsPrincipal claimsPrincipal)
        : base(options)
    {
        ClaimsPrincipal = claimsPrincipal;
        _nameIdentifier =
            long.TryParse(ClaimsPrincipal?.Claims?.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                out var nameIdentifier)
                ? nameIdentifier
                : -1;
    }


    protected AuditDbContext(DbContextOptions options) : this(options, null)
    {
    }

    public ClaimsPrincipal ClaimsPrincipal { get; protected set; }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected virtual void SetAuditFields()
    {
        var allBasicAuditEntities =
            ChangeTracker.Entries<EfBasicAuditEntity>().Where(x => x.State == EntityState.Added);
        foreach (var addedEntity in allBasicAuditEntities)
        {
            addedEntity.Entity.CreateTime = DateTime.UtcNow;
            addedEntity.Entity.CreateBy = _nameIdentifier;
        }

        var allModifiedEntities = ChangeTracker.Entries<EfBasicAuditEntity>()
            .Where(x => x.State is EntityState.Modified);
        foreach (var modifiedEntity in allModifiedEntities)
        {
            modifiedEntity.Entity.ModifyTime = DateTime.UtcNow;
            modifiedEntity.Entity.ModifyBy = _nameIdentifier;
        }
    }
}