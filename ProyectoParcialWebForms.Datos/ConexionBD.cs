using System.Configuration;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Datos
{
    /// <summary>Lee la cadena de conexión "conexionDB" del Web.config de la aplicación.</summary>
    internal static class ConexionBD
    {
        public static string Cadena
        {
            get
            {
                ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings["conexionDB"];
                if (cs == null)
                {
                    throw new AccesoDatosException("No se encontró la cadena de conexión 'conexionDB' en Web.config.");
                }
                return cs.ConnectionString;
            }
        }
    }
}
