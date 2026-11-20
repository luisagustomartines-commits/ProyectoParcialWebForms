using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Presenters;
using ProyectoParcialWebForms.Seguridad;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms
{
    /// <summary>
    /// Vista (MVP) de la gestión de actividades. Solo recoge datos de los controles,
    /// muestra lo que el Presenter ordena y delega cada acción en ActividadesPresenter.
    /// </summary>
    public partial class Actividades : System.Web.UI.Page, IActividadesView
    {
        private ActividadesPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            SesionWeb.RequerirOrganizador(this);
            _presenter = new ActividadesPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                _presenter.Inicializar();
            }
        }

        // ------------------------------------------------------------
        // Eventos: solo avisan al Presenter
        // ------------------------------------------------------------
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            _presenter.Guardar();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            _presenter.Limpiar();
        }

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

        protected void gvActividades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out id))
            {
                return;
            }

            if (e.CommandName == "Editar")
            {
                _presenter.Editar(id);
            }
            else if (e.CommandName == "Desactivar")
            {
                _presenter.Desactivar(id);
            }
        }

        // ------------------------------------------------------------
        // Implementación del contrato IActividadesView
        // ------------------------------------------------------------
        public UsuarioSesion UsuarioActual
        {
            get { return SesionWeb.Actual; }
        }

        public string FiltroDesde
        {
            get { return txtDesde.Text; }
        }

        public string FiltroHasta
        {
            get { return txtHasta.Text; }
        }

        public ActividadFormulario ObtenerFormulario()
        {
            return new ActividadFormulario
            {
                ActividadId = hfActividadId.Value,
                Titulo = txtTitulo.Text,
                Descripcion = txtDescripcion.Text,
                TipoId = ddlTipo.SelectedValue,
                FechaInicio = txtFechaInicio.Text,
                FechaFin = txtFechaFin.Text,
                HoraInicio = txtHoraInicio.Text,
                HoraFin = txtHoraFin.Text,
                Modalidad = ddlModalidad.SelectedValue,
                Lugar = txtLugar.Text,
                Cupo = txtCupo.Text,
                Estado = ddlEstado.SelectedValue
            };
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

        public void MostrarTipos(IList<TipoActividadDto> tipos)
        {
            ddlTipo.Items.Clear();
            ddlTipo.Items.Add(new ListItem("-- Seleccione un tipo --", "0"));

            ddlFiltroTipo.Items.Clear();
            ddlFiltroTipo.Items.Add(new ListItem("Todos los tipos", "0"));

            foreach (TipoActividadDto tipo in tipos)
            {
                ddlTipo.Items.Add(new ListItem(tipo.Nombre, tipo.TipoId.ToString()));
                ddlFiltroTipo.Items.Add(new ListItem(tipo.Nombre, tipo.TipoId.ToString()));
            }
        }

        public void MostrarActividades(IList<ActividadDto> actividades)
        {
            gvActividades.DataSource = actividades;
            gvActividades.DataBind();
            lblTotal.Text = "Actividades mostradas: " + actividades.Count;
        }

        public void MostrarEnFormulario(ActividadDto a)
        {
            hfActividadId.Value = a.ActividadId.ToString();
            lblModo.Text = "Editando actividad #" + a.ActividadId;
            btnGuardar.Text = "Actualizar";

            txtTitulo.Text = a.Titulo;
            txtDescripcion.Text = a.Descripcion;
            Seleccionar(ddlTipo, a.TipoId.ToString());
            txtFechaInicio.Text = a.FechaInicio.HasValue ? a.FechaInicio.Value.ToString("yyyy-MM-dd") : string.Empty;
            txtFechaFin.Text = a.FechaFin.HasValue ? a.FechaFin.Value.ToString("yyyy-MM-dd") : string.Empty;
            txtHoraInicio.Text = a.HoraInicio.HasValue ? a.HoraInicio.Value.ToString(@"hh\:mm") : string.Empty;
            txtHoraFin.Text = a.HoraFin.HasValue ? a.HoraFin.Value.ToString(@"hh\:mm") : string.Empty;
            Seleccionar(ddlModalidad, a.Modalidad);
            txtCupo.Text = a.Cupo.ToString();
            txtLugar.Text = a.Lugar;
            Seleccionar(ddlEstado, a.Estado);
        }

        public void LimpiarFormulario()
        {
            hfActividadId.Value = string.Empty;
            lblModo.Text = "Nueva actividad";
            btnGuardar.Text = "Guardar";

            txtTitulo.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            ddlTipo.SelectedIndex = 0;
            txtFechaInicio.Text = string.Empty;
            txtFechaFin.Text = string.Empty;
            txtHoraInicio.Text = string.Empty;
            txtHoraFin.Text = string.Empty;
            ddlModalidad.SelectedIndex = 0;
            txtCupo.Text = string.Empty;
            txtLugar.Text = string.Empty;
            ddlEstado.SelectedIndex = 0;

            ltErrores.Text = string.Empty;
            lblMensaje.Text = string.Empty;
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

            System.Text.StringBuilder html = new System.Text.StringBuilder("<ul class=\"errores\">");
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
        protected string FormatoHorario(object inicio, object fin)
        {
            if (inicio is TimeSpan && fin is TimeSpan)
            {
                return ((TimeSpan)inicio).ToString(@"hh\:mm") + " - " + ((TimeSpan)fin).ToString(@"hh\:mm");
            }
            return string.Empty;
        }

        private static void Seleccionar(DropDownList lista, string valor)
        {
            ListItem item = lista.Items.FindByValue(valor);
            if (item != null)
            {
                lista.ClearSelection();
                item.Selected = true;
            }
        }
    }
}
