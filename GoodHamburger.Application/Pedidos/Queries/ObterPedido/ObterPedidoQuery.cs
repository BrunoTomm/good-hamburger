using GoodHamburger.Application.Pedidos.DTOs;
using GoodHamburger.Domain.Common;
using GoodHamburger.Domain.Pedidos;
using MediatR;

namespace GoodHamburger.Application.Pedidos.Queries.ObterPedido;

public sealed record ObterPedidoQuery(Guid Id, Guid UsuarioId) : IRequest<Result<PedidoDto>>;

public sealed class ObterPedidoQueryHandler(IPedidoRepository repository)
    : IRequestHandler<ObterPedidoQuery, Result<PedidoDto>>
{
    public async Task<Result<PedidoDto>> Handle(ObterPedidoQuery request, CancellationToken ct)
    {
        var pedido = await repository.ObterPorIdAsync(request.Id, request.UsuarioId, ct);

        if (pedido is null)
            return Result.Failure<PedidoDto>("Pedido não encontrado.");

        return Result.Success(new PedidoDto(
            pedido.Id,
            pedido.Itens.Select(i => new ItemPedidoDto(i.Tipo.ToString(), i.Nome, i.Preco)).ToList(),
            pedido.Subtotal,
            pedido.PercentualDesconto,
            pedido.Desconto,
            pedido.Total,
            pedido.CriadoEm,
            pedido.AtualizadoEm));
    }
}
