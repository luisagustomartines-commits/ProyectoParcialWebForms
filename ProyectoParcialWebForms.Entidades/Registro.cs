using System;

namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>
    /// Entidad que representa una fila de la tabla Registros.
    /// Se comparte entre todas las capas; no contiene lógica.
    /// </summary>
    public class Registro
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Programa { get; set; }
        public string Semestre { get; set; }
        public string Participacion { get; set; }
        public string Jornada { get; set; }
        public string Temas { get; set; }
        public string Comentarios { get; set; }
        public string Rol { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
