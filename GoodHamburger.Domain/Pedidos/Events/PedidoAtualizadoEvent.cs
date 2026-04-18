using GoodHamburger.Domain.Common;

namespace GoodHamburger.Domain.Pedidos.Events;

public sealed record PedidoAtualizadoEvent(Guid PedidoId) : IDomainEvent;
