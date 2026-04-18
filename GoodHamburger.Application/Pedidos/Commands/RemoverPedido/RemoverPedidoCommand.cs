using GoodHamburger.Application.Common.Interfaces;
using GoodHamburger.Domain.Common;
using GoodHamburger.Domain.Pedidos;
using MediatR;

namespace GoodHamburger.Application.Pedidos.Commands.RemoverPedido;

public sealed record RemoverPedidoCommand(Guid Id, Guid UsuarioId) : IRequest<Result>;

public sealed class RemoverPedidoCommandHandler(IPedidoRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<RemoverPedidoCommand, Result>
{
    public async Task<Result> Handle(RemoverPedidoCommand request, CancellationToken ct)
    {
        var pedido = await repository.ObterPorIdAsync(request.Id, request.UsuarioId, ct);
        if (pedido is null)
            return Result.Failure("Pedido não encontrado.");

        pedido.Deletar();
        repository.Atualizar(pedido);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
