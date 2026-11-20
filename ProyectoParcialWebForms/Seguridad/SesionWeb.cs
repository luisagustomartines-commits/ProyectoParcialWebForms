using System.Text;
using System.Web;
using System.Web.UI;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Seguridad
{
    /// <summary>
    /// Sesión del usuario autenticado y guardas de acceso a las páginas.
    /// El usuario se guarda en la sesión del servidor: nunca se confía en un id enviado desde un control.
    /// </summary>
    public static class SesionWeb
    {
        private const string Clave = "UsuarioSesion";

        public static UsuarioSesion Actual
        {
            get
            {
                HttpContext contexto = HttpContext.Current;
                return (contexto == null || contexto.Session == null) ? null : contexto.Session[Clave] as UsuarioSesion;
            }
        }

        public static void Iniciar(UsuarioSesion usuario)
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session[Clave] = usuario;
        }

        public static void Cerrar()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        /// <summary>Redirige al ingreso si no hay sesión iniciada.</summary>
        public static void RequerirLogin(Page pagina)
        {
            if (Actual == null)
            {
                pagina.Response.Redirect("Acceso", true);
            }
        }

        /// <summary>Redirige al ingreso (sin sesión) o al catálogo (sin rol de organizador).</summary>
        public static void RequerirOrganizador(Page pagina)
        {
            UsuarioSesion usuario = Actual;

            if (usuario == null)
            {
                pagina.Response.Redirect("Acceso", true);
            }
            else if (!usuario.EsOrganizador)
            {
                pagina.Response.Redirect("Catalogo", true);
            }
        }
    }

    /// <summary>Menú de navegación que cambia según la sesión y el rol.</summary>
    public static class Navegacion
    {
        public static string Render()
        {
            UsuarioSesion usuario = SesionWeb.Actual;
            StringBuilder html = new StringBuilder(
                "<div style=\"margin-top:12px;display:flex;gap:18px;flex-wrap:wrap;align-items:center\">");

            Enlace(html, "Default", "Registro de participantes");

            if (usuario == null)
            {
                Enlace(html, "Acceso", "Ingresar");
            }
            else
            {
                Enlace(html, "Catalogo", "Actividades disponibles");

                if (usuario.EsOrganizador)
                {
                    Enlace(html, "Actividades", "Gestión de actividades");
                    Enlace(html, "Reportes", "Reportes");
                    Enlace(html, "ListaRegistros", "Participantes");
                }

                html.Append("<span style=\"color:#fff;font-size:.82rem;opacity:.85\">👤 ")
                    .Append(HttpUtility.HtmlEncode(usuario.NombreCompleto))
                    .Append(" (").Append(HttpUtility.HtmlEncode(usuario.Rol)).Append(")</span>");
                Enlace(html, "Acceso?salir=1", "Cerrar sesión");
            }

            html.Append("</div>");
            return html.ToString();
        }

        private static void Enlace(StringBuilder html, string destino, string texto)
        {
            html.Append("<a href=\"").Append(destino)
                .Append("\" style=\"color:#e0a640;font-weight:600;font-size:.86rem;text-decoration:none\">")
                .Append(HttpUtility.HtmlEncode(texto)).Append("</a>");
        }
    }
}
