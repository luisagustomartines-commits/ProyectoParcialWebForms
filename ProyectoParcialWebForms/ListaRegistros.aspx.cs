using System;
using System.Collections.Generic;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Presenters;
using ProyectoParcialWebForms.Seguridad;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms
{
    public partial class ListaRegistros : System.Web.UI.Page, IListaRegistrosView
    {
        private ListaRegistrosPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            SesionWeb.RequerirOrganizador(this);
            _presenter = new ListaRegistrosPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Cargar();
            }
        }

        public UsuarioSesion UsuarioActual
        {
            get { return SesionWeb.Actual; }
        }

        public void MostrarRegistros(IList<Registro> registros)
        {
            gvRegistros.DataSource = registros;
            gvRegistros.DataBind();
            lblTotal.Text = "Total de participantes: " + registros.Count;
        }

        public void MostrarMensaje(string mensaje)
        {
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }
    }
}
