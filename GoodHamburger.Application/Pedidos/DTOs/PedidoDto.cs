namespace GoodHamburger.Application.Pedidos.DTOs;

public sealed record PedidoDto(
    Guid Id,
    IReadOnlyList<ItemPedidoDto> Itens,
    decimal Subtotal,
    decimal PercentualDesconto,
    decimal Desconto,
    decimal Total,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public sealed record ItemPedidoDto(string Tipo, string Nome, decimal Preco);
