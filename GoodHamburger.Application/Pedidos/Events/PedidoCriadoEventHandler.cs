using GoodHamburger.Domain.Pedidos.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GoodHamburger.Application.Pedidos.Events;

public sealed class PedidoCriadoEventHandler(ILogger<PedidoCriadoEventHandler> logger)
    : INotificationHandler<PedidoCriadoEvent>
{
    public Task Handle(PedidoCriadoEvent notification, CancellationToken ct)
    {
        logger.LogInformation("Pedido {PedidoId} criado pelo usuário {UsuarioId}", notification.PedidoId, notification.UsuarioId);
        return Task.CompletedTask;
    }
}
