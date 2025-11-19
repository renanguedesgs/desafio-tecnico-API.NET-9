using Domain.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class ReportController : ControllerBase
{
    private readonly ILockService _lockService;
    private readonly ILogger<ReportController> _logger;

    public ReportController(ILockService lockService, ILogger<ReportController> logger)
    {
        _lockService = lockService;
        _logger = logger;
    }

    [HttpPost("/processar-relatorio")]
    public async Task<IActionResult> ProcessarRelatorio(CancellationToken ct)
    {
        _logger.LogInformation("Tentando adquirir o lock");
        var acquired = await _lockService.AcquireAsync(
            "processar-relatorio",
            expiry: TimeSpan.FromSeconds(10),
            wait: TimeSpan.FromSeconds(5),
            retryInterval: TimeSpan.FromMilliseconds(200),
            ct);

        if (!acquired)
        {
            _logger.LogWarning("Recurso ocupado");
            return StatusCode(423, new { message = "Recurso ocupado" });
        }

        try
        {
            _logger.LogInformation("Lock adquirido");
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
            _logger.LogInformation("Executando o processo");
            return Ok(new { status = "Processado" });
        }
        finally
        {
            await _lockService.ReleaseAsync("processar-relatorio");
            _logger.LogInformation("Lock liberado");
        }
    }

}
