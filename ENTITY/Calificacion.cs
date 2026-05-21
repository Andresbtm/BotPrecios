using System;

namespace ENTITY
{
    public class Calificacion
    {
        public int IdCalificacion { get; set; }
        public int Puntaje { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int IdProducto { get; set; }
        public int IdSupermercado { get; set; }
        public int IdUsuario { get; set; }
    }
}
