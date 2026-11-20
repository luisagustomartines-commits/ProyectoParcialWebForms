using System.Collections.Generic;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Views
{
    /// <summary>
    /// Contrato de la vista (MVP): lo que el Presenter necesita pedirle o indicarle a la página.
    /// No contiene HTML, controles ni SQL.
    /// </summary>
    public interface IActividadesView
    {
        // --- Entrada: lo que el usuario escribió y la sesión ---
        UsuarioSesion UsuarioActual { get; }
        ActividadFormulario ObtenerFormulario();
        string FiltroTexto { get; }
        int FiltroTipoId { get; }
        string FiltroEstado { get; }
        string FiltroDesde { get; }
        string FiltroHasta { get; }

        // --- Salida: lo que el Presenter ordena mostrar ---
        void MostrarTipos(IList<TipoActividadDto> tipos);
        void MostrarActividades(IList<ActividadDto> actividades);
        void MostrarEnFormulario(ActividadDto actividad);
        void LimpiarFormulario();
        void MostrarMensaje(string mensaje, bool esError);
        void MostrarErrores(IList<string> errores);
    }
}
