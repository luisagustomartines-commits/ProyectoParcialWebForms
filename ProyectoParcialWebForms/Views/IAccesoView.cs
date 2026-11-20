using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Views
{
    public interface IAccesoView
    {
        string Identificacion { get; }
        string Correo { get; }
        void IniciarSesion(UsuarioSesion usuario);
        void CerrarSesion();
        void Redirigir(string destino);
        void MostrarMensaje(string mensaje, bool esError);
    }
}
