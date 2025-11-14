namespace Proyecto_Inmobiliaria.Models
{
	public interface IRepositorioUsuario : IRepositorio<Usuario>
	{
		Usuario ObtenerPorEmail(string email);
	}
}