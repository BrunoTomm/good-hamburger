using GoodHamburger.Domain.Pedidos.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.Pedidos.Events;

public sealed class PedidoAtualizadoEventHandler(ILogger<PedidoAtualizadoEventHandler> logger)
    : INotificationHandler<PedidoAtualizadoEvent>
{
    public Task Handle(PedidoAtualizadoEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Pedido {PedidoId} atualizado", notification.PedidoId);
        return Task.CompletedTask;
    }
}
