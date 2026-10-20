using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Interceptors;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ISystemConfigService _configService;

    public AuditInterceptor(ICurrentUserService currentUserService, ISystemConfigService configService)
    {
        _currentUserService = currentUserService;
        _configService = configService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ValidateReadOnly(eventData.Context);
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ValidateReadOnly(eventData.Context);
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ValidateReadOnly(DbContext? context)
    {
        if (context == null) return;

        if (_configService.IsReadOnly)
        {
            var hasModifications = context.ChangeTracker.Entries()
                .Any(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

            if (hasModifications)
            {
                throw new InvalidOperationException("یہ مالی سال لاک (Read-Only) ہے۔ اس میں نیا اندراج یا ترمیم کرنا ممکن نہیں ہے۔\n(This financial year is locked/archived in Read-Only mode. Modifications are strictly restricted.)");
            }
        }
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context == null) return;

        var entries = context.ChangeTracker.Entries<AuditBase>();
        int userId = _currentUserService.GetCurrentUserId();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = userId;
                entry.Entity.CreatedAt = DateTime.Now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedBy = userId;
                entry.Entity.UpdatedAt = DateTime.Now;
            }
        }
    }
}
