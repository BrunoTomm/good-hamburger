using FluentAssertions;
using GoodHamburger.Domain.Pedidos;

namespace GoodHamburger.Tests.Domain;

public sealed class PedidoDescontoTests
{
    private static Pedido CriarPedidoComItens(params TipoItem[] tipos)
    {
        var pedido = Pedido.Criar(Guid.NewGuid()).Value;
        foreach (var tipo in tipos)
        {
            var item = tipo switch
            {
                TipoItem.Sanduiche => new ItemPedido(TipoItem.Sanduiche, "X Burger", 5.00m),
                TipoItem.Batata => new ItemPedido(TipoItem.Batata, "Batata Frita", 2.00m),
                TipoItem.Refrigerante => new ItemPedido(TipoItem.Refrigerante, "Refrigerante", 2.50m),
                _ => throw new ArgumentOutOfRangeException()
            };
            pedido.AdicionarItem(item);
        }
        return pedido;
    }

    [Fact]
    public void ComboCompleto_DeveAplicar20PorCentoDeDesconto()
    {
        var pedido = CriarPedidoComItens(TipoItem.Sanduiche, TipoItem.Batata, TipoItem.Refrigerante);

        pedido.PercentualDesconto.Should().Be(0.20m);
        pedido.Total.Should().Be(pedido.Subtotal * 0.80m);
    }

    [Fact]
    public void SanduicheComRefrigerante_DeveAplicar15PorCentoDeDesconto()
    {
        var pedido = CriarPedidoComItens(TipoItem.Sanduiche, TipoItem.Refrigerante);

        pedido.PercentualDesconto.Should().Be(0.15m);
        pedido.Total.Should().Be(pedido.Subtotal * 0.85m);
    }

    [Fact]
    public void SanduicheComBatata_DeveAplicar10PorCentoDeDesconto()
    {
        var pedido = CriarPedidoComItens(TipoItem.Sanduiche, TipoItem.Batata);

        pedido.PercentualDesconto.Should().Be(0.10m);
        pedido.Total.Should().Be(pedido.Subtotal * 0.90m);
    }

    [Fact]
    public void SanduicheSozinho_NaoDeveAplicarDesconto()
    {
        var pedido = CriarPedidoComItens(TipoItem.Sanduiche);

        pedido.PercentualDesconto.Should().Be(0m);
        pedido.Total.Should().Be(pedido.Subtotal);
    }

    [Fact]
    public void AdicionarItemDuplicado_DeveRetornarFalha()
    {
        var pedido = CriarPedidoComItens(TipoItem.Sanduiche);

        var resultado = pedido.AdicionarItem(new ItemPedido(TipoItem.Sanduiche, "X Egg", 4.50m));

        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Contain("Sanduiche");
    }

    [Fact]
    public void ComboCompleto_CalculaValoresCorretamente()
    {
        var pedido = CriarPedidoComItens(TipoItem.Sanduiche, TipoItem.Batata, TipoItem.Refrigerante);

        pedido.Subtotal.Should().Be(9.50m);
        pedido.Desconto.Should().Be(1.90m);
        pedido.Total.Should().Be(7.60m);
    }
}
