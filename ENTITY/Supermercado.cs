using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Supermercado
    {
        public int IdSupermercado { get; set; }
        public string Nombre { get; set; }
        public string Ciudad { get; set; }
        public int Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
