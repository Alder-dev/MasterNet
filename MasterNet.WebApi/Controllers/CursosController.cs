using MasterNet.Application.Core;
using MasterNet.Application.Cursos.CursoCreate;
using MasterNet.Application.Cursos.CursoUpdate;
using MasterNet.Application.Cursos.GetCursos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Cursos.CursoCreate.CursoCreateCommand;
using static MasterNet.Application.Cursos.CursoCreate.CursoDeleteCommand;
using static MasterNet.Application.Cursos.CursoReporteExcel.ReportEQuery;
using static MasterNet.Application.Cursos.CursoUpdate.CursoUpdateCommand;
using static MasterNet.Application.Cursos.GetCurso.GetCursoQuery;
using static MasterNet.Application.Cursos.GetCursos.GetCursosQuery;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/cursos")]
public class CursosController : ControllerBase
{
    private readonly ISender _sender;

    public CursosController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<Result<Guid>>> CursoCreate([FromForm] CursoCreateRequest request, CancellationToken cancellationToken)
    {
        var command = new CursoCreateCommandRequest(request);
        return await _sender.Send(command, cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<Guid>>> CursoUpdate([FromBody] CursoUpdateRequest request, Guid id, CancellationToken cancellationToken)
    {
        var command = new CursoUpdateCommandRequest(request, id);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : BadRequest();
    }

    [HttpGet]
    public async Task<ActionResult> PaginationCursos([FromQuery] GetCursosRequest request, CancellationToken cancellationToken)
    {
        var query = new GetCursosQueryRequest { CursosRequest = request };
        var resultado = await _sender.Send(query, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : NotFound();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> CursoGet(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCursoQueryRequest { Id = id };
        var resultado = await _sender.Send(query, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Value) : BadRequest();
    }

    [HttpGet("report")]
    public async Task<IActionResult> ReporteCSV(CancellationToken cancellationToken)
    {
        var query = new ReportEQueryRequest();
        var result = await _sender.Send(query, cancellationToken);
        byte[] excelBytes = result.ToArray();
        return File(excelBytes, "text/csv", "cursos.csv");
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Unit>> CursoDelete(Guid id, CancellationToken cancellationToken)
    {
        var command = new CursoDeleteCommandRequest(id);
        var result = await _sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest();
    }

}