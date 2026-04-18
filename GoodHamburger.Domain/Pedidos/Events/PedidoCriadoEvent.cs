using GoodHamburger.Domain.Common;

namespace GoodHamburger.Domain.Pedidos.Events;

public sealed record PedidoCriadoEvent(Guid PedidoId, Guid UsuarioId) : IDomainEvent;
