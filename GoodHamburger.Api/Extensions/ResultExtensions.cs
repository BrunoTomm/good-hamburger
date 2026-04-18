using GoodHamburger.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller) =>
        result.IsSuccess
            ? controller.NoContent()
            : controller.Problem(detail: result.Error, statusCode: 422, title: "Operação inválida");

    public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller, int successStatus = 200) =>
        result.IsSuccess
            ? successStatus == 201
                ? controller.StatusCode(201, result.Value)
                : controller.Ok(result.Value)
            : result.Error == "Pedido não encontrado." || result.Error == "Usuário não encontrado."
                ? controller.NotFound(new ProblemDetails { Title = "Não encontrado", Detail = result.Error, Status = 404 })
                : controller.UnprocessableEntity(new ProblemDetails { Title = "Operação inválida", Detail = result.Error, Status = 422 });
}
