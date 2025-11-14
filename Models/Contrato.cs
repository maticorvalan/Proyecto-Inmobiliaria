using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Proyecto_Inmobiliaria.Models
{
    public class Contrato
    {
        [Key]
        [Display(Name = "Codigo")]
        public int id { get; set; }
        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
        [Display(Name = "Fecha de Terminación Efectiva")]
        public DateTime? FechaTerminacionEfectiva { get; set; }
        [Required]
        public decimal Monto { get; set; }
        public int idInquilino { get; set; }
        public int idInmueble { get; set; }
        public bool Estado { get; set; }
        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }

         public override string ToString()
        {
            return $"Contrato: {id} Inquilino: {Inquilino} Inmueble: {Inmueble} /MONTO: {Monto} ";
        }
    }

    
}