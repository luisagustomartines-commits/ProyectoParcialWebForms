namespace ProyectoParcialWebForms.Views
{
    /// <summary>
    /// Datos del formulario tal como los escribió el usuario (todo en texto).
    /// La vista solo los recoge; el Presenter los convierte y los envía al negocio.
    /// </summary>
    public class ActividadFormulario
    {
        public string ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string TipoId { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string Modalidad { get; set; }
        public string Lugar { get; set; }
        public string Cupo { get; set; }
        public string Estado { get; set; }
    }
}
