using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Presenters;
using ProyectoParcialWebForms.Seguridad;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms
{
    /// <summary>Vista (MVP) del catálogo. Cada acción se delega en CatalogoPresenter.</summary>
    public partial class Catalogo : System.Web.UI.Page, ICatalogoView
    {
        private CatalogoPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            SesionWeb.RequerirLogin(this);
            _presenter = new CatalogoPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Inicializar();
            }
        }

        // ------------------------------------------------------------
        // Eventos
        // ------------------------------------------------------------
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            _presenter.Buscar();
        }

        protected void btnVerTodas_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = string.Empty;
            ddlFiltroTipo.SelectedIndex = 0;
            ddlFiltroEstado.SelectedIndex = 0;
            txtDesde.Text = string.Empty;
            txtHasta.Text = string.Empty;
            _presenter.Buscar();
        }

        protected void gvCatalogo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "Detalle" && int.TryParse(Convert.ToString(e.CommandArgument), out id))
            {
                _presenter.VerDetalle(id);
            }
        }

        protected void gvMisInscripciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (e.CommandName == "CancelarInscripcion" && int.TryParse(Convert.ToString(e.CommandArgument), out id))
            {
                _presenter.CancelarInscripcion(id);
            }
        }

        protected void btnInscribirme_Click(object sender, EventArgs e)
        {
            _presenter.Inscribirse();
        }

        protected void btnCancelarInscripcion_Click(object sender, EventArgs e)
        {
            _presenter.CancelarInscripcion(ActividadSeleccionadaId);
        }

        protected void btnPublicarResena_Click(object sender, EventArgs e)
        {
            _presenter.PublicarResena();
        }

        // ------------------------------------------------------------
        // ICatalogoView: entrada
        // ------------------------------------------------------------
        public UsuarioSesion UsuarioActual
        {
            get { return SesionWeb.Actual; }
        }

        public string FiltroTexto
        {
            get { return txtBuscar.Text; }
        }

        public int FiltroTipoId
        {
            get
            {
                int id;
                return int.TryParse(ddlFiltroTipo.SelectedValue, out id) ? id : 0;
            }
        }

        public string FiltroEstado
        {
            get { return ddlFiltroEstado.SelectedValue; }
        }

        public string FiltroDesde
        {
            get { return txtDesde.Text; }
        }

        public string FiltroHasta
        {
            get { return txtHasta.Text; }
        }

        public int ActividadSeleccionadaId
        {
            get
            {
                object valor = ViewState["ActividadSeleccionadaId"];
                return valor == null ? 0 : (int)valor;
            }
            set { ViewState["ActividadSeleccionadaId"] = value; }
        }

        public string CalificacionResena
        {
            get { return ddlCalificacion.SelectedValue; }
        }

        public string ComentarioResena
        {
            get { return txtComentarioResena.Text; }
        }

        // ------------------------------------------------------------
        // ICatalogoView: salida
        // ------------------------------------------------------------
        public void MostrarTipos(IList<TipoActividadDto> tipos)
        {
            ddlFiltroTipo.Items.Clear();
            ddlFiltroTipo.Items.Add(new ListItem("Todos los tipos", "0"));

            foreach (TipoActividadDto tipo in tipos)
            {
                ddlFiltroTipo.Items.Add(new ListItem(tipo.Nombre, tipo.TipoId.ToString()));
            }
        }

        public void MostrarActividades(IList<ActividadDto> actividades)
        {
            gvCatalogo.DataSource = actividades;
            gvCatalogo.DataBind();
            lblTotal.Text = "Actividades mostradas: " + actividades.Count;
        }

        public void MostrarMisInscripciones(IList<InscripcionDto> inscripciones)
        {
            gvMisInscripciones.DataSource = inscripciones;
            gvMisInscripciones.DataBind();
        }

        public void MostrarDetalle(ActividadDto a, bool puedeInscribirse, bool inscrito, bool puedeResenar)
        {
            pnlDetalle.Visible = true;

            lblDetTitulo.Text = Server.HtmlEncode(a.Titulo);
            lblDetTipo.Text = Server.HtmlEncode(a.TipoNombre);
            lblDetEstado.Text = Server.HtmlEncode(a.Estado);
            lblDetModalidad.Text = Server.HtmlEncode(a.Modalidad);
            lblDetDescripcion.Text = Server.HtmlEncode(a.Descripcion);
            lblDetLugar.Text = Server.HtmlEncode(a.Lugar);

            string inicio = a.FechaInicio.HasValue ? a.FechaInicio.Value.ToString("dd/MM/yyyy") : string.Empty;
            string fin = a.FechaFin.HasValue ? a.FechaFin.Value.ToString("dd/MM/yyyy") : string.Empty;
            lblDetFecha.Text = inicio == fin ? inicio : inicio + " - " + fin;
            lblDetHorario.Text = FormatoHorario(a.HoraInicio, a.HoraFin);
            lblDetCupos.Text = a.CuposDisponibles + " disponibles de " + a.Cupo + " (" + a.Inscritos + " inscritos)";
            lblDetValoracion.Text = FormatoPromedio(a.PromedioCalificacion, a.TotalResenas);

            btnInscribirme.Visible = puedeInscribirse;
            btnCancelarInscripcion.Visible = inscrito;
            pnlNuevaResena.Visible = puedeResenar;

            if (inscrito)
            {
                lblDetAviso.Text = "✔ Usted está inscrito en esta actividad.";
            }
            else if (!puedeInscribirse)
            {
                lblDetAviso.Text = "Esta actividad no admite nuevas inscripciones (sin cupos, finalizada o inactiva).";
            }
            else
            {
                lblDetAviso.Text = string.Empty;
            }
        }

        public void OcultarDetalle()
        {
            pnlDetalle.Visible = false;
        }

        public void MostrarResenas(IList<ResenaDto> resenas)
        {
            gvResenas.DataSource = resenas;
            gvResenas.DataBind();
        }

        public void MostrarInscritos(IList<InscripcionDto> inscritos)
        {
            pnlInscritos.Visible = inscritos != null;

            if (inscritos != null)
            {
                gvInscritos.DataSource = inscritos;
                gvInscritos.DataBind();
            }
        }

        public void LimpiarResena()
        {
            ddlCalificacion.SelectedIndex = 0;
            txtComentarioResena.Text = string.Empty;
        }

        public void MostrarMensaje(string mensaje, bool esError)
        {
            ltErrores.Text = string.Empty;
            lblMensaje.CssClass = esError ? "msg err" : "msg ok";
            lblMensaje.Text = Server.HtmlEncode(mensaje);
        }

        public void MostrarErrores(IList<string> errores)
        {
            lblMensaje.Text = string.Empty;

            StringBuilder html = new StringBuilder("<ul class=\"errores\">");
            foreach (string error in errores)
            {
                html.Append("<li>").Append(Server.HtmlEncode(error)).Append("</li>");
            }
            html.Append("</ul>");
            ltErrores.Text = html.ToString();
        }

        // ------------------------------------------------------------
        // Utilidades de presentación
        // ------------------------------------------------------------
        private static string FormatoHorario(TimeSpan? inicio, TimeSpan? fin)
        {
            if (inicio.HasValue && fin.HasValue)
            {
                return inicio.Value.ToString(@"hh\:mm") + " - " + fin.Value.ToString(@"hh\:mm");
            }
            return string.Empty;
        }

        protected string FormatoPromedio(object promedio, object total)
        {
            int cantidad = Convert.ToInt32(total);
            if (cantidad == 0)
            {
                return "Sin reseñas";
            }
            return Convert.ToDouble(promedio).ToString("0.0") + " ★ (" + cantidad + ")";
        }

        protected string Estrellas(object calificacion)
        {
            int n = Math.Max(0, Math.Min(5, Convert.ToInt32(calificacion)));
            return new string('★', n) + new string('☆', 5 - n);
        }
    }
}
