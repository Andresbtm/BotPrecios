using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public long IdChat { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
