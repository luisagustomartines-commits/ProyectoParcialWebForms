using System.Collections.Generic;
using System.Data.SqlClient;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Datos
{
    /// <summary>Acceso a la tabla TiposActividad.</summary>
    public class TipoActividadRepository
    {
        public List<TipoActividadDto> Listar()
        {
            const string sql = "SELECT TipoId, Nombre FROM dbo.TiposActividad ORDER BY Nombre;";
            List<TipoActividadDto> lista = new List<TipoActividadDto>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new TipoActividadDto
                            {
                                TipoId = dr.GetInt32(0),
                                Nombre = dr.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudieron leer los tipos de actividad.", ex);
            }

            return lista;
        }
    }
}
