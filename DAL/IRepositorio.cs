using System.Collections.Generic;

namespace DAL
{
    public interface IRepositorio<T>
    {
        List<T> ObtenerTodos();
    }
}
