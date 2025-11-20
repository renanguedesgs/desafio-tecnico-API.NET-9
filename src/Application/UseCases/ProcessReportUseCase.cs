using Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace Application.UseCases;

public class ProcessReportUseCase
{
    private readonly ILockService _lockService;
    private readonly ILogger<ProcessReportUseCase> _logger;

    public ProcessReportUseCase(ILockService lockService, ILogger<ProcessReportUseCase> logger)
    {
        _lockService = lockService;
        _logger = logger;
    }

    public async Task<string> ExecuteAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Tentando adquirir o lock");

        if (!await _lockService.TryAcquireAsync("report", TimeSpan.FromSeconds(5), ct))
        {
            _logger.LogWarning("Recurso ocupado");
            return "Recurso ocupado. Tente novamente mais tarde.";
        }

        try
        {
            _logger.LogInformation("Lock adquirido");
            _logger.LogInformation("Executando o processo");
            Thread.Sleep(5000);
            return "Processo concluído";
        }
        finally
        {
            await _lockService.ReleaseAsync("report", ct);
            _logger.LogInformation("Lock liberado");
        }
    }

    public string Execute()
    {
        return ExecuteAsync().GetAwaiter().GetResult();
    }
}
