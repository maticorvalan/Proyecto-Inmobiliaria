using Microsoft.AspNetCore.Mvc;
using Proyecto_Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_Inmobiliaria.Controllers 
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPropietario repositorioPropietario;
        public InmueblesController(IRepositorioPropietario repositorioPropietario, IRepositorioInmueble repositorioInmueble)
        {
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPropietario = repositorioPropietario;
        }

        // GET: Inmuebles
        public IActionResult Index(int? pagina = 1, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            IEnumerable<Inmueble> inmuebles;

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                // Buscar inmuebles disponibles en el rango de fechas
                inmuebles = repositorioInmueble.ObtenerDisponiblesPorFecha(fechaInicio.Value, fechaFin.Value)
                    .OrderBy(i => i.Id);
            }
            else
            {
                // Si no se ingresaron fechas
                inmuebles = repositorioInmueble.ObtenerTodos()
                    .OrderBy(i => i.Id);
            }

            int pageNumber = pagina ?? 1;
            int pageSize = 5;

            int totalPaginas = (int)Math.Ceiling((double)inmuebles.Count() / pageSize);

            ViewBag.Pagina = pageNumber;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            var inmueblesPaginados = inmuebles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return View(inmueblesPaginados);
        }      


        // GET: Inmuebles/Create
        public IActionResult Create()
        {
            var propietarios = repositorioPropietario.ObtenerTodos().Select(p => new SelectListItem
            {
                Text = $"{p.Nombre} {p.Apellido} / {p.Dni}", // Texto a mostrar en la lista desplegable
                Value = p.Id.ToString() // Valor que se enviará al servidor
            }).ToList();
            ViewBag.Propietarios = propietarios;
            return View();
        }

        // POST: Inmuebles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                repositorioInmueble.Alta(inmueble);
                return RedirectToAction(nameof(Index));
            }
            return View(inmueble);
        }

        // GET: Inmuebles/Edit/{id}
        public IActionResult Edit(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);
            if (inmueble == null) return NotFound();

            inmueble.Propietario = repositorioPropietario.ObtenerPorId(inmueble.IdPropietario);
            return View(inmueble);
        }

        // POST: Inmuebles/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                repositorioInmueble.Modificacion(inmueble);
                return RedirectToAction(nameof(Index));
            }

            inmueble.Propietario = repositorioPropietario.ObtenerPorId(inmueble.IdPropietario);
            return View(inmueble);
        }

        // GET: Inmuebles/Eliminar/{id}
        [Authorize(Policy = "Administrador")]
        public IActionResult Eliminar(int id)
        {
            var inmueble = repositorioInmueble.ObtenerPorId(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // POST: Inmuebles/Eliminar/{id}
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public IActionResult EliminarConfirmed(
            int id,
            [FromServices] IRepositorioImagen repoImagen,
            [FromServices] IWebHostEnvironment environment,
            [FromServices] IRepositorioContrato repoContrato)
        {
            try
            {
                // 0. Traer el inmueble
                var inmueble = repositorioInmueble.ObtenerPorId(id);

                if (inmueble == null)
                {
                    return NotFound();
                }
                // 1. Verificar si el Inmuble tiene contratos activos
                var contratoActivo = repoContrato.ObtenerPorInmueble(id);
                if (contratoActivo != null)
                {
                    TempData["Error"] = $"No se puede eliminar el inmueble porque tiene un contrato vigente.";
                    // Redirigir a la vista Eliminar del Inmueble
                    return RedirectToAction("Eliminar", "Inmuebles", new { id });
                }

                // 2. Eliminar portada si existe
                if (!string.IsNullOrEmpty(inmueble.UrlPortada))
                {
                    string rutaPortada = Path.Combine(environment.WebRootPath,
                        inmueble.UrlPortada.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(rutaPortada))
                    {
                        System.IO.File.Delete(rutaPortada);
                    }
                }

                // 3. Eliminar imágenes interiores
                var imagenes = repoImagen.BuscarPorInmueble(id);
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

                // 4. Eliminar el inmueble
                repositorioInmueble.Baja(id);

                TempData["Mensaje"] = "Inmueble e imágenes eliminados correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Inmuebles/Imagenes/5
        public ActionResult Imagenes(int id, [FromServices] IRepositorioImagen repoImagen)
        {
            var entidad = repositorioInmueble.ObtenerPorId(id);
            entidad.Imagenes = repoImagen.BuscarPorInmueble(id);
            return View(entidad);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Portada(int InmuebleId, IFormFile PortadaFile, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                var inmueble = repositorioInmueble.ObtenerPorId(InmuebleId);
                if (inmueble != null && !string.IsNullOrEmpty(inmueble.UrlPortada))
                {
                    string rutaEliminar = Path.Combine(environment.WebRootPath, inmueble.UrlPortada.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(rutaEliminar))
                    {
                        System.IO.File.Delete(rutaEliminar);
                    }
                }

                string url = null;
                if (PortadaFile != null)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads", "Inmuebles");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = "portada_" + InmuebleId + Path.GetExtension(PortadaFile.FileName);
                    string rutaFisicaCompleta = Path.Combine(path, fileName);

                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        PortadaFile.CopyTo(stream);
                    }

                    url = "/Uploads/Inmuebles/" + fileName;
                }

                repositorioInmueble.ModificarPortada(InmuebleId, url);

                TempData["Mensaje"] = "Portada actualizada correctamente";
                return RedirectToAction(nameof(Imagenes), new { id = InmuebleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Imagenes), new { id = InmuebleId });
            }
        }

        // GET: Inmuebles/PorPropietario/5
        public IActionResult PorPropietario(int id)
        {
            var propietario = repositorioPropietario.ObtenerPorId(id);
            var inmuebles = repositorioInmueble.BuscarPorPropietario(id);
            // Le pasamos el propietario y sus inmuebles a la vista
            ViewBag.Propietario = propietario;
            return View(inmuebles);
        }




    }
}
