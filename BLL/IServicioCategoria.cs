using ENTITY;
using System.Collections.Generic;

namespace BLL
{
    public interface IServicioCategoria
    {
        List<Categoria> ListarTodas();
        (string emoji, string titulo) ObtenerDatosCategoria(string nombreCategoria);
        string ObtenerNombrePorId(int idCategoria);
    }
}
