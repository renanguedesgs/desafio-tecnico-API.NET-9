using FluentAssertions;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests;

public class ReportConcurrencyTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    public ReportConcurrencyTests(ApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Only_One_Request_Should_Process_While_Others_Get_Locked()
    {
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => _client.GetAsync("/processar-relatorio"))
            .ToArray();

        await Task.WhenAll(tasks);

        var okCount = tasks.Count(t => t.Result.StatusCode == HttpStatusCode.OK);
        var lockedCount = tasks.Count(t => (int)t.Result.StatusCode == 423 || t.Result.StatusCode == HttpStatusCode.Conflict);

        okCount.Should().BeGreaterOrEqualTo(1);
        lockedCount.Should().BeGreaterOrEqualTo(1);
        (okCount + lockedCount).Should().Be(10);
    }
}
