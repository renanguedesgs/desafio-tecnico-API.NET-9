using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Api.Tests;

public class ApiIntegrationTests
{
    [Fact]
    public async Task HomeIndex_ShouldReturnSuccess()
    {
        await using var app = new WebApplicationFactory<Api.Program>();
        var client = app.CreateClient();
        var response = await client.GetAsync("/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ProcessarRelatorio_ConcurrentRequests_ShouldLockOthers()
    {
        await using var app = new WebApplicationFactory<Api.Program>();
        var client = app.CreateClient();

        var tasks = Enumerable.Range(0, 10).Select(_ => client.PostAsync("/processar-relatorio", content: null)).ToArray();
        await Task.WhenAll(tasks);

        var statusCodes = tasks.Select(t => t.Result.StatusCode).ToList();
        Assert.True(statusCodes.Count(s => s == HttpStatusCode.OK) >= 1);
        Assert.True(statusCodes.Count(s => (int)s == 423) >= 1);
    }
}
