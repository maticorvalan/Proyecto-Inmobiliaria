using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Proyecto_Inmobiliaria.Models
{
	[Table("Inmuebles")]
	public class Inmueble
	{
		[Key]
		[Display(Name = "Nº")]
		public int Id { get; set; }
		//[Required]
		[Display(Name = "Dirección")]
		[Required(ErrorMessage = "Debe agregar una direccion")]
		public string Direccion { get; set; }
		[Required(ErrorMessage = "Debe agregar una cantidad de ambientes")]
		[Range(1, 10, ErrorMessage = "La cantidad de ambientes debe estar entre 1 y 10")]

		public int Ambientes { get; set; }
        [Required(ErrorMessage = "Debe agregar un uso")]
        public string Uso { get; set; }
        [Required(ErrorMessage = "Debe agregar un tipo")]
        public string Tipo { get; set; }
        [Required(ErrorMessage = "Debe agregar una superficie")]
		[Range(1, 1000, ErrorMessage = "La superficie debe estar entre 1 y 1000")]
		public int Superficie { get; set; }
		[Required(ErrorMessage = "Debe agregar un precio")]
		public int Precio { get; set; }

		public decimal? Latitud { get; set; }
		public decimal? Longitud { get; set; }
		[Display(Name = "Dueño")]
		[Required(ErrorMessage = "Debe agregar un propietario")]
		public int IdPropietario { get; set; }
		[ForeignKey(nameof(IdPropietario))]
		public Propietario? Propietario { get; set; }
		public string? UrlPortada { get; set; }
		[NotMapped]//Para EF
		public IFormFile? PortadaFile { get; set; }
		[ForeignKey(nameof(Imagen.InmuebleId))]
		public IList<Imagen> Imagenes { get; set; } = new List<Imagen>();
		[Required(ErrorMessage = "Debe agregar un estado")]
		public string Estado { get; set; } 
		

		public override string ToString()
    {
        return $"{Id} {Direccion} | {Propietario?.Nombre} {Propietario?.Apellido}";
    }
	}
	
}
