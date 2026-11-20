using System;

namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>
    /// Objeto de transferencia de una actividad académica entre capas.
    /// No contiene controles de interfaz ni SQL. Las fechas y horas son nulables
    /// para distinguir "no se indicó" de un valor válido.
    /// </summary>
    public class ActividadDto
    {
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int TipoId { get; set; }
        public string TipoNombre { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public string Modalidad { get; set; }
        public string Lugar { get; set; }
        public int Cupo { get; set; }
        public string Estado { get; set; }

        // Datos calculados por la consulta (inscripciones y reseñas)
        public int Inscritos { get; set; }
        public int TotalResenas { get; set; }
        public double PromedioCalificacion { get; set; }

        public int CuposDisponibles
        {
            get { return Math.Max(0, Cupo - Inscritos); }
        }
    }
}
