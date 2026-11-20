using System;
using System.Collections.Generic;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Presenters;
using ProyectoParcialWebForms.Seguridad;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms
{
    public partial class Reportes : System.Web.UI.Page, IReportesView
    {
        private ReportesPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            SesionWeb.RequerirOrganizador(this);
            _presenter = new ReportesPresenter(this);
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

        public void MostrarResumen(ResumenGeneralDto r)
        {
            lblTotalActividades.Text = r.TotalActividades.ToString();
            lblActivas.Text = r.ActividadesActivas.ToString();
            lblProximas.Text = r.ActividadesProximas.ToString();
            lblFinalizadas.Text = r.ActividadesFinalizadas.ToString();
            lblParticipantes.Text = r.TotalParticipantes.ToString();
            lblInscripciones.Text = r.InscripcionesActivas.ToString();
            lblResenas.Text = r.TotalResenas.ToString();
            lblPromedio.Text = r.TotalResenas == 0 ? "—" : r.PromedioGeneral.ToString("0.0");
        }

        public void MostrarActividades(IList<ReporteActividadDto> actividades)
        {
            gvReporte.DataSource = actividades;
            gvReporte.DataBind();
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            lblMensaje.CssClass = esError ? "msg err" : "msg ok";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }
    }
}
