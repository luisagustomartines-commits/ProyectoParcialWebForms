using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Datos
{
    /// <summary>Acceso a la tabla Resenas.</summary>
    public class ResenaRepository
    {
        public int Insertar(ResenaDto r)
        {
            const string sql =
                "INSERT INTO dbo.Resenas(ActividadId, RegistroId, Calificacion, Comentario) " +
                "VALUES(@actividadId, @registroId, @calificacion, @comentario); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@actividadId", SqlDbType.Int).Value = r.ActividadId;
                    cmd.Parameters.Add("@registroId", SqlDbType.Int).Value = r.RegistroId;
                    cmd.Parameters.Add("@calificacion", SqlDbType.TinyInt).Value = (byte)r.Calificacion;
                    cmd.Parameters.Add("@comentario", SqlDbType.NVarChar, 1000).Value = r.Comentario ?? string.Empty;
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    throw new ResenaDuplicadaException();
                }
                throw new AccesoDatosException("No se pudo guardar la reseña.", ex);
            }
        }

        public List<ResenaDto> ListarPorActividad(int actividadId)
        {
            const string sql =
                "SELECT r.ResenaId, r.ActividadId, r.RegistroId, (p.Nombres + N' ' + p.Apellidos) AS NombreParticipante, " +
                "r.Calificacion, r.Comentario, r.Fecha " +
                "FROM dbo.Resenas r INNER JOIN dbo.Registros p ON p.Id = r.RegistroId " +
                "WHERE r.ActividadId = @actividadId ORDER BY r.Fecha DESC, r.ResenaId DESC;";

            List<ResenaDto> lista = new List<ResenaDto>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@actividadId", SqlDbType.Int).Value = actividadId;
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new ResenaDto
                            {
                                ResenaId = Convert.ToInt32(dr["ResenaId"]),
                                ActividadId = Convert.ToInt32(dr["ActividadId"]),
                                RegistroId = Convert.ToInt32(dr["RegistroId"]),
                                NombreParticipante = Convert.ToString(dr["NombreParticipante"]),
                                Calificacion = Convert.ToInt32(dr["Calificacion"]),
                                Comentario = Convert.ToString(dr["Comentario"]),
                                Fecha = Convert.ToDateTime(dr["Fecha"])
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar las reseñas.", ex);
            }

            return lista;
        }
    }
}
