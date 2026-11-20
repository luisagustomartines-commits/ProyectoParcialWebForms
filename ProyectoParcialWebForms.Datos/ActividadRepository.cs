using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Datos
{
    /// <summary>
    /// Repositorio de actividades académicas (tabla Actividades).
    /// Todas las consultas son parametrizadas; ningún dato del usuario se concatena en el SQL.
    /// </summary>
    public class ActividadRepository
    {
        private const string SelectBase =
            "SELECT a.ActividadId, a.Titulo, a.Descripcion, a.TipoId, t.Nombre AS TipoNombre, " +
            "a.FechaInicio, a.FechaFin, a.HoraInicio, a.HoraFin, a.Modalidad, a.Lugar, a.Cupo, a.Estado, " +
            "(SELECT COUNT(*) FROM dbo.Inscripciones i WHERE i.ActividadId = a.ActividadId AND i.Estado = N'Activa') AS Inscritos, " +
            "(SELECT COUNT(*) FROM dbo.Resenas r WHERE r.ActividadId = a.ActividadId) AS TotalResenas, " +
            "(SELECT ISNULL(AVG(CAST(r.Calificacion AS FLOAT)), 0) FROM dbo.Resenas r WHERE r.ActividadId = a.ActividadId) AS Promedio " +
            "FROM dbo.Actividades a INNER JOIN dbo.TiposActividad t ON t.TipoId = a.TipoId ";

        /// <summary>Inserta una actividad y devuelve su Id.</summary>
        public int Insertar(ActividadDto a)
        {
            const string sql =
                "INSERT INTO dbo.Actividades(Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupo, Estado) " +
                "VALUES(@titulo, @descripcion, @tipoId, @fechaInicio, @fechaFin, @horaInicio, @horaFin, @modalidad, @lugar, @cupo, @estado); " +
                "SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    AgregarParametros(cmd, a);
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                throw Traducir(ex, "No se pudo guardar la actividad.");
            }
        }

        /// <summary>Actualiza una actividad. Devuelve false si el Id no existe.</summary>
        public bool Actualizar(ActividadDto a)
        {
            const string sql =
                "UPDATE dbo.Actividades SET Titulo=@titulo, Descripcion=@descripcion, TipoId=@tipoId, " +
                "FechaInicio=@fechaInicio, FechaFin=@fechaFin, HoraInicio=@horaInicio, HoraFin=@horaFin, " +
                "Modalidad=@modalidad, Lugar=@lugar, Cupo=@cupo, Estado=@estado " +
                "WHERE ActividadId=@id;";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = a.ActividadId;
                    AgregarParametros(cmd, a);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                throw Traducir(ex, "No se pudo actualizar la actividad.");
            }
        }

        /// <summary>Desactiva (eliminación lógica). Devuelve false si no existe o ya estaba inactiva.</summary>
        public bool Desactivar(int actividadId)
        {
            const string sql =
                "UPDATE dbo.Actividades SET Estado = N'Inactiva' WHERE ActividadId = @id AND Estado <> N'Inactiva';";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = actividadId;
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo desactivar la actividad.", ex);
            }
        }

        /// <summary>Devuelve una actividad por Id, o null si no existe.</summary>
        public ActividadDto ObtenerPorId(int actividadId)
        {
            const string sql = SelectBase + "WHERE a.ActividadId = @id;";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = actividadId;
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        return dr.Read() ? Mapear(dr) : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar la actividad.", ex);
            }
        }

        /// <summary>Cantidad de inscripciones activas de una actividad.</summary>
        public int ContarInscritosActivos(int actividadId)
        {
            const string sql =
                "SELECT COUNT(*) FROM dbo.Inscripciones WHERE ActividadId = @id AND Estado = N'Activa';";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = actividadId;
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar las inscripciones de la actividad.", ex);
            }
        }

        /// <summary>
        /// Busca por texto (título o lugar), tipo (0 = todos), estado ("" = todos menos inactivas)
        /// y rango de fechas (la actividad se cruza con el rango indicado).
        /// </summary>
        public List<ActividadDto> Buscar(string texto, int tipoId, string estado, DateTime? desde, DateTime? hasta)
        {
            const string sql = SelectBase +
                "WHERE (@texto = N'' OR a.Titulo LIKE N'%' + @texto + N'%' OR a.Lugar LIKE N'%' + @texto + N'%') " +
                "AND (@tipoId = 0 OR a.TipoId = @tipoId) " +
                "AND (@estado = N'' OR a.Estado = @estado) " +
                "AND (@estado <> N'' OR a.Estado <> N'Inactiva') " +
                "AND (@desde IS NULL OR a.FechaFin >= @desde) " +
                "AND (@hasta IS NULL OR a.FechaInicio <= @hasta) " +
                "ORDER BY a.FechaInicio DESC, a.ActividadId DESC;";

            List<ActividadDto> lista = new List<ActividadDto>();

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@texto", SqlDbType.NVarChar, 120).Value = texto ?? string.Empty;
                    cmd.Parameters.Add("@tipoId", SqlDbType.Int).Value = tipoId;
                    cmd.Parameters.Add("@estado", SqlDbType.NVarChar, 20).Value = estado ?? string.Empty;
                    cmd.Parameters.Add("@desde", SqlDbType.Date).Value = desde.HasValue ? (object)desde.Value.Date : DBNull.Value;
                    cmd.Parameters.Add("@hasta", SqlDbType.Date).Value = hasta.HasValue ? (object)hasta.Value.Date : DBNull.Value;

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(Mapear(dr));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar la lista de actividades.", ex);
            }

            return lista;
        }

        // ------------------------------------------------------------
        private static void AgregarParametros(SqlCommand cmd, ActividadDto a)
        {
            cmd.Parameters.Add("@titulo", SqlDbType.NVarChar, 120).Value = a.Titulo;
            cmd.Parameters.Add("@descripcion", SqlDbType.NVarChar, 1000).Value = a.Descripcion;
            cmd.Parameters.Add("@tipoId", SqlDbType.Int).Value = a.TipoId;
            cmd.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = a.FechaInicio.Value.Date;
            cmd.Parameters.Add("@fechaFin", SqlDbType.Date).Value = a.FechaFin.Value.Date;
            cmd.Parameters.Add("@horaInicio", SqlDbType.Time).Value = a.HoraInicio.Value;
            cmd.Parameters.Add("@horaFin", SqlDbType.Time).Value = a.HoraFin.Value;
            cmd.Parameters.Add("@modalidad", SqlDbType.NVarChar, 20).Value = a.Modalidad;
            cmd.Parameters.Add("@lugar", SqlDbType.NVarChar, 150).Value = a.Lugar;
            cmd.Parameters.Add("@cupo", SqlDbType.Int).Value = a.Cupo;
            cmd.Parameters.Add("@estado", SqlDbType.NVarChar, 20).Value = a.Estado;
        }

        private static Exception Traducir(SqlException ex, string mensaje)
        {
            // 2627 / 2601 = violación de restricción UNIQUE (título + fecha de inicio)
            if (ex.Number == 2627 || ex.Number == 2601)
            {
                return new ActividadDuplicadaException();
            }
            return new AccesoDatosException(mensaje, ex);
        }

        private static ActividadDto Mapear(SqlDataReader dr)
        {
            return new ActividadDto
            {
                ActividadId = Convert.ToInt32(dr["ActividadId"]),
                Titulo = Convert.ToString(dr["Titulo"]),
                Descripcion = Convert.ToString(dr["Descripcion"]),
                TipoId = Convert.ToInt32(dr["TipoId"]),
                TipoNombre = Convert.ToString(dr["TipoNombre"]),
                FechaInicio = Convert.ToDateTime(dr["FechaInicio"]),
                FechaFin = Convert.ToDateTime(dr["FechaFin"]),
                HoraInicio = (TimeSpan)dr["HoraInicio"],
                HoraFin = (TimeSpan)dr["HoraFin"],
                Modalidad = Convert.ToString(dr["Modalidad"]),
                Lugar = Convert.ToString(dr["Lugar"]),
                Cupo = Convert.ToInt32(dr["Cupo"]),
                Estado = Convert.ToString(dr["Estado"]),
                Inscritos = Convert.ToInt32(dr["Inscritos"]),
                TotalResenas = Convert.ToInt32(dr["TotalResenas"]),
                PromedioCalificacion = Convert.ToDouble(dr["Promedio"])
            };
        }
    }
}
