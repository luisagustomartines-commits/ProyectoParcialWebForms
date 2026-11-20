using System.Collections.Generic;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Views
{
    public interface IReportesView
    {
        UsuarioSesion UsuarioActual { get; }
        void MostrarResumen(ResumenGeneralDto resumen);
        void MostrarActividades(IList<ReporteActividadDto> actividades);
        void MostrarMensaje(string mensaje, bool esError);
    }
}
