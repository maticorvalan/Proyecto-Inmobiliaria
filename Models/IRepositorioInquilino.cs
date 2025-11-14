using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria.Models
{
	public interface IRepositorioInquilino : IRepositorio<Inquilino>
	{
		Inquilino ObtenerPorEmail(string email);
		IList<Inquilino> ObtenerLista(int paginaNro, int tamPagina);
	}
}