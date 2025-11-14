using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

namespace Proyecto_Inmobiliaria.Controllers;

[Authorize]
public class PagoController : Controller
{
    private readonly IConfiguration configuration;

    private readonly IRepositorioPago repositorio;
    private readonly IRepositorioContrato repositorioContrato;

    public PagoController(IConfiguration configuration, IRepositorioPago repositorio, IRepositorioContrato repositorioContrato)
    {
        this.configuration = configuration;
        this.repositorio = repositorio;
        this.repositorioContrato = repositorioContrato;
    }

    public IActionResult Index()
    {
        var lista = repositorio.ObtenerTodos();
        if (TempData["SuccessMessage"] != null)
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
        }
        return View(lista);
    }
    public IActionResult Crear(int id)
    {

        ViewBag.Contratos = repositorioContrato.ObtenerTodos().Select(c => new
        {
            c.id,
            Descripcion = $"Contrato {c.id} - " +
            $"Inquilino: {(c.Inquilino != null ? $"{c.Inquilino.Nombre} {c.Inquilino.Apellido}" : "No especificado")} - " +
            $"Dueño: {(c.Inmueble?.Propietario != null ? $"{c.Inmueble.Propietario.Nombre} {c.Inmueble.Propietario.Apellido}" : "No especificado")} - " +
            $"Dirección: {c.Inmueble?.Direccion ?? "No especificada"}",
            c.Monto
        }).ToList();

        if (id > 0)
        {
            var pago = repositorio.ObtenerPorId(id);
            return View(pago);
        }
        else
        {
            return View();
        }
    }

    [HttpPost]
    public IActionResult Guardar(Pago pago)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Por favor, verifique los datos ingresados.";
            return RedirectToAction(nameof(Crear), new { idPago = pago.id });
        }
        RepositorioAuditoria ra = new RepositorioAuditoria(configuration);
        try
        {
            if (pago.id > 0)
            {
                repositorio.Modificacion(pago);
                ra.NuevaAuditoria("Modificación Pago", User.Identity?.Name ?? "Sistema", $"Pago ID: {pago.id}");
                TempData["SuccessMessage"] = "Pago actualizado correctamente.";
            }
            else
            {
                repositorio.Alta(pago);
                ra.NuevaAuditoria("Creación Pago", User.Identity?.Name ?? "Sistema", $"Contrato ID: {pago.IdContrato}");
                TempData["SuccessMessage"] = "Pago creado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            TempData["ErrorMessage"] = "Ocurrió un error al procesar el pago.";
            return RedirectToAction(nameof(Index));
        }
    }
    public IActionResult Editar(int id)
    {
        try
        {
            if (id > 0)
            {
                var pago = repositorio.ObtenerPorId(id);

                if (pago == null)
                {
                    TempData["ErrorMessage"] = "El pago solicitado no existe.";
                    return RedirectToAction(nameof(Index));
                }
                var contrato = repositorioContrato.ObtenerPorId(pago.IdContrato);
                ViewBag.Contrato = contrato;
                return View(pago);
            }
            else
            {
                TempData["ErrorMessage"] = "ID de pago inválido.";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            TempData["ErrorMessage"] = "Ocurrió un error al intentar cargar la edición.";
            return RedirectToAction(nameof(Index));
        }
    }
    public IActionResult Detalle(int id)
    {
        if (id > 0)
        {
            var pago = repositorio.ObtenerPorId(id);
            if (pago != null)
            {
                var contrato = repositorioContrato.ObtenerPorId(pago.IdContrato);
                ViewBag.Contrato = contrato;
                return View(pago);
            }
            else
            {
                TempData["ErrorMessage"] = "El pago solicitado no existe.";
                return RedirectToAction(nameof(Index));
            }
        }
        else
        {
            return View();
        }

    }
    [Authorize(Policy = "Administrador")]
    public IActionResult Eliminar(int id)
    {
        RepositorioAuditoria ra = new RepositorioAuditoria(configuration);

        try
        {
            repositorio.Baja(id);
            ra.NuevaAuditoria("Anulacion de Pago", User.Identity?.Name ?? "Sistema", $"Pago ID: {id}");
            TempData["SuccessMessage"] = "Estado del pago actualizado correctamente.";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            TempData["ErrorMessage"] = "No se pudo actualizar el estado del pago.";
        }
        return RedirectToAction(nameof(Index));
    }

}