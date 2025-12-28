using Microsoft.AspNetCore.Mvc;

namespace Veilingklok.Core.Shared;

public static class ApiResultExtensions
{
    public static IActionResult ToActionResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.Success)
            return controller.Ok(result.Value);

        var msg = result.Error ?? "Onbekende fout.";
        return controller.BadRequest(msg);
    }
}