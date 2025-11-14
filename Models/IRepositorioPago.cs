using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria.Models
{
    public interface IRepositorioPago : IRepositorio<Pago>
    {

        void Multar(int idContrato, decimal monto);
        IList<Pago> ObtenerTodosPaginado(int paginaNro = 1, int tamPagina = 10);
        IList<Pago>? PagosPorContrato(int idContrato);
        IList<Pago>? PagosPendientes(int idContrato);
	}
}