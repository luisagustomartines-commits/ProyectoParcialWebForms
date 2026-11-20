using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Datos
{
    /// <summary>Acceso a la tabla Inscripciones, incluida la inscripción transaccional con control de cupos.</summary>
    public class InscripcionRepository
    {
        /// <summary>
        /// Inscribe a una persona dentro de UNA transacción. La actividad se bloquea (UPDLOCK, HOLDLOCK)
        /// mientras se verifica el cupo, de modo que dos solicitudes simultáneas no puedan consumir
        /// el último cupo a la vez.
        /// </summary>
        public ResultadoInscripcion Inscribir(int actividadId, int registroId)
        {
            const string sql =
                "SET XACT_ABORT ON; " +
                "BEGIN TRANSACTION; " +
                "DECLARE @cupo INT, @estado NVARCHAR(20), @fin DATE, @inscrito NVARCHAR(20); " +
                "SELECT @cupo = Cupo, @estado = Estado, @fin = FechaFin " +
                "FROM dbo.Actividades WITH (UPDLOCK, HOLDLOCK) WHERE ActividadId = @actividadId; " +
                "IF @cupo IS NULL BEGIN ROLLBACK TRANSACTION; SELECT 1; RETURN; END " +
                "IF @estado NOT IN (N'Activa', N'Próxima') OR @fin < CAST(GETDATE() AS DATE) " +
                "BEGIN ROLLBACK TRANSACTION; SELECT 2; RETURN; END " +
                "SELECT @inscrito = Estado FROM dbo.Inscripciones WHERE ActividadId = @actividadId AND RegistroId = @registroId; " +
                "IF @inscrito = N'Activa' BEGIN ROLLBACK TRANSACTION; SELECT 3; RETURN; END " +
                "IF (SELECT COUNT(*) FROM dbo.Inscripciones WHERE ActividadId = @actividadId AND Estado = N'Activa') >= @cupo " +
                "BEGIN ROLLBACK TRANSACTION; SELECT 4; RETURN; END " +
                "IF @inscrito IS NULL " +
                "INSERT INTO dbo.Inscripciones(ActividadId, RegistroId) VALUES(@actividadId, @registroId); " +
                "ELSE " +
                "UPDATE dbo.Inscripciones SET Estado = N'Activa', FechaInscripcion = SYSDATETIME() " +
                "WHERE ActividadId = @actividadId AND RegistroId = @registroId; " +
                "COMMIT TRANSACTION; " +
                "SELECT 0;";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@actividadId", SqlDbType.Int).Value = actividadId;
                    cmd.Parameters.Add("@registroId", SqlDbType.Int).Value = registroId;

                    con.Open();
                    return (ResultadoInscripcion)Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo registrar la inscripción.", ex);
            }
        }

        /// <summary>Cancela una inscripción activa. Devuelve false si no existía una inscripción activa.</summary>
        public bool Cancelar(int actividadId, int registroId)
        {
            const string sql =
                "UPDATE dbo.Inscripciones SET Estado = N'Cancelada' " +
                "WHERE ActividadId = @actividadId AND RegistroId = @registroId AND Estado = N'Activa';";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@actividadId", SqlDbType.Int).Value = actividadId;
                    cmd.Parameters.Add("@registroId", SqlDbType.Int).Value = registroId;
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo cancelar la inscripción.", ex);
            }
        }

        public bool TieneInscripcionActiva(int actividadId, int registroId)
        {
            const string sql =
                "SELECT COUNT(1) FROM dbo.Inscripciones " +
                "WHERE ActividadId = @actividadId AND RegistroId = @registroId AND Estado = N'Activa';";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@actividadId", SqlDbType.Int).Value = actividadId;
                    cmd.Parameters.Add("@registroId", SqlDbType.Int).Value = registroId;
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar la inscripción.", ex);
            }
        }

        /// <summary>Inscripciones de una persona (activas y canceladas), la más reciente primero.</summary>
        public List<InscripcionDto> ListarPorParticipante(int registroId)
        {
            const string sql =
                "SELECT i.InscripcionId, i.ActividadId, a.Titulo, a.FechaInicio, a.FechaFin, a.Estado AS EstadoActividad, " +
                "i.Estado, i.FechaInscripcion, N'' AS NombreParticipante, N'' AS Programa " +
                "FROM dbo.Inscripciones i INNER JOIN dbo.Actividades a ON a.ActividadId = i.ActividadId " +
                "WHERE i.RegistroId = @registroId ORDER BY a.FechaInicio DESC;";

            return Leer(sql, "@registroId", registroId);
        }

        /// <summary>Personas inscritas (activas) en una actividad.</summary>
        public List<InscripcionDto> ListarPorActividad(int actividadId)
        {
            const string sql =
                "SELECT i.InscripcionId, i.ActividadId, a.Titulo, a.FechaInicio, a.FechaFin, a.Estado AS EstadoActividad, " +
                "i.Estado, i.FechaInscripcion, (p.Nombres + N' ' + p.Apellidos) AS NombreParticipante, p.Programa " +
                "FROM dbo.Inscripciones i " +
                "INNER JOIN dbo.Actividades a ON a.ActividadId = i.ActividadId " +
                "INNER JOIN dbo.Registros p ON p.Id = i.RegistroId " +
                "WHERE i.ActividadId = @actividadId AND i.Estado = N'Activa' ORDER BY i.FechaInscripcion;";

            return Leer(sql, "@actividadId", actividadId);
        }

        // ------------------------------------------------------------
        private static List<InscripcionDto> Leer(string sql, string parametro, int valor)
        {
            List<InscripcionDto> lista = new List<InscripcionDto>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add(parametro, SqlDbType.Int).Value = valor;
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new InscripcionDto
                            {
                                InscripcionId = Convert.ToInt32(dr["InscripcionId"]),
                                ActividadId = Convert.ToInt32(dr["ActividadId"]),
                                Titulo = Convert.ToString(dr["Titulo"]),
                                FechaInicio = Convert.ToDateTime(dr["FechaInicio"]),
                                FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                                EstadoActividad = Convert.ToString(dr["EstadoActividad"]),
                                Estado = Convert.ToString(dr["Estado"]),
                                FechaInscripcion = Convert.ToDateTime(dr["FechaInscripcion"]),
                                NombreParticipante = Convert.ToString(dr["NombreParticipante"]),
                                Programa = Convert.ToString(dr["Programa"])
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar las inscripciones.", ex);
            }

            return lista;
        }
    }
}
