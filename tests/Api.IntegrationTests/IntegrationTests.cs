using Api;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Api.IntegrationTests;

public class PatientsCrudIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PatientsCrudIntegrationTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact(DisplayName = "CREATE - Deve adicionar paciente via API")]
    public async Task Create_Patient_Should_Work()
    {
        var patient = new Patient(0, "Maria Silva", new(1985, 4, 12), "Hemograma");
        var response = await _client.PostAsJsonAsync("/patients", patient);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(DisplayName = "READ - Deve recuperar paciente via API")]
    public async Task Read_Patient_Should_Work()
    {
        var response = await _client.GetAsync("/patients/1");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var patient = await response.Content.ReadFromJsonAsync<Patient>();
        patient!.Name.Should().Be("Maria Silva");
    }

    [Fact(DisplayName = "UPDATE - Deve atualizar paciente via API")]
    public async Task Update_Patient_Should_Work()
    {
        var patient = new Patient(1, "Maria Silva", new(1985, 4, 12), "Raio-X");
        var response = await _client.PutAsJsonAsync("/patients/1", patient);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "DELETE - Deve remover paciente via API")]
    public async Task Delete_Patient_Should_Work()
    {
        var response = await _client.DeleteAsync("/patients/1");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
