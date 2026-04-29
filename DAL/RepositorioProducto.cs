using ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class RepositorioProducto
    {
        public List<Producto> ObtenerPorCategoria(string categoria)
        {
            var todos = ObtenerTodos();
            return todos.FindAll(p => p.Categoria.ToLower() == categoria.ToLower());
        }

        public Producto ObtenerPorComando(string comando)
        {
            return ObtenerTodos().Find(p => p.Comando == comando);
        }

        private List<Producto> ObtenerTodos()
        {
            return new List<Producto>
            {
                // Granos
                new Producto { Id=1, Nombre="Arroz",    Categoria="granos",  Unidad="kg",    Emoji="🌾", Comando="/arroz" },
                new Producto { Id=2, Nombre="Maíz",     Categoria="granos",  Unidad="kg",    Emoji="🌽", Comando="/maiz" },
                new Producto { Id=3, Nombre="Frijoles", Categoria="granos",  Unidad="kg",    Emoji="🫘", Comando="/frijoles" },
                new Producto { Id=4, Nombre="Lentejas", Categoria="granos",  Unidad="kg",    Emoji="🫘", Comando="/lentejas" },
                new Producto { Id=5, Nombre="Pasta",    Categoria="granos",  Unidad="kg",    Emoji="🍝", Comando="/pasta" },
                // Aceites
                new Producto { Id=6, Nombre="Aceite Vegetal", Categoria="aceites", Unidad="litro", Emoji="🫙", Comando="/aceitevegetal" },
                new Producto { Id=7, Nombre="Aceite de Maíz", Categoria="aceites", Unidad="litro", Emoji="🌽", Comando="/aceitemaiz" },
                new Producto { Id=8, Nombre="Margarina",      Categoria="aceites", Unidad="kg",    Emoji="🧈", Comando="/margarina" },
                new Producto { Id=9, Nombre="Mantequilla",    Categoria="aceites", Unidad="kg",    Emoji="🧈", Comando="/mantequilla" },
                // Carnes
                new Producto { Id=10, Nombre="Pollo",    Categoria="carnes", Unidad="kg", Emoji="🍗", Comando="/pollo" },
                new Producto { Id=11, Nombre="Res",      Categoria="carnes", Unidad="kg", Emoji="🥩", Comando="/res" },
                new Producto { Id=12, Nombre="Cerdo",    Categoria="carnes", Unidad="kg", Emoji="🥓", Comando="/cerdo" },
                new Producto { Id=13, Nombre="Pescado",  Categoria="carnes", Unidad="kg", Emoji="🐟", Comando="/pescado" },
                new Producto { Id=14, Nombre="Mortadela",Categoria="carnes", Unidad="kg", Emoji="🌭", Comando="/mortadela" },
            };
        }
    }
}
