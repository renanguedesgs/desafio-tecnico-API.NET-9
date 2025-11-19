using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class HomeController : Controller
{
    private readonly GetAllPatientsUseCase _useCase;

    public HomeController(GetAllPatientsUseCase useCase)
    {
        _useCase = useCase;
    }

    public IActionResult Index()
    {
        var patients = _useCase.Execute();
        return View(patients);
    }
}
