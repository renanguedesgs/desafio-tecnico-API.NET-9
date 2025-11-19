using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("patients")]
public class PatientsController : Controller
{
    private readonly GetAllPatientsUseCase _getAll;
    private readonly CreatePatientUseCase _create;
    private readonly UpdatePatientUseCase _update;
    private readonly DeletePatientUseCase _delete;

    public PatientsController(
        GetAllPatientsUseCase getAll,
        CreatePatientUseCase create,
        UpdatePatientUseCase update,
        DeletePatientUseCase delete)
    {
        _getAll = getAll;
        _create = create;
        _update = update;
        _delete = delete;
    }

    // GET /patients
    [HttpGet("/")]
    public IActionResult Index()
    {
        var patients = _getAll.Execute();
        return View(patients);
    }

    // GET /patients/{id}
    [HttpGet("details/{id:int}")]
    public IActionResult Details(int id)
    {
        var patients = _getAll.Execute();
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient is null) return NotFound();
        return View(patient);
    }

    // POST /patients
    [HttpPost("create")]
    public IActionResult Create(PatientDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        _create.Execute(dto);
        return RedirectToAction(nameof(Index));
    }

    // GET /patients/edit/{id}
    [HttpGet("edit/{id:int}")]
    public IActionResult Edit(int id)
    {
        var patients = _getAll.Execute();
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient is null) return NotFound();
        return View(patient);
    }

    // POST /patients/edit/{id}
    [HttpPost("edit/{id:int}")]
    public IActionResult Edit(PatientDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        _update.Execute(dto);
        return RedirectToAction(nameof(Index));
    }

    // POST /patients/delete/{id}
    [HttpPost("delete/{id:int}")]
    public IActionResult Delete(int id)
    {
        _delete.Execute(id);
        return RedirectToAction(nameof(Index));
    }
}
