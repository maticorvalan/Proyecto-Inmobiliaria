using System;

namespace Proyecto_Inmobiliaria.Models
{
    public class Auditoria
    {
        public int id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string? Detalle { get; set; } 
    }
}