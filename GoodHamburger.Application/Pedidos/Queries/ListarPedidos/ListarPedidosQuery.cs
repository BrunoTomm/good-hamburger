using GoodHamburger.Application.Common;
using GoodHamburger.Application.Pedidos.DTOs;
using GoodHamburger.Domain.Pedidos;
using MediatR;

namespace GoodHamburger.Application.Pedidos.Queries.ListarPedidos;

public sealed record ListarPedidosQuery(Guid UsuarioId, int Pagina = 1, int TamanhoPagina = 10)
    : IRequest<PagedResult<PedidoDto>>;

public sealed class ListarPedidosQueryHandler(IPedidoRepository repository)
    : IRequestHandler<ListarPedidosQuery, PagedResult<PedidoDto>>
{
    public async Task<PagedResult<PedidoDto>> Handle(ListarPedidosQuery request, CancellationToken ct)
    {
        var (items, total) = await repository.ListarAsync(request.UsuarioId, request.Pagina, request.TamanhoPagina, ct);
        return new PagedResult<PedidoDto>(items.Select(ToDto).ToList(), total, request.Pagina, request.TamanhoPagina);
    }

    private static PedidoDto ToDto(Pedido p) => new(
        p.Id,
        p.Itens.Select(i => new ItemPedidoDto(i.Tipo.ToString(), i.Nome, i.Preco)).ToList(),
        p.Subtotal,
        p.PercentualDesconto,
        p.Desconto,
        p.Total,
        p.CriadoEm,
        p.AtualizadoEm);
}
