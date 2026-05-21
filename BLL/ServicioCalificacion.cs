using DAL;
using System;

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
            return _repositorio.ObtenerPromedio(idProducto, idSupermercado);
        }

        public int ObtenerTotalVotos(int idProducto, int idSupermercado)
        {
            return _repositorio.ObtenerTotalVotos(idProducto, idSupermercado);
        }
    }
}
