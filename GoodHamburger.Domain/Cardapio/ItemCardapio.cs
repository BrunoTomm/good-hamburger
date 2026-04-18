using GoodHamburger.Domain.Pedidos;

namespace GoodHamburger.Domain.Cardapio;

public sealed record ItemCardapio(string Nome, decimal Preco, TipoItem Tipo, string Categoria);

public static class Cardapio
{
    public static readonly IReadOnlyList<ItemCardapio> Itens =
    [
        new("X Burger",      5.00m, TipoItem.Sanduiche,    "Sanduíches"),
        new("X Egg",         4.50m, TipoItem.Sanduiche,    "Sanduíches"),
        new("X Bacon",       7.00m, TipoItem.Sanduiche,    "Sanduíches"),
        new("Batata Frita",  2.00m, TipoItem.Batata,       "Acompanhamentos"),
        new("Refrigerante",  2.50m, TipoItem.Refrigerante, "Acompanhamentos"),
    ];

    public static ItemCardapio? Encontrar(string nome) =>
        Itens.FirstOrDefault(i => string.Equals(i.Nome, nome, StringComparison.OrdinalIgnoreCase));
}
