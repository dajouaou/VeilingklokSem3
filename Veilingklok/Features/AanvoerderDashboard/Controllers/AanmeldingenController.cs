using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veilingklok.Core.Entities;
using Veilingklok.Features.AanvoerdersDashboard.Dtos;
using Veilingklok.Features.AanvoerdersDashboard.Services;

namespace Veilingklok.Features.AanvoerdersDashboard.Controllers;

[Authorize(Roles = "Aanvoerder")]
[ApiController]
[Route("api/aanmeldingen")]
public class AanmeldingenController : ControllerBase
{
    private readonly AanmeldingService _service;
    private readonly IMapper _mapper;

    public AanmeldingenController(AanmeldingService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AanmeldingDto>>> GetAanmeldingen()
    {
        var data = await _service.GetAanmeldingenAsync();
        return Ok(_mapper.Map<IEnumerable<AanmeldingDto>>(data));
    }

    [HttpPost]
    public async Task<ActionResult> CreateAanmelding([FromBody] NieuweAanmeldingDto dto)
    {
        if (dto.Aantal <= 0 || dto.MinimumPrijs <= 0)
            return BadRequest("Aantal en minimumprijs moeten groter zijn dan 0.");

        var entity = _mapper.Map<Aanmelding>(dto);
        await _service.CreateAanmeldingAsync(entity);

        return Created("", null);
    }
}
