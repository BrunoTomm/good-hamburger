using GoodHamburger.Domain.Common;
using GoodHamburger.Domain.Pedidos.Events;

namespace GoodHamburger.Domain.Pedidos;

public sealed class Pedido : BaseEntity
{
    private readonly List<ItemPedido> _itens = [];

    private Pedido() { }

    public static Result<Pedido> Criar(Guid usuarioId)
    {
        var pedido = new Pedido { UsuarioId = usuarioId };
        pedido.AddDomainEvent(new PedidoCriadoEvent(pedido.Id, usuarioId));
        return Result.Success(pedido);
    }

    public Guid UsuarioId { get; private init; }
    public IReadOnlyList<ItemPedido> Itens => _itens.AsReadOnly();

    public decimal Subtotal => _itens.Sum(i => i.Preco);
    public decimal PercentualDesconto => CalcularDesconto();
    public decimal Desconto => Subtotal * PercentualDesconto;
    public decimal Total => Subtotal - Desconto;

    public Result AdicionarItem(ItemPedido item)
    {
        if (_itens.Any(i => i.Tipo == item.Tipo))
            return Result.Failure($"O pedido já contém um item do tipo '{item.Tipo}'.");

        _itens.Add(item);
        AtualizadoEm = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Atualizar(IEnumerable<ItemPedido> novosItens)
    {
        _itens.Clear();

        foreach (var item in novosItens)
        {
            var resultado = AdicionarItem(item);
            if (resultado.IsFailure)
                return resultado;
        }

        AddDomainEvent(new PedidoAtualizadoEvent(Id));
        return Result.Success();
    }

    public void Deletar() => DeletadoEm = DateTime.UtcNow;

    private decimal CalcularDesconto() => _itens.Select(i => i.Tipo).ToHashSet() switch
    {
        var t when t.IsSupersetOf([TipoItem.Sanduiche, TipoItem.Batata, TipoItem.Refrigerante]) => 0.20m,
        var t when t.IsSupersetOf([TipoItem.Sanduiche, TipoItem.Refrigerante]) => 0.15m,
        var t when t.IsSupersetOf([TipoItem.Sanduiche, TipoItem.Batata]) => 0.10m,
        _ => 0m
    };
}
