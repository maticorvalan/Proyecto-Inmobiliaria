using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Inmobiliaria.Models;
using System.Collections.Generic;

namespace Proyecto_Inmobiliaria.Controllers
{
    [Authorize(Policy = "Administrador")]
    public class AuditoriaController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly RepositorioAuditoria repositorioAuditoria;

        public AuditoriaController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorioAuditoria = new RepositorioAuditoria(configuration);
        }

        public IActionResult Index()
        {
            IList<Auditoria> auditorias = repositorioAuditoria.ObtenerAuditorias();
            return View(auditorias);
        }
    }
}