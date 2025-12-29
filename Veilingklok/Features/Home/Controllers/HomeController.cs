using Microsoft.AspNetCore.Mvc;
using Veilingklok.Features.Home.Dtos;

namespace Veilingklok.Features.Home;

[ApiController]
public sealed class HomeController : ControllerBase
{
    private readonly IHomeService _service;

    public HomeController(IHomeService service)
    {
        _service = service;
    }

    [HttpGet("/api/home")]
    [ProducesResponseType(typeof(HomeDto), StatusCodes.Status200OK)]
    public ActionResult<HomeDto> Get()
    {
        return Ok(_service.GetHome());
    }
}