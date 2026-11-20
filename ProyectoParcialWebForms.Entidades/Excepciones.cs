using System;

namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>Se incumplió una regla de negocio (dato inválido o faltante).</summary>
    public class ReglaNegocioException : Exception
    {
        public ReglaNegocioException(string mensaje) : base(mensaje) { }
    }

    /// <summary>La identificación ya existe en la base de datos.</summary>
    public class IdentificacionDuplicadaException : Exception
    {
        public IdentificacionDuplicadaException(string identificacion)
            : base("Ya existe un registro con la identificación " + identificacion + ".") { }
    }

    /// <summary>El registro que se intentó modificar ya no existe.</summary>
    public class RegistroNoEncontradoException : Exception
    {
        public RegistroNoEncontradoException()
            : base("El registro que intenta editar ya no existe.") { }
    }

    /// <summary>Ya existe una actividad con el mismo título y fecha de inicio.</summary>
    public class ActividadDuplicadaException : Exception
    {
        public ActividadDuplicadaException()
            : base("Ya existe una actividad con el mismo título y la misma fecha de inicio.") { }
    }

    /// <summary>La persona ya publicó una reseña para esa actividad.</summary>
    public class ResenaDuplicadaException : Exception
    {
        public ResenaDuplicadaException()
            : base("Ya publicó una reseña para esta actividad.") { }
    }

    /// <summary>Falla técnica al acceder a SQL Server (conexión, permisos, tabla inexistente...).</summary>
    public class AccesoDatosException : Exception
    {
        public AccesoDatosException(string mensaje, Exception interna = null) : base(mensaje, interna) { }
    }
}
