using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public sealed class CardapioService(HttpClient http)
{
    public async Task<List<ItemCardapioDto>?> ObterAsync() =>
        await http.GetFromJsonAsync<List<ItemCardapioDto>>("cardapio");
}