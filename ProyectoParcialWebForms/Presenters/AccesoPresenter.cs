using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Negocio;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms.Presenters
{
    /// <summary>Coordina el ingreso: valida con el servicio y le pide a la vista iniciar la sesión.</summary>
    public class AccesoPresenter
    {
        private readonly IAccesoView _vista;
        private readonly AutenticacionService _servicio = new AutenticacionService();

        public AccesoPresenter(IAccesoView vista)
        {
            _vista = vista;
        }

        public void Ingresar()
        {
            try
            {
                UsuarioSesion usuario = _servicio.Ingresar(_vista.Identificacion, _vista.Correo);
                _vista.IniciarSesion(usuario);
                _vista.Redirigir("Catalogo");
            }
            catch (ReglaNegocioException ex)
            {
                _vista.MostrarMensaje(ex.Message, true);
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible validar sus datos. Revise la conexión con la base de datos.", true);
            }
        }

        public void Salir()
        {
            _vista.CerrarSesion();
            _vista.MostrarMensaje("Sesión cerrada correctamente.", false);
        }
    }
}
