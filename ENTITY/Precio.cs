using System;

namespace ENTITY
{
    public class Precio
    {
        public int IdPrecio { get; set; }
        public decimal Valor { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Fuente { get; set; }
        public int IdProducto { get; set; }
        public int IdSupermercado { get; set; }
    }
}
