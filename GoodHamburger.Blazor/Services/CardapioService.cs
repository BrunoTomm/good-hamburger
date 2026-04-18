using System.Net.Http.Json;
using GoodHamburger.Blazor.Models;

namespace GoodHamburger.Blazor.Services;

public sealed class CardapioService(HttpClient http)
{
    public async Task<List<ItemCardapioDto>?> ObterAsync() =>
        await http.GetFromJsonAsync<List<ItemCardapioDto>>("cardapio");
}
