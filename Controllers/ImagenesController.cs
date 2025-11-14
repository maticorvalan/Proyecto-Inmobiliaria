using Microsoft.AspNetCore.Mvc;
using Proyecto_Inmobiliaria.Models;

public class ImagenesController : Controller
{
    private readonly IRepositorioImagen repo;
    private readonly IWebHostEnvironment environment;

    public ImagenesController(IRepositorioImagen repo, IWebHostEnvironment environment)
    {
        this.repo = repo;
        this.environment = environment;
    }

    [HttpPost]
    public IActionResult Alta(int id, List<IFormFile> imagenes)
    {
        var result = new List<Imagen>();
        try
        {
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads", "Inmuebles");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            foreach (var archivo in imagenes)
            {
                if (archivo != null && archivo.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(archivo.FileName);
                    string rutaFisicaCompleta = Path.Combine(path, fileName);

                    using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
                    {
                        archivo.CopyTo(stream);
                    }

                    var img = new Imagen
                    {
                        InmuebleId = id,
                        Url = Path.Combine("/Uploads/Inmuebles", fileName)
                    };

                    repo.Alta(img);
                    result.Add(img);
                }
            }
            return Json(repo.BuscarPorInmueble(id)); // Devuelve todas las imágenes del inmueble
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult Eliminar(int id)
    {
        try
        {
            var img = repo.ObtenerPorId(id);
            if (img != null)
            {
                // Eliminar archivo físico
                string rutaEliminar = Path.Combine(environment.WebRootPath, img.Url.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(rutaEliminar))
                {
                    System.IO.File.Delete(rutaEliminar);
                }
                repo.Baja(id);
                return Json(repo.BuscarPorInmueble(img.InmuebleId)); // Devuelve lista actualizada
            }
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
