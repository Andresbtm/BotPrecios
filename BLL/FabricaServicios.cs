using DAL;

namespace BLL
{
    public class FabricaServicios
    {
        public static ServicioCategoria CrearServicioCategoria()
        {
            IConexion conexion = new Conexion();
            IRepositorioCategoria repoCategoria = new RepositorioCategoria(conexion);
            return new ServicioCategoria(repoCategoria);
        }

        public static ServicioProducto CrearServicioProducto()
        {
            IConexion conexion = new Conexion();

            IRepositorioProducto repoProducto = new RepositorioProducto(conexion);
            IRepositorioPrecio repoPrecio = new RepositorioPrecio(conexion);
            IRepositorioCalificacion repoCalificacion = new RepositorioCalificacion(conexion);
            IRepositorioCategoria repoCategoria = new RepositorioCategoria(conexion);

            IServicioPrecio servicioPrecio = new ServicioPrecio(repoPrecio);
            IServicioCalificacion servicioCalificacion = new ServicioCalificacion(repoCalificacion);
            IServicioCategoria servicioCategoria = new ServicioCategoria(repoCategoria);

            return new ServicioProducto(repoProducto, servicioPrecio, servicioCalificacion, servicioCategoria);
        }

        public static ServicioPrecio CrearServicioPrecio()
        {
            IConexion conexion = new Conexion();
            IRepositorioPrecio repoPrecio = new RepositorioPrecio(conexion);
            return new ServicioPrecio(repoPrecio);
        }

        public static ServicioUsuario CrearServicioUsuario()
        {
            IConexion conexion = new Conexion();
            IRepositorioUsuario repoUsuario = new RepositorioUsuario(conexion);
            return new ServicioUsuario(repoUsuario);
        }

        public static ServicioCalificacion CrearServicioCalificacion()
        {
            IConexion conexion = new Conexion();
            IRepositorioCalificacion repoCalificacion = new RepositorioCalificacion(conexion);
            return new ServicioCalificacion(repoCalificacion);
        }

        public static ServicioSupermercado CrearServicioSupermercado()
        {
            IConexion conexion = new Conexion();
            IRepositorioSupermercado repoSupermercado = new RepositorioSupermercado(conexion);
            return new ServicioSupermercado(repoSupermercado);
        }
    }
}
