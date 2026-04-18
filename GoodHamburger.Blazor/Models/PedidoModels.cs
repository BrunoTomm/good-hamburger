namespace GoodHamburger.Blazor.Models;

public record ItemPedidoDto(string Tipo, string Nome, decimal Preco);

public record PedidoDto(
    Guid Id,
    List<ItemPedidoDto> Itens,
    decimal Subtotal,
    decimal PercentualDesconto,
    decimal Desconto,
    decimal Total,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public record PagedResult<T>(List<T> Items, int Total, int Pagina, int TamanhoPagina, int TotalPaginas);

public record ItemCardapioDto(string Nome, decimal Preco, string Tipo, string Categoria);
