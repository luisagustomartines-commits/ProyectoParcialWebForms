using System.Collections.Generic;

namespace ProyectoParcialWebForms.Views
{
    /// <summary>Datos del formulario de participantes tal como los escribió el usuario.</summary>
    public class RegistroFormulario
    {
        public string Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Programa { get; set; }
        public string Semestre { get; set; }
        public string Participacion { get; set; }
        public string Jornada { get; set; }
        public IList<string> Temas { get; set; }
        public string Comentarios { get; set; }
    }
}
