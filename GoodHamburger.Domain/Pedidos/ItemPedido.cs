namespace GoodHamburger.Domain.Pedidos;

public sealed record ItemPedido(TipoItem Tipo, string Nome, decimal Preco);
