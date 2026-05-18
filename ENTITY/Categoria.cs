using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string Emoji { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
