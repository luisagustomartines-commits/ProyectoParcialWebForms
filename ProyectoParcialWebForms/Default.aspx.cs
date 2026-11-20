using System;
using System.Linq;
using System.Web.UI.WebControls;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Presenters;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms
{
    /// <summary>
    /// Vista (MVP) del formulario de participantes. Solo recoge y muestra datos;
    /// las acciones se delegan en RegistroPresenter.
    /// </summary>
    public partial class _Default : System.Web.UI.Page, IRegistroView
    {
        private RegistroPresenter _presenter;

        protected void Page_Init(object sender, EventArgs e)
        {
            _presenter = new RegistroPresenter(this);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlResultado.Visible = false;
            }
        }

        // ------------------------------------------------------------
        // Validadores de la página
        // ------------------------------------------------------------
        protected void cvTemas_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = cblTemas.Items.Cast<ListItem>().Any(item => item.Selected);
        }

        protected void cvAcepta_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkAcepta.Checked;
        }

        // ------------------------------------------------------------
        // Eventos: avisan al Presenter
        // ------------------------------------------------------------
        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
            {
                pnlResultado.Visible = false;
                return;
            }

            _presenter.Registrar();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            _presenter.Buscar();
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            _presenter.Limpiar();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.Url.AbsolutePath);
        }

        // ------------------------------------------------------------
        // IRegistroView: entrada
        // ------------------------------------------------------------
        public RegistroFormulario ObtenerFormulario()
        {
            return new RegistroFormulario
            {
                Identificacion = txtIdentificacion.Text,
                Nombres = txtNombres.Text,
                Apellidos = txtApellidos.Text,
                Correo = txtCorreo.Text,
                Telefono = txtTelefono.Text,
                Programa = ddlPrograma.SelectedItem.Text,
                Semestre = ddlSemestre.SelectedItem.Text,
                Participacion = rblParticipacion.SelectedItem == null ? string.Empty : rblParticipacion.SelectedItem.Text,
                Jornada = ddlJornada.SelectedItem.Text,
                Temas = cblTemas.Items.Cast<ListItem>().Where(item => item.Selected).Select(item => item.Text).ToList(),
                Comentarios = txtComentarios.Text
            };
        }

        public string BuscarIdentificacion
        {
            get { return txtBuscarId.Text; }
        }

        public string BuscarCorreo
        {
            get { return txtBuscarCorreo.Text; }
        }

        public int? RegistroEditandoId
        {
            get { return ViewState["RegistroEditandoId"] as int?; }
            set { ViewState["RegistroEditandoId"] = value; }
        }

        // ------------------------------------------------------------
        // IRegistroView: salida
        // ------------------------------------------------------------
        public void MostrarRegistroEnFormulario(Registro registro)
        {
            string idBuscado = txtBuscarId.Text;
            string correoBuscado = txtBuscarCorreo.Text;
            LimpiarFormulario();
            txtBuscarId.Text = idBuscado;
            txtBuscarCorreo.Text = correoBuscado;

            RegistroEditandoId = registro.Id;

            txtIdentificacion.Text = registro.Identificacion;
            txtIdentificacion.ReadOnly = true; // la identificación identifica el registro, no se cambia
            txtNombres.Text = registro.Nombres;
            txtApellidos.Text = registro.Apellidos;
            txtCorreo.Text = registro.Correo;
            txtTelefono.Text = registro.Telefono;
            txtComentarios.Text = registro.Comentarios;

            Seleccionar(ddlPrograma, registro.Programa);
            Seleccionar(ddlSemestre, registro.Semestre);
            Seleccionar(rblParticipacion, registro.Participacion);
            Seleccionar(ddlJornada, registro.Jornada);

            string[] temas = (registro.Temas ?? string.Empty).Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tema in temas)
            {
                ListItem item = cblTemas.Items.FindByText(tema);
                if (item != null)
                {
                    item.Selected = true;
                }
            }

            chkAcepta.Checked = true; // ya aceptó el tratamiento de datos al registrarse
            btnRegistrar.Text = "ACTUALIZAR REGISTRO";
        }

        public void LimpiarFormulario()
        {
            RegistroEditandoId = null;
            btnRegistrar.Text = "REGISTRAR";

            txtBuscarId.Text = string.Empty;
            txtBuscarCorreo.Text = string.Empty;
            lblBusqueda.Text = string.Empty;
            lblError.Text = string.Empty;

            txtIdentificacion.ReadOnly = false;
            txtIdentificacion.Text = string.Empty;
            txtNombres.Text = string.Empty;
            txtApellidos.Text = string.Empty;
            txtCorreo.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            ddlPrograma.SelectedIndex = 0;
            ddlSemestre.SelectedIndex = 0;
            rblParticipacion.ClearSelection();
            ddlJornada.SelectedIndex = 0;

            foreach (ListItem item in cblTemas.Items)
            {
                item.Selected = false;
            }

            txtComentarios.Text = string.Empty;
            chkAcepta.Checked = false;
        }

        public void MostrarResultado(string mensaje, string codigo, string resumen)
        {
            // Server.HtmlEncode evita que lo escrito por el usuario se interprete como HTML
            lblMensaje.Text = Server.HtmlEncode(mensaje);
            lblCodigo.Text = Server.HtmlEncode(codigo);
            lblResumen.Text = Server.HtmlEncode(resumen);
            pnlResultado.Visible = true;
        }

        public void OcultarResultado()
        {
            pnlResultado.Visible = false;
        }

        public void MostrarError(string mensaje)
        {
            lblError.Text = Server.HtmlEncode(mensaje ?? string.Empty);
            if (!string.IsNullOrEmpty(mensaje))
            {
                pnlResultado.Visible = false;
            }
        }

        public void MostrarBusqueda(string mensaje, bool correcto)
        {
            lblBusqueda.CssClass = correcto ? "info-text ok-text" : "info-text error-text";
            lblBusqueda.Text = Server.HtmlEncode(mensaje);
        }

        private static void Seleccionar(ListControl lista, string texto)
        {
            lista.ClearSelection();
            ListItem item = lista.Items.FindByText(texto);
            if (item != null)
            {
                item.Selected = true;
            }
        }
    }
}
