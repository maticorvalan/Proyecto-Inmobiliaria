using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Inmobiliaria.Models;
using System.Collections.Generic;
using System.Linq;

namespace Proyecto_Inmobiliaria.Controllers
{
    [Authorize]
    public class PropietariosController : Controller
    {
        private readonly IRepositorioPropietario repositorioPropietario;

        public PropietariosController(IRepositorioPropietario repo)
        {
            repositorioPropietario = repo;

        }

        // GET: Propietarios
        public ActionResult Index(int? pagina = 1)
        {     

            var propietarios = repositorioPropietario.ObtenerTodos().OrderBy(i => i.Id);

            int pageNumber = pagina ?? 1; // Si pagina es null, usar 1
            int pageSize = 5;

            int totalPaginas = (int)Math.Ceiling((double)propietarios.Count() / pageSize);

            ViewBag.Pagina = pageNumber;
            ViewBag.TotalPaginas = totalPaginas;

            var propietariosPaginados = propietarios
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return View(propietariosPaginados);
        }

        // GET: Propietarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            propietario.Estado = true; // Establecer estado por defecto a true
            if (ModelState.IsValid)
            {
                repositorioPropietario.Alta(propietario);
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        // GET: Propietarios/Edit/5
        public IActionResult Edit(int id)
        {
            var propietario = repositorioPropietario.ObtenerPorId(id);
            return View(propietario);
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            if (id != propietario.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                //propietario.Estado = propietario.Estado ?? false; // Establece a false si no se proporciona
                repositorioPropietario.Modificacion(propietario);
                return RedirectToAction(nameof(Index));
            }
            return View(propietario);
        }

        // GET: Propietarios/Eliminar/5
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var propietario = repositorioPropietario.ObtenerPorId(id);
            return View(propietario);
        }

        // POST: Propietarios/Eliminar/{id}
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public IActionResult EliminarConfirmed(
            int id,
            [FromServices] IRepositorioInmueble repositorioInmueble,
            [FromServices] IRepositorioImagen repoImagen,
            [FromServices] IWebHostEnvironment environment,
            [FromServices] IRepositorioContrato repoContrato
            )
        {
            try
            {
                // 0. Verificar si el propietario tiene contratos activos en uno de sus Inmuebles
                var inmuebles = repositorioInmueble.BuscarPorPropietario(id);
                if (inmuebles != null || inmuebles.Any())
                {
                    foreach (var inmueble in inmuebles)
                    {
                        var contratoActivo = repoContrato.ObtenerPorInmueble(inmueble.Id);
                        if (contratoActivo != null)
                        {
                            // Guardar mensaje en TempData
                            TempData["Error"] = $"No se puede eliminar el propietario porque su inmueble ID:{inmueble.Id}, en la direccion; {inmueble.Direccion}, tiene un contrato vigente.";

                            // Redirigir a la vista Eliminar del Propietario
                            return RedirectToAction("Eliminar", "Propietarios", new { id });
                        }
                    }
                }
                // Si no hay contratos activos => eliminar 
                if (inmuebles != null || inmuebles.Any())
                {
                    foreach (var inmueble in inmuebles)
                    {

                        // 1. Eliminar portada si existe
                        if (!string.IsNullOrEmpty(inmueble.UrlPortada))
                        {
                            string rutaPortada = Path.Combine(environment.WebRootPath,
                                inmueble.UrlPortada.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                            if (System.IO.File.Exists(rutaPortada))
                            {
                                System.IO.File.Delete(rutaPortada);
                            }
                        }

                        // 2. Eliminar imágenes interiores
                        var imagenes = repoImagen.BuscarPorInmueble(inmueble.Id);
                        foreach (var img in imagenes)
                        {
                            if (!string.IsNullOrEmpty(img.Url))
                            {
                                string rutaImg = Path.Combine(environment.WebRootPath,
                                    img.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                                if (System.IO.File.Exists(rutaImg))
                                {
                                    System.IO.File.Delete(rutaImg);
                                }
                            }
                            repoImagen.Baja(img.Id);
                        }

                        // 3. Eliminar el inmueble
                        repositorioInmueble.Baja(inmueble.Id);
                    }
                }

                // 5. Eliminar el Propietario
                repositorioPropietario.Baja(id);

                TempData["Mensaje"] = "Propietario e Inmueble e imágenes eliminados correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult BuscarPorNombre(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
                return Json(new { datos = new List<Propietario>() });

            var lista = repositorioPropietario.BuscarPorNombre(q)
                                            .Take(5)
                                            .ToList();

            return Json(new { datos = lista });
        }
    }
}