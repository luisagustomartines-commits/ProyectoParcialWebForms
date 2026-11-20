using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Negocio;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms.Presenters
{
    public class ListaRegistrosPresenter
    {
        private readonly IListaRegistrosView _vista;
        private readonly RegistroService _servicio = new RegistroService();

        public ListaRegistrosPresenter(IListaRegistrosView vista)
        {
            _vista = vista;
        }

        public void Cargar()
        {
            try
            {
                _vista.MostrarRegistros(_servicio.Listar(_vista.UsuarioActual));
            }
            catch (ReglaNegocioException ex)
            {
                _vista.MostrarMensaje(ex.Message);
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible leer los participantes. Revise la conexión con la base de datos.");
            }
        }
    }
}
