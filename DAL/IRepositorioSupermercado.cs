using ENTITY;
using System.Collections.Generic;

namespace DAL
{
    public interface IRepositorioSupermercado
    {
        void Insertar(string nombre, string ciudad);
        List<Supermercado> ListarTodos();
        List<Supermercado> ListarActivos();
        Supermercado BuscarPorId(int idSupermercado);
        void ActualizarActivo(int idSupermercado, int activo);
    }
}
