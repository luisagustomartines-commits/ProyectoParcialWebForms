using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Datos
{
    /// <summary>Consultas de resumen para los reportes.</summary>
    public class ReporteRepository
    {
        public ResumenGeneralDto ObtenerResumen()
        {
            const string sql =
                "SELECT " +
                "(SELECT COUNT(*) FROM dbo.Actividades WHERE Estado <> N'Inactiva') AS TotalActividades, " +
                "(SELECT COUNT(*) FROM dbo.Actividades WHERE Estado = N'Activa') AS Activas, " +
                "(SELECT COUNT(*) FROM dbo.Actividades WHERE Estado = N'Próxima') AS Proximas, " +
                "(SELECT COUNT(*) FROM dbo.Actividades WHERE Estado = N'Finalizada') AS Finalizadas, " +
                "(SELECT COUNT(*) FROM dbo.Registros) AS Participantes, " +
                "(SELECT COUNT(*) FROM dbo.Inscripciones WHERE Estado = N'Activa') AS Inscripciones, " +
                "(SELECT COUNT(*) FROM dbo.Resenas) AS Resenas, " +
                "(SELECT ISNULL(AVG(CAST(Calificacion AS FLOAT)), 0) FROM dbo.Resenas) AS Promedio;";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        dr.Read();
                        return new ResumenGeneralDto
                        {
                            TotalActividades = Convert.ToInt32(dr["TotalActividades"]),
                            ActividadesActivas = Convert.ToInt32(dr["Activas"]),
                            ActividadesProximas = Convert.ToInt32(dr["Proximas"]),
                            ActividadesFinalizadas = Convert.ToInt32(dr["Finalizadas"]),
                            TotalParticipantes = Convert.ToInt32(dr["Participantes"]),
                            InscripcionesActivas = Convert.ToInt32(dr["Inscripciones"]),
                            TotalResenas = Convert.ToInt32(dr["Resenas"]),
                            PromedioGeneral = Convert.ToDouble(dr["Promedio"])
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo generar el resumen.", ex);
            }
        }

        public List<ReporteActividadDto> ObtenerPorActividad()
        {
            const string sql =
                "SELECT a.ActividadId, a.Titulo, t.Nombre AS Tipo, a.Estado, a.Cupo, " +
                "(SELECT COUNT(*) FROM dbo.Inscripciones i WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') AS Inscritos, " +
                "(SELECT COUNT(*) FROM dbo.Resenas r WHERE r.ActividadId = a.ActividadId) AS TotalResenas, " +
                "(SELECT ISNULL(AVG(CAST(r.Calificacion AS FLOAT)), 0) FROM dbo.Resenas r WHERE r.ActividadId = a.ActividadId) AS Promedio " +
                "FROM dbo.Actividades a INNER JOIN dbo.TiposActividad t ON t.TipoId = a.TipoId " +
                "WHERE a.Estado <> N'Inactiva' ORDER BY a.FechaInicio DESC;";

            List<ReporteActividadDto> lista = new List<ReporteActividadDto>();

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
                            lista.Add(new ReporteActividadDto
                            {
                                ActividadId = Convert.ToInt32(dr["ActividadId"]),
                                Titulo = Convert.ToString(dr["Titulo"]),
                                Tipo = Convert.ToString(dr["Tipo"]),
                                Estado = Convert.ToString(dr["Estado"]),
                                Cupo = Convert.ToInt32(dr["Cupo"]),
                                Inscritos = Convert.ToInt32(dr["Inscritos"]),
                                TotalResenas = Convert.ToInt32(dr["TotalResenas"]),
                                Promedio = Convert.ToDouble(dr["Promedio"])
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo generar el reporte por actividad.", ex);
            }

            return lista;
        }
    }
}
