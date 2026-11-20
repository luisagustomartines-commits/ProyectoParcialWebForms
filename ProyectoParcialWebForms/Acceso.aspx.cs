using System;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Presenters;
using ProyectoParcialWebForms.Seguridad;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms
{
    public partial class Acceso : System.Web.UI.Page, IAccesoView
    {
        private AccesoPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new AccesoPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["salir"] == "1")
            {
                _presenter.Salir();
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            _presenter.Ingresar();
        }

        // ---- IAccesoView ----
        public string Identificacion
        {
            get { return txtIdentificacion.Text; }
        }

        public string Correo
        {
            get { return txtCorreo.Text; }
        }

        public void IniciarSesion(UsuarioSesion usuario)
        {
            SesionWeb.Iniciar(usuario);
        }

        public void CerrarSesion()
        {
            SesionWeb.Cerrar();
        }

        public void Redirigir(string destino)
        {
            Response.Redirect(destino, false);
            Context.ApplicationInstance.CompleteRequest();
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.CssClass = esError ? "msg err" : "msg ok";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }
    }
}
