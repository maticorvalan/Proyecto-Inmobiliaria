using System.ComponentModel.DataAnnotations;

namespace Proyecto_Inmobiliaria.Models
{
    public class Usuario
    {
        public enum enRoles
	{
		Administrador = 1,
		Empleado = 2,
	}
        [Key]
		[Display(Name = "Código")]
		public int id { get; set; }
		[Required(ErrorMessage = "Debe agregar un nombre")]
        [RegularExpression("^[a-zA-ZÁÉÍÓÚáéíóúÑñ\\s]+$", 
            ErrorMessage = "El Nombre solo puede contener letras y espacios")]
		public string? Nombre { get; set; }
		[Required(ErrorMessage = "Debe agregar un apellido")]
        [RegularExpression("^[a-zA-ZÁÉÍÓÚáéíóúÑñ\\s]+$", 
            ErrorMessage = "El Apellido solo puede contener letras y espacios")]
		public string? Apellido { get; set; }
		[Required(ErrorMessage = "Debe agregar un email"),
        EmailAddress(ErrorMessage = "Formato de correo inválido")]
		public string? Email { get; set; }
		[Required(ErrorMessage ="Debe agregar una nueva clave"), DataType(DataType.Password)]
		public string? Clave { get; set; }
		public string? Avatar { get; set; } = "";
		public IFormFile? AvatarFile { get; set; }
		[Required(ErrorMessage = "Debe agregar un rol")]
		public int Rol { get; set; }
		public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

		public static IDictionary<int, string> ObtenerRoles()
		{
			SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
			Type tipoEnumRol = typeof(enRoles);
			foreach (var valor in Enum.GetValues(tipoEnumRol))
			{
				roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor) ?? string.Empty);
			}
			return roles;
		}
    }
}