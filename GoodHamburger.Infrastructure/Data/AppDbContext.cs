using GoodHamburger.Application.Common.Interfaces;
using GoodHamburger.Domain.Auth;
using GoodHamburger.Domain.Common;
using GoodHamburger.Domain.Pedidos;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IUnitOfWork, IDbContextAccessor
{
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    public IEnumerable<BaseEntity> GetTrackedEntities() =>
        ChangeTracker.Entries<BaseEntity>().Select(e => e.Entity);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
