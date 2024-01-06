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

    public DbSet<EfAuditEntry> AuditEntries { get; set; }
    public ClaimsPrincipal ClaimsPrincipal { get; protected set; }

    protected virtual IEnumerable<EfAuditEntry> OnBeforeSaveChanges(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        var entries = new List<EfAuditEntry>();
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged ||
                entry.Entity is not IFullAuditable)
                continue;
            var auditEntry = new EfAuditEntry
            {
                ActionType = entry.State.ToString(),
                EntityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey()).CurrentValue?.ToString(),
                EntityName = entry.Metadata.ClrType.Name,
                TimeStamp = DateTime.UtcNow,
                NameIdentifier = _nameIdentifier,
                Changes = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue })
                    .ToDictionary(i => i.Name, i => i.CurrentValue),
                TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList()
            };
            entries.Add(auditEntry);
        }

        return entries;
    }

    protected virtual async Task OnAfterSaveChangesAsync(IEnumerable<EfAuditEntry> auditEntries,
        CancellationToken cancellationToken = default)
    {
        if (!(auditEntries?.Any() ?? false))
            await Task.CompletedTask;
        foreach (var entry in auditEntries)
        {
           
            foreach (var prop in entry.TempProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    entry.EntityId = prop.CurrentValue.ToString();
                    entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                }
            }
        }

        AuditEntries.AddRange(auditEntries);
        await base.SaveChangesAsync(cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var isManualTransaction = false;
        try
        {
            SetAuditFields();
            var auditEntries = OnBeforeSaveChanges(cancellationToken);
            if (auditEntries.Any() && Database is
                { AutoTransactionBehavior: AutoTransactionBehavior.Never, CurrentTransaction: null })
            {
                Database.AutoTransactionBehavior = AutoTransactionBehavior.WhenNeeded;

                isManualTransaction = true;
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChangesAsync(auditEntries, cancellationToken);
            return result;
        }
        finally
        {
            if (isManualTransaction)
                Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }
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