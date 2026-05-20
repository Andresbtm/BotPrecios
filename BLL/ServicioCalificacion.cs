using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServicioCalificacion
    {
        private readonly RepositorioCalificacion _repositorio = new RepositorioCalificacion();

        public void Insertar(int puntaje, int idProducto, int idSupermercado, int idUsuario)
        {
            try
            {
                _repositorio.Insertar(puntaje, idProducto, idSupermercado, idUsuario);
            }
            catch (Exception ex)
            {
                // Relanzar con mensaje amigable para el bot
                throw new Exception(ex.Message.Contains("20013")
                    ? "Ya calificaste este producto en este supermercado."
                    : "Error al guardar la calificación.");
            }
        }

        public double ObtenerPromedio(int idProducto, int idSupermercado)
        {
            var calificaciones = _repositorio.ListarPorProductoSuper(idProducto, idSupermercado);
            if (calificaciones.Count == 0) return 0;

            int l_total = 0;
            foreach (var c in calificaciones)
                l_total += c.Puntaje;

            return Math.Round((double)l_total / calificaciones.Count, 1);
        }

        public int ObtenerTotalVotos(int idProducto, int idSupermercado)
        {
            return _repositorio.ListarPorProductoSuper(idProducto, idSupermercado).Count;
        }
    }
}
