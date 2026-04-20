namespace GoodHamburger.Web.Services;

public sealed class CartService
{
    private readonly Dictionary<string, CartItem> _items = new();

    public event Action? CartChanged;

    public IReadOnlyDictionary<string, CartItem> Items => _items;
    public int TotalItens => _items.Count;
    public decimal Subtotal => _items.Values.Sum(i => i.Preco);

    public decimal PercentualDesconto
    {
        get
        {
            var temSanduiche = _items.Values.Any(i => i.Categoria.Contains("Sanduí") || i.Categoria == "Sanduiches");
            var temBatata    = _items.Values.Any(i => i.Nome.Contains("Batata"));
            var temRefri     = _items.Values.Any(i => i.Nome.Contains("Refrigerante"));
            if (temSanduiche && temBatata && temRefri) return 0.20m;
            if (temSanduiche && temRefri)              return 0.15m;
            if (temSanduiche && temBatata)             return 0.10m;
            return 0m;
        }
    }

    public decimal Desconto => Subtotal * PercentualDesconto;
    public decimal Total    => Subtotal - Desconto;

    public bool Contem(string nome) => _items.ContainsKey(nome);

    public void Adicionar(string nome, string categoria, decimal preco)
    {
        _items[nome] = new CartItem(nome, categoria, preco);
        CartChanged?.Invoke();
    }

    public void Remover(string nome) { _items.Remove(nome); CartChanged?.Invoke(); }

    public void Toggle(string nome, string categoria, decimal preco)
    {
        if (_items.ContainsKey(nome)) Remover(nome);
        else Adicionar(nome, categoria, preco);
    }

    public void Limpar() { _items.Clear(); CartChanged?.Invoke(); }

    public List<string> ObterNomes() => [.. _items.Keys];
}

public sealed record CartItem(string Nome, string Categoria, decimal Preco);
