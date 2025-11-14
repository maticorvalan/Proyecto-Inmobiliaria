using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria.Models
{
	public interface IRepositorioImagen : IRepositorio<Imagen>
	{
		IList<Imagen> BuscarPorInmueble(int inmuebleId);
		
	}
}
