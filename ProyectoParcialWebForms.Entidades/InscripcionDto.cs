using System;

namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>Inscripción de un participante a una actividad.</summary>
    public class InscripcionDto
    {
        public int InscripcionId { get; set; }
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string EstadoActividad { get; set; }
        public string Estado { get; set; }
        public DateTime FechaInscripcion { get; set; }
        public string NombreParticipante { get; set; }
        public string Programa { get; set; }
    }
}
