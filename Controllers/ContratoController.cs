using Proyecto_Inmobiliaria.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Proyecto_Inmobiliaria.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IRepositorioContrato repositorio;
        private readonly IRepositorioInquilino rinq;
        private readonly IRepositorioInmueble rinm;
        private readonly IRepositorioPago rpago;

        public ContratoController(IConfiguration config, IRepositorioContrato repositorio, IRepositorioPago rpago, IRepositorioInquilino rinq, IRepositorioInmueble rinm)
        {
            this.configuration = config;
            this.rinq = rinq;
            this.rinm = rinm;
            this.repositorio = repositorio;
            this.rpago = rpago;
        }
        [HttpGet]
        public ActionResult Index(int pagina = 1, string estado = "todos", int? dias = null)
        {
            try
            {
                var tamaño = 5;
                var listaCompleta = repositorio.ObtenerTodos(); // Cambiado para filtrar en memoria
                var fechaActual = DateTime.Now.Date;

                var listaFiltrada = estado switch
                {
                    // Un contrato está vigente si está activo Y su fecha de finalización real es futura.
                    "vigentes" => listaCompleta.Where(c => 
                        c.Estado && 
                        (c.FechaTerminacionEfectiva ?? c.FechaFin) >= fechaActual).ToList(),

                    // Un contrato no está vigente si está inactivo O su fecha de finalización real ya pasó.
                    "no-vigentes" => listaCompleta.Where(c => 
                        !c.Estado || 
                        (c.FechaTerminacionEfectiva ?? c.FechaFin) < fechaActual).ToList(),

                    // Próximo a vencer: Debe estar ACTIVO y su fecha de finalización ORIGINAL está dentro del plazo.
                    "proximos-a-vencer" when dias.HasValue => listaCompleta.Where(c =>
                        c.Estado &&
                        !c.FechaTerminacionEfectiva.HasValue &&
                        c.FechaFin >= fechaActual &&
                        (c.FechaFin - fechaActual).TotalDays <= dias.Value).ToList(),
                    
                    _ => listaCompleta.ToList()
                };

                var totalFiltrado = listaFiltrada.Count();
                var listaPaginada = listaFiltrada.Skip((pagina - 1) * tamaño).Take(tamaño).ToList();
                ViewBag.TotalPaginas = (int)Math.Ceiling(totalFiltrado / (double)tamaño);


                foreach (var contrato in listaPaginada)
                {
                    contrato.Inquilino = rinq.ObtenerPorId(contrato.idInquilino);
                    contrato.Inmueble = rinm.ObtenerPorId(contrato.idInmueble);
                }

                ViewBag.EstadoActual = estado;
                ViewBag.Dias = dias;
                ViewBag.Pagina = pagina;

                ViewBag.id = TempData["id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(listaPaginada);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al obtener la lista de contratos: {ex.Message}");
                return View();
            }
        }
        [HttpGet]
        public IActionResult Editar(int id)
        {
            ViewBag.Inquilino = rinq.ObtenerTodos();
            ViewBag.Inmueble = rinm.ObtenerTodos();
            if (id > 0)
            {
                var contrato = repositorio.ObtenerPorId(id);
                return View(contrato);
            }
            else
            {
                return View();
            }
        }
        public IActionResult Detalle(int id)
        {
            ViewBag.Inquilino = rinq.ObtenerTodos();
            ViewBag.Inmueble = rinm.ObtenerTodos();
            if (id > 0)
            {
                var contrato = repositorio.ObtenerPorId(id);
                return View(contrato);
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public IActionResult Guardar(Contrato contrato)
        {
            if (!ModelState.IsValid)
            {
                return View(contrato);
            }
            bool superposicion = repositorio.ControlSuperPosicion(contrato.idInmueble, contrato.FechaInicio,
                                            contrato.FechaFin, contrato.id > 0 ? contrato.id : null);
            if (superposicion)
            {
                ModelState.AddModelError("", "Ya existe un contrato para este inmueble que se superpone en las fechas seleccionadas.");
                ViewBag.Inquilino = rinq.ObtenerTodos();
                ViewBag.Inmueble = rinm.ObtenerTodos();
                return View("Editar", contrato);
            }

            RepositorioAuditoria ra = new RepositorioAuditoria(configuration);
            try
            {
                // Si es una renovación (viene de la vista Renovar)
                if (TempData["EsRenovacion"] != null)
                {
                    // Obtener el contrato anterior
                    var contratoAnterior = repositorio.ObtenerPorId(Convert.ToInt32(TempData["ContratoAnteriorId"]));
                    if (contratoAnterior != null)
                    {

                        // Desactivar el contrato anterior
                        contratoAnterior.Estado = false;
                        repositorio.Modificacion(contratoAnterior);
                        ra.NuevaAuditoria("Finalización Contrato", User.Identity?.Name ?? "Sistema",
                            $"Contrato ID: {contratoAnterior.id} finalizado por renovación");
                    }

                    // Crear el nuevo contrato
                    repositorio.Alta(contrato);
                    ra.NuevaAuditoria("Renovación Contrato", User.Identity?.Name ?? "Sistema",
                        $"Nuevo Contrato ID: {contrato.id} (renovación del contrato {contratoAnterior?.id})");
                    TempData["SuccessMessage"] = "Contrato renovado correctamente.";
                }
                else
                {
                    if (contrato.id > 0)
                    {
                        repositorio.Modificacion(contrato);
                        ra.NuevaAuditoria("Modificación Contrato", User.Identity?.Name ?? "Sistema", $"Contrato ID: {contrato.id}");
                        TempData["SuccessMessage"] = "Contrato actualizado correctamente.";
                    }
                    else
                    {
                        repositorio.Alta(contrato);
                        ra.NuevaAuditoria("Creación Contrato", User.Identity?.Name ?? "Sistema", $"Contrato ID: {contrato.id}");
                        TempData["SuccessMessage"] = "Contrato creado correctamente.";
                    }

                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al procesar el contrato.";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public IActionResult Eliminar(int id)
        {
            try
            {
                repositorio.Baja(id);
                TempData["SuccessMessage"] = "Contrato eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = "No se pudo eliminar el contrato";
                Console.WriteLine(ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult ListaPagos(int id)
        {
            try
            {
                var contrato = repositorio.ObtenerPorId(id);
                if (contrato == null)
                {
                    TempData["ErrorMessage"] = "Contrato no encontrado.";
                    Console.WriteLine("No contrato");
                    return RedirectToAction(nameof(Index));
                }
                var pagos = rpago.PagosPorContrato(id);
                ViewBag.Contrato = contrato;
                if (pagos == null || !pagos.Any())
                {
                    ViewData["InfoMessage"] = "No se encontraron pagos para ese contrato.";
                    Console.WriteLine("No hay pagos");
                }
                return View(pagos);

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los pagos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }

        }
        public IActionResult Renovar(int id)
        {
            try
            {
                if (id > 0)
                {
                    var contrato = repositorio.ObtenerPorId(id);
                    ViewBag.Inquilino = rinq.ObtenerPorId(contrato.idInquilino);
                    ViewBag.Inmueble = rinm.ObtenerPorId(contrato.idInmueble);
                    TempData["EsRenovacion"] = true;
                    TempData["ContratoAnteriorId"] = id;
                    var newContrato = new Contrato
                    {
                        id = contrato.id,
                        idInquilino = contrato.idInquilino,
                        idInmueble = contrato.idInmueble,
                        Monto = contrato.Monto,
                        FechaInicio = contrato.FechaFin,
                        FechaFin = contrato.FechaFin.AddMonths(1),
                        Estado = contrato.Estado
                    };
                    return View(newContrato);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el contrato: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult FinalizarContrato(int id)
        {
            try
            {
                var contrato = repositorio.ObtenerPorId(id);
                if (contrato == null)
                {
                    TempData["ErrorMessage"] = "Contrato no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // --- Lógica de Cálculo de Deuda y Multa ---

                // 1. Calcular meses adeudados
                var pagosRealizados = rpago.PagosPorContrato(id).Count();
                var mesesTranscurridos = ((DateTime.Now.Year - contrato.FechaInicio.Year) * 12) + DateTime.Now.Month - contrato.FechaInicio.Month;
                if (DateTime.Now.Day < contrato.FechaInicio.Day) mesesTranscurridos--;
                var mesesAdeudados = Math.Max(0, mesesTranscurridos - pagosRealizados);
                var montoDeuda = mesesAdeudados * contrato.Monto;

                // 2. Calcular la multa
                decimal montoMulta = 0;
                var duracionTotalContrato = (contrato.FechaFin - contrato.FechaInicio).TotalDays;
                var tiempoTranscurrido = (DateTime.Now - contrato.FechaInicio).TotalDays;

                if (tiempoTranscurrido < duracionTotalContrato / 2)
                {
                    montoMulta = contrato.Monto * 2;
                }
                else
                {
                    montoMulta = contrato.Monto * 1;
                }

                // 3. Pasar todos los datos a la vista
                ViewBag.MesesAdeudados = mesesAdeudados;
                ViewBag.MontoDeuda = montoDeuda;
                ViewBag.MontoMulta = montoMulta;
                ViewBag.ContratoOriginal = contrato;

                var contratoParaVista = new Contrato
                {
                    id = contrato.id,
                    FechaTerminacionEfectiva = DateTime.Now,
                    FechaInicio = contrato.FechaInicio,
                    FechaFin = contrato.FechaFin,
                    idInquilino = contrato.idInquilino,
                    idInmueble = contrato.idInmueble,
                    Monto = contrato.Monto,
                };

                return View(contratoParaVista);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar el contrato: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarFinalizacion(Contrato contrato, bool generarPagoMulta = false)
        {
            try
            {
                // Validar que el contrato a finalizar existe
                var contratoOriginal = repositorio.ObtenerPorId(contrato.id);
                if (contratoOriginal == null)
                {
                    TempData["ErrorMessage"] = "No se pudo encontrar el contrato original para finalizarlo.";
                    return RedirectToAction(nameof(Index));
                }

                // Actualizar el contrato con la fecha de terminación y desactivarlo
                contratoOriginal.FechaTerminacionEfectiva = contrato.FechaTerminacionEfectiva;
                contratoOriginal.Estado = false;
                repositorio.Modificacion(contratoOriginal);

                // Si el usuario marcó el checkbox, generar el pago de la multa
                if (generarPagoMulta)
                {
                    RepositorioAuditoria ra = new RepositorioAuditoria(configuration);
                    // Recalculamos la multa para asegurar consistencia
                    decimal montoMulta = 0;
                    var duracionTotalContrato = (contratoOriginal.FechaFin - contratoOriginal.FechaInicio).TotalDays;
                    var tiempoTranscurrido = (contrato.FechaTerminacionEfectiva.Value - contratoOriginal.FechaInicio).TotalDays;

                    if (tiempoTranscurrido < duracionTotalContrato / 2)
                    {
                        montoMulta = contratoOriginal.Monto * 2;
                    }
                    else
                    {
                        montoMulta = contratoOriginal.Monto * 1;
                    }

                    var pagoMulta = new Pago
                    {
                        IdContrato = contrato.id,
                        FechaPago = DateTime.Now,
                        Monto = montoMulta,
                        Detalle = "Pago de multa por terminación anticipada",
                        Estado = "Pagado",
                        Multa = true
                    };
                    rpago.Alta(pagoMulta);
                    ra.NuevaAuditoria("Finalización Contrato", User.Identity?.Name ?? "Sistema",
                            $"Contrato ID: {contratoOriginal.id} finalizado anticipadamente");
                }

                TempData["SuccessMessage"] = "El contrato ha sido finalizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al finalizar el contrato: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}