using System.Net.Http.Json;
using GoodHamburger.Blazor.Models;

namespace GoodHamburger.Blazor.Services;

public sealed class PedidoService(HttpClient http)
{
    public async Task<PagedResult<PedidoDto>?> ListarAsync(int pagina = 1, int tamanhoPagina = 10) =>
        await http.GetFromJsonAsync<PagedResult<PedidoDto>>($"pedidos?pagina={pagina}&tamanhoPagina={tamanhoPagina}");

    public async Task<PedidoDto?> ObterAsync(Guid id) =>
        await http.GetFromJsonAsync<PedidoDto>($"pedidos/{id}");

    public async Task<PedidoDto?> CriarAsync(List<string> itens)
    {
        var response = await http.PostAsJsonAsync("pedidos", new { itens });
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<PedidoDto>()
            : null;
    }

    public async Task<PedidoDto?> AtualizarAsync(Guid id, List<string> itens)
    {
        var response = await http.PutAsJsonAsync($"pedidos/{id}", new { itens });
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<PedidoDto>()
            : null;
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        var response = await http.DeleteAsync($"pedidos/{id}");
        return response.IsSuccessStatusCode;
    }
}
