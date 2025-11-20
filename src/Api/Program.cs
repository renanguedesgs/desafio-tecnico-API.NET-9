using Api.Extensions;
using Api.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build(); 

app.SeedDatabase();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

namespace Api
{
    public partial class Program { }
}
