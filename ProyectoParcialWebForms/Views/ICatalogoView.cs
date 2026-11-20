using System.Collections.Generic;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Views
{
    /// <summary>Contrato de la vista del catálogo: listado, detalle, inscripción y reseñas.</summary>
    public interface ICatalogoView
    {
        // --- Entrada ---
        UsuarioSesion UsuarioActual { get; }
        string FiltroTexto { get; }
        int FiltroTipoId { get; }
        string FiltroEstado { get; }
        string FiltroDesde { get; }
        string FiltroHasta { get; }
        int ActividadSeleccionadaId { get; set; }
        string CalificacionResena { get; }
        string ComentarioResena { get; }

        // --- Salida ---
        void MostrarTipos(IList<TipoActividadDto> tipos);
        void MostrarActividades(IList<ActividadDto> actividades);
        void MostrarMisInscripciones(IList<InscripcionDto> inscripciones);
        void MostrarDetalle(ActividadDto actividad, bool puedeInscribirse, bool inscrito, bool puedeResenar);
        void OcultarDetalle();
        void MostrarResenas(IList<ResenaDto> resenas);
        void MostrarInscritos(IList<InscripcionDto> inscritos); // null = ocultar (no es organizador)
        void LimpiarResena();
        void MostrarMensaje(string mensaje, bool esError);
        void MostrarErrores(IList<string> errores);
    }
}
