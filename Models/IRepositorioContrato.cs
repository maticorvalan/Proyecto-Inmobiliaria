using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria.Models
{
    public interface IRepositorioContrato : IRepositorio<Contrato>
    {
        Contrato? ObtenerPorInmueble(int idInmueble);
        IList<Contrato> ObtenerTodosPaginado(int paginaNro = 1, int tamPagina = 10);
        bool ControlSuperPosicion(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? idContrato = null);
	}
}