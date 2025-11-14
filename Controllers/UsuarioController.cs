using System.Security.Claims;
using Proyecto_Inmobiliaria.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Inmobiliaria.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly IRepositorioUsuario repositorio;

        public UsuarioController(IConfiguration configuration, IWebHostEnvironment environment, IRepositorioUsuario repositorio)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.repositorio = repositorio;
        }
        // GET: Usuario
        [Authorize(Policy = "Administrador")]
        public ActionResult Index(int? pagina = 1)
        {     

            var usuarios = repositorio.ObtenerTodos().OrderBy(i => i.id);

            int pageNumber = pagina ?? 1; // Si pagina es null, usar 1
            int pageSize = 5;

            int totalPaginas = (int)Math.Ceiling((double)usuarios.Count() / pageSize);

            ViewBag.Pagina = pageNumber;
            ViewBag.TotalPaginas = totalPaginas;

            var usuariosPaginados = usuarios
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return View(usuariosPaginados);
        }

        // GET: Usuario/Detalle/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Detalle(int id)
        {
            ViewData["Title"] = "Detalle del Usuario";
            if (User.Identity?.Name == null)
                return RedirectToAction("Login");
            var u = repositorio.ObtenerPorId(id);
            return View("Detalle", u);
        }

        // GET: Usuario/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Crear()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Crear(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Hasheo(u.Clave);
                u.Clave = hashed;
                u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)Usuario.enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorio.Alta(u);
                if (u.AvatarFile != null && u.id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads", "Avatares");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads/Avatares", fileName);
                    // Esta operación guarda la foto en memoria en la ruta que necesitamos
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }                    
                    repositorio.Modificacion(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                Console.WriteLine(ex.Message);
                return View();
            }
        }

        // GET: Usuario/Perfil/
        [Authorize]
        public ActionResult Perfil()
        {
            var idUsuarioClaim = User.FindFirst("IdUsuario")?.Value;
            if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim, out var id))
            {
                return Unauthorized();
            }

            var u = repositorio.ObtenerPorId(id);
            if (u == null)
            {
                return NotFound();
            }

            ViewData["Title"] = "Mi Perfil";
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Editar", u); // Reutiliza la vista Editar
        }

        // GET: Usuario/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Editar(int id)
        {        
            ViewData["Title"] = "Editar perfil";
            var u = repositorio.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }

      

        // POST: Usuario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> Editar(int id, Usuario u)
        {
            var vista = nameof(Editar);
            try
            {
                // Validaciones de seguridad
                if (!User.IsInRole("Administrador"))
                {
                    vista = nameof(Perfil);
                    if (User.Identity?.Name == null) return RedirectToAction("Login");
                    var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.id != id)
                        return RedirectToAction(nameof(Index), "Home");
                }

                var usuarioBD = repositorio.ObtenerPorId(id);
                if (usuarioBD == null) return NotFound();

                // Si viene una nueva imagen, reemplazo
                if (u.AvatarFile != null)
                {
                    if (!string.IsNullOrEmpty(usuarioBD.Avatar))
                    {
                        string rutaEliminar = Path.Combine(environment.WebRootPath,
                            usuarioBD.Avatar.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                        if (System.IO.File.Exists(rutaEliminar))
                            System.IO.File.Delete(rutaEliminar);
                    }

                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads", "Avatares");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string fileName = $"avatar_{u.id}_{Guid.NewGuid()}{Path.GetExtension(u.AvatarFile.FileName)}";
                    string pathCompleto = Path.Combine(path, fileName);

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }

                    u.Avatar = Path.Combine("/Uploads/Avatares", fileName);
                }
                else
                {
                    // Mantener el avatar anterior
                    u.Avatar = usuarioBD.Avatar;
                }

                u.id = id;
                string hashed = Hasheo(u.Clave);
                u.Clave = hashed;
                u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)Usuario.enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                repositorio.Modificacion(u);

                // Actualizar la cookie de autenticación si el usuario editó su propio perfil
                if (User.Identity != null && User.Identity.Name == usuarioBD.Email)
                {
                    // Crear nuevas claims con los datos actualizados
                   var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, u.Email),
                        new Claim("FullName", u.Nombre + " " + u.Apellido),
                        new Claim(ClaimTypes.Role, ((Usuario.enRoles)u.Rol).ToString()),
                        new Claim("IdUsuario", u.id.ToString()),
                        new Claim("Avatar", string.IsNullOrEmpty(u.Avatar) ? "/Uploads/Avatares/usuarioSinImagen.png" : u.Avatar.Replace('\\','/'))
                    };


                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true // mantiene sesión iniciada
                    };

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // cerrar la actual
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );
                }

                // Redirección según el rol
                if (u.Rol == (int)Usuario.enRoles.Empleado)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View(vista, u);
            }
        }


        // GET: Usuarios/Eliminar/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            
            var usuario = repositorio.ObtenerPorId(id);           
            return View(usuario);
        }

        // POST: Usuarios/EliminarConfirmado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult EliminarConfirmado(int id)
        {
            try
            {
                var usuario = repositorio.ObtenerPorId(id);               

                // Si el usuario es Administrador
                if (usuario.Rol == (int)Usuario.enRoles.Administrador)
                {
                    // Contar cuántos administradores hay en total
                    var cantidadAdmins = repositorio.ObtenerTodos()
                        .Count(u => u.Rol == (int)Usuario.enRoles.Administrador);

                    if (cantidadAdmins <= 1)
                    {
                        // Si es el único administrador → no permitir borrar
                        TempData["Error"] = "No se puede eliminar este usuario porque es el único administrador.";
                        return RedirectToAction(nameof(Eliminar), new { id });
                    }
                }

                // Eliminar avatar 
                if (!string.IsNullOrEmpty(usuario.Avatar))
                {
                    string ruta = Path.Combine(environment.WebRootPath, usuario.Avatar.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(ruta))
                        System.IO.File.Delete(ruta);
                }

                // Eliminar de la base
                repositorio.Baja(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error al intentar eliminar el usuario: " + ex.Message;
                return RedirectToAction(nameof(Eliminar), new { id });
            }
        }


        [Authorize]
        public IActionResult Avatar()
        {
            if (User.Identity?.Name == null) return RedirectToAction("Login");
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            string fileName = "avatar_" + u.id + Path.GetExtension(u.Avatar);
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);

            //leer el archivo
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
            //devolverlo
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [Authorize]
        public string AvatarBase64()
        {
            if (User.Identity?.Name == null) return "";
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            string fileName = "avatar_" + u.id + Path.GetExtension(u.Avatar);
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);

            //leer el archivo
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
            //devolverlo
            return Convert.ToBase64String(fileBytes);
        }

        [Authorize]
        [HttpPost("[controller]/[action]/{fileName}")]
        public IActionResult FromBase64([FromBody] string imagen, [FromRoute] string fileName)
        {
            //arma el path
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);
            //convierto a arreglo de bytes
            var bytes = Convert.FromBase64String(imagen);
            //lo escribe
            System.IO.File.WriteAllBytes(pathCompleto, bytes);
            return Ok();
        }

        [Authorize]
        public ActionResult Foto()
        {
            try
            {
                if (User.Identity?.Name == null) return RedirectToAction("Login");
                var u = repositorio.ObtenerPorEmail(User.Identity.Name);
                Console.WriteLine("Avatar: ", u.Avatar);
                var stream = System.IO.File.Open(
                        Path.Combine(environment.WebRootPath, u.Avatar.Substring(1)),
                        FileMode.Open,
                        FileAccess.Read);
                var ext = Path.GetExtension(u.Avatar);
                
                return new FileStreamResult(stream, $"image/{ext.Substring(1)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [Authorize]
        public ActionResult Datos()
        {
            try
            {
                var u = repositorio.ObtenerPorEmail(User.Identity.Name);
                string buffer = "Nombre;Apellido;Email" + Environment.NewLine +
                        $"{u.Nombre};{u.Apellido};{u.Email}";
                var stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(buffer));
                var res = new FileStreamResult(stream, "text/plain");
                res.FileDownloadName = "Datos.csv";
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult LoginModal()
        {
            return PartialView("_LoginModal", new LoginView());
        }

        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        // POST: Usuarios/Login/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            try
            {
                var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
                if (ModelState.IsValid)
                {
                    string hashed = Hasheo(login.Clave ?? "");
                    var e = repositorio.ObtenerPorEmail(login.Usuario ?? "");
                    if (e == null || e.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        TempData["returnUrl"] = returnUrl;
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                        new Claim("IdUsuario", e.id.ToString()),
                        new Claim("Avatar", string.IsNullOrEmpty(e.Avatar) ? "/Uploads/Avatares/usuarioSinImagen.png" : e.Avatar.Replace('\\','/'))                    };

                    var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));
                    TempData.Remove("returnUrl");
                    return Redirect(returnUrl);
                }
                TempData["returnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                Console.WriteLine(ex.Message);
                return View();
            }
        }

        // GET: /salir
        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public string Hasheo(string clave)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: clave,
                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));
        }
    }
    
}