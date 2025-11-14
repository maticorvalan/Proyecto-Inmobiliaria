namespace Proyecto_Inmobiliaria.Models
{
    public class Pago
    {
        public int id { get; set; }
        public int IdContrato { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string? Estado { get; set; }
        public string? Detalle { get; set; }
        public bool Multa { get; set; }
        public Contrato? Contrato { get; set; }

         public override string ToString()
        {
            return $"Pago: {id} Contrato: {IdContrato} /MONTO: {Monto} ";
        }
    }
}