using Microsoft.EntityFrameworkCore;
using SyriaShop.Application;

namespace SyriaShop.Infrastructure;

public sealed class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
