using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Datos
{
    /// <summary>
    /// Repositorio de participantes (tabla Registros). Solo esta capa conoce SQL Server y ADO.NET.
    /// Recibe y devuelve entidades; traduce los errores de SQL a excepciones propias.
    /// </summary>
    public class RegistroRepository
    {
        private const string Columnas =
            "Id, Codigo, Identificacion, Nombres, Apellidos, Correo, Telefono, Programa, " +
            "Semestre, Participacion, Jornada, Temas, Comentarios, FechaRegistro, Rol";

        /// <summary>Inserta un registro y devuelve el código generado (EVT-AAAA-####).</summary>
        public string Insertar(Registro r)
        {
            const string sql =
                "INSERT INTO Registros(Identificacion, Nombres, Apellidos, Correo, Telefono, Programa, Semestre, Participacion, Jornada, Temas, Comentarios) " +
                "VALUES(@identificacion, @nombres, @apellidos, @correo, @telefono, @programa, @semestre, @participacion, @jornada, @temas, @comentarios); " +
                "DECLARE @id INT = CAST(SCOPE_IDENTITY() AS INT); " +
                "UPDATE Registros SET Codigo = 'EVT-' + CAST(YEAR(GETDATE()) AS VARCHAR(4)) + '-' + RIGHT('0000' + CAST(@id AS VARCHAR(10)), 4) WHERE Id = @id; " +
                "SELECT Codigo FROM Registros WHERE Id = @id;";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@identificacion", SqlDbType.VarChar, 12).Value = r.Identificacion;
                    AgregarParametros(cmd, r);

                    con.Open();
                    return Convert.ToString(cmd.ExecuteScalar());
                }
            }
            catch (SqlException ex)
            {
                // 2627 / 2601 = violación de restricción UNIQUE
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    throw new IdentificacionDuplicadaException(r.Identificacion);
                }
                throw new AccesoDatosException("No se pudo guardar el registro en la base de datos.", ex);
            }
        }

        /// <summary>Actualiza el registro por Id. Devuelve su código, o null si el Id no existe.</summary>
        public string Actualizar(Registro r)
        {
            const string sql =
                "UPDATE Registros SET Nombres=@nombres, Apellidos=@apellidos, Correo=@correo, Telefono=@telefono, " +
                "Programa=@programa, Semestre=@semestre, Participacion=@participacion, Jornada=@jornada, " +
                "Temas=@temas, Comentarios=@comentarios WHERE Id=@id; " +
                "SELECT Codigo FROM Registros WHERE Id=@id;";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = r.Id;
                    AgregarParametros(cmd, r);

                    con.Open();
                    object resultado = cmd.ExecuteScalar();
                    return resultado == null ? null : Convert.ToString(resultado);
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo actualizar el registro en la base de datos.", ex);
            }
        }

        /// <summary>Busca un registro por identificación. Devuelve null si no existe.</summary>
        public Registro ObtenerPorIdentificacion(string identificacion)
        {
            const string sql = "SELECT " + Columnas + " FROM Registros WHERE Identificacion = @identificacion";

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cadena))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.Add("@identificacion", SqlDbType.VarChar, 12).Value = identificacion;

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        return dr.Read() ? Mapear(dr) : null;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo consultar la base de datos.", ex);
            }
        }

        /// <summary>Devuelve todos los registros, del más reciente al más antiguo.</summary>
        public List<Registro> Listar()
        {
            const string sql = "SELECT " + Columnas + " FROM Registros ORDER BY Id DESC";
            List<Registro> lista = new List<Registro>();

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
                            lista.Add(Mapear(dr));
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new AccesoDatosException("No se pudo leer la lista de registros.", ex);
            }

            return lista;
        }

        // ------------------------------------------------------------
        private static void AgregarParametros(SqlCommand cmd, Registro r)
        {
            cmd.Parameters.Add("@nombres", SqlDbType.VarChar, 80).Value = r.Nombres;
            cmd.Parameters.Add("@apellidos", SqlDbType.VarChar, 80).Value = r.Apellidos;
            cmd.Parameters.Add("@correo", SqlDbType.VarChar, 120).Value = r.Correo;
            cmd.Parameters.Add("@telefono", SqlDbType.VarChar, 10).Value = r.Telefono;
            cmd.Parameters.Add("@programa", SqlDbType.VarChar, 100).Value = r.Programa;
            cmd.Parameters.Add("@semestre", SqlDbType.VarChar, 10).Value = r.Semestre;
            cmd.Parameters.Add("@participacion", SqlDbType.VarChar, 30).Value = r.Participacion;
            cmd.Parameters.Add("@jornada", SqlDbType.VarChar, 20).Value = r.Jornada;
            cmd.Parameters.Add("@temas", SqlDbType.VarChar, 500).Value = r.Temas;
            cmd.Parameters.Add("@comentarios", SqlDbType.VarChar, 400).Value = r.Comentarios ?? string.Empty;
        }

        private static Registro Mapear(SqlDataReader dr)
        {
            return new Registro
            {
                Id = Convert.ToInt32(dr["Id"]),
                Codigo = Convert.ToString(dr["Codigo"]),
                Identificacion = Convert.ToString(dr["Identificacion"]),
                Nombres = Convert.ToString(dr["Nombres"]),
                Apellidos = Convert.ToString(dr["Apellidos"]),
                Correo = Convert.ToString(dr["Correo"]),
                Telefono = Convert.ToString(dr["Telefono"]),
                Programa = Convert.ToString(dr["Programa"]),
                Semestre = Convert.ToString(dr["Semestre"]),
                Participacion = Convert.ToString(dr["Participacion"]),
                Jornada = Convert.ToString(dr["Jornada"]),
                Temas = Convert.ToString(dr["Temas"]),
                Comentarios = Convert.ToString(dr["Comentarios"]),
                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                Rol = Convert.ToString(dr["Rol"])
            };
        }
    }
}
