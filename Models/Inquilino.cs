using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria.Models
{
    public class Inquilino
    {
        [Key]
        [Display(Name = "Código")]
        public int id { get; set; }
        [Required(ErrorMessage = "Debe agregar un nombre")]
        [RegularExpression("^[a-zA-ZÁÉÍÓÚáéíóúÑñ\\s]+$", 
            ErrorMessage = "El Nombre solo puede contener letras y espacios")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe agregar un apellido")]
        [RegularExpression("^[a-zA-ZÁÉÍÓÚáéíóúÑñ\\s]+$", 
            ErrorMessage = "El Apellido solo puede contener letras y espacios")]
        public string Apellido { get; set; } = string.Empty;
        [Required(ErrorMessage = "Debe agregar un DNI")]
        [Range(10000000, 99999999, ErrorMessage = "El DNI debe tener exactamente 8 dígitos")]
        public int DNI { get; set; }
        [Required(ErrorMessage = "Debe agregar un telefono")]
        public string Telefono { get; set; } = string.Empty;
        [Required(ErrorMessage = "Debe agregar un email"),
        EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public bool Estado { get; set; }

        public override string ToString()
        {
            return $"{Nombre} {Apellido} {DNI}";
        }
        
	}
}
