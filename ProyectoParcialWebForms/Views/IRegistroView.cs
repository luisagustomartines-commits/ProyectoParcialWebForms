using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Views
{
    /// <summary>Contrato de la vista del formulario de participantes.</summary>
    public interface IRegistroView
    {
        // --- Entrada ---
        RegistroFormulario ObtenerFormulario();
        string BuscarIdentificacion { get; }
        string BuscarCorreo { get; }
        int? RegistroEditandoId { get; set; }

        // --- Salida ---
        void MostrarRegistroEnFormulario(Registro registro);
        void LimpiarFormulario();
        void MostrarResultado(string mensaje, string codigo, string resumen);
        void OcultarResultado();
        void MostrarError(string mensaje);
        void MostrarBusqueda(string mensaje, bool correcto);
    }
}
