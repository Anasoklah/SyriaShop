using Microsoft.EntityFrameworkCore.Storage;
using SyriaShop.Application;

namespace SyriaShop.Infrastructure;

internal sealed class UnitOfWorkTransaction(IDbContextTransaction transaction) : IUnitOfWorkTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken = default) =>
        transaction.CommitAsync(cancellationToken);

    public Task RollbackAsync(CancellationToken cancellationToken = default) =>
        transaction.RollbackAsync(cancellationToken);

    public async ValueTask DisposeAsync() => await transaction.DisposeAsync();
}
