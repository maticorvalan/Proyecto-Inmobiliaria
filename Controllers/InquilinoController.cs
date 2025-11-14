using System.Data;
using Proyecto_Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_Inmobiliaria.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly RepositorioInquilino repositorio;

        public InquilinoController(IConfiguration config)
        {
            this.repositorio = new RepositorioInquilino(config);
        }
        [HttpGet]
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                var tamaño = 5;
                var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
                ViewBag.Pagina = pagina;
                var total = repositorio.ObtenerCantidad();
                ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
                ViewBag.id = TempData["id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al obtener la lista de inquilinos: {ex.Message}");
                return View();
            }
        }
        [HttpGet]
        public IActionResult Editar(int id)
        {
            if (id > 0)
            {
                var inquilino = repositorio.ObtenerPorId(id);
                return View(inquilino);
            }
            else
            {
                return View();
            }
        }
        public IActionResult Detalle(int id)
        {
            if (id > 0)
            {
                var inquilino = repositorio.ObtenerPorId(id);
                return View(inquilino);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Guardar(Inquilino inquilino)
        {
            if (!ModelState.IsValid)
            {
                return View(inquilino);
            }
            if (inquilino.id > 0)
            {
                repositorio.Modificacion(inquilino);
                TempData["SuccessMessage"] = "Inquilino actualizado correctamente.";
            }
            else
            {
                repositorio.Alta(inquilino);
                TempData["SuccessMessage"] = "Inquilino creado correctamente.";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["SuccessMessage"] = "Inquilino eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = "No se pudo eliminar el inquilino debido a que posee un contrato vigente";
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
    
}