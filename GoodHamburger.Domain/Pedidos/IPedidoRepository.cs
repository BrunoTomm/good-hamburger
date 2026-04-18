namespace GoodHamburger.Domain.Pedidos;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id, Guid usuarioId, CancellationToken ct = default);
    Task<(IReadOnlyList<Pedido> Items, int Total)> ListarAsync(Guid usuarioId, int pagina, int tamanhoPagina, CancellationToken ct = default);
    Task AdicionarAsync(Pedido pedido, CancellationToken ct = default);
    void Atualizar(Pedido pedido);
}
