using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ServicioPruebaConexion
    {
        private readonly PruebaConexion _prueba = new PruebaConexion();

        public string Probar()
        {
            return _prueba.Probar();
        }
    }
}
