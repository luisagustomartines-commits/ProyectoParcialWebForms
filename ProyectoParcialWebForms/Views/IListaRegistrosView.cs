using System.Collections.Generic;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Views
{
    public interface IListaRegistrosView
    {
        UsuarioSesion UsuarioActual { get; }
        void MostrarRegistros(IList<Registro> registros);
        void MostrarMensaje(string mensaje);
    }
}
