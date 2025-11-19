using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

public class PatientsEndpointTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public PatientsEndpointTests(ApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });
    }

    [Fact]
    public async Task Get_Patients_Should_Render_View_With_Data()
    {
        var response = await _client.GetAsync("/patients");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("Maria Silva").And.Contain("João Souza");

        // Optional: validate view markers
        Regex.IsMatch(html, "<table.*?>").Should().BeTrue();
    }
}
