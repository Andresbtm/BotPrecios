using ENTITY;
using System.Collections.Generic;

namespace DAL
{
    public interface IRepositorioSupermercado : IRepositorio<Supermercado>
    {
        void Insertar(string nombre, string ciudad);
        List<Supermercado> ListarActivos();
        Supermercado BuscarPorId(int idSupermercado);
        void ActualizarActivo(int idSupermercado, int activo);
    }
}
