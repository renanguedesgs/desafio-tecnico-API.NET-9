using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("processar-relatorio")]
public class ReportController : ControllerBase
{
    private readonly ProcessReportUseCase _useCase;

    public ReportController(ProcessReportUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpPost]
    public async Task<IActionResult> Process(CancellationToken ct)
    {
        var result = await _useCase.ExecuteAsync(ct);

        return result == "Processo concluído"
            ? Ok(new { message = result })
            : StatusCode(423, new { message = result });
    }
}
