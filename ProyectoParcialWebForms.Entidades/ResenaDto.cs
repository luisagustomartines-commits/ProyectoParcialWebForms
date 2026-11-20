using System;

namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>Reseña o valoración de una actividad.</summary>
    public class ResenaDto
    {
        public int ResenaId { get; set; }
        public int ActividadId { get; set; }
        public int RegistroId { get; set; }
        public string NombreParticipante { get; set; }
        public int Calificacion { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }
}
