using GoodHamburger.Domain.Pedidos;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public sealed class PedidoRepository(AppDbContext context) : IPedidoRepository
{
    public async Task<Pedido?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct = default)
    {
        var query = context.Pedidos.AsQueryable();

        if (context.Database.IsRelational())
            query = query.Include(p => p.Itens);

        return await query.FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId, ct);
    }

    public async Task<(IReadOnlyList<Pedido> Items, int Total)> ListarAsync(
        Guid usuarioId, int pagina, int tamanhoPagina, CancellationToken ct = default)
    {
        var query = context.Pedidos.AsQueryable();

        if (context.Database.IsRelational())
            query = query.Include(p => p.Itens);

        query = query.Where(p => p.UsuarioId == usuarioId).OrderByDescending(p => p.CriadoEm);

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AdicionarAsync(Pedido pedido, CancellationToken ct = default) =>
        await context.Pedidos.AddAsync(pedido, ct);

    public void Atualizar(Pedido pedido) =>
        context.Pedidos.Update(pedido);
}
