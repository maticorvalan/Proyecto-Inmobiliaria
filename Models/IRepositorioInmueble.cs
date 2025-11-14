using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria.Models
{
	public interface IRepositorioInmueble : IRepositorio<Inmueble>
	{
		int ModificarPortada(int InmuebleId, string ruta);
		IList<Inmueble> BuscarPorPropietario(int idPropietario);
		IList<Inmueble> ObtenerDisponiblesPorFecha(DateTime fechaInicio, DateTime fechaFin);
	}
}