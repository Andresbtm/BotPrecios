using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENTITY
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string Unidad { get; set; } // kg, litro, unidad
        public string Emoji { get; set; }
        public string Comando { get; set; }
    }
}
