using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Negocio;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms.Presenters
{
    public class ReportesPresenter
    {
        private readonly IReportesView _vista;
        private readonly ReporteService _servicio = new ReporteService();

        public ReportesPresenter(IReportesView vista)
        {
            _vista = vista;
        }

        public void Cargar()
        {
            try
            {
                _vista.MostrarResumen(_servicio.ObtenerResumen(_vista.UsuarioActual));
                _vista.MostrarActividades(_servicio.ObtenerPorActividad(_vista.UsuarioActual));
            }
            catch (ReglaNegocioException ex)
            {
                _vista.MostrarMensaje(ex.Message, true);
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible generar el reporte. Intente nuevamente.", true);
            }
        }
    }
}
