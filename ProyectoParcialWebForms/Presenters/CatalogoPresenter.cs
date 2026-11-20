using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Negocio;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms.Presenters
{
    /// <summary>
    /// Presenter del catálogo: listado con filtros, detalle, inscripción con control de cupos,
    /// cancelación y reseñas. Toma la identidad de la persona SIEMPRE de la sesión (UsuarioActual).
    /// </summary>
    public class CatalogoPresenter
    {
        private readonly ICatalogoView _vista;
        private readonly ActividadService _actividades = new ActividadService();
        private readonly InscripcionService _inscripciones = new InscripcionService();
        private readonly ResenaService _resenas = new ResenaService();

        public CatalogoPresenter(ICatalogoView vista)
        {
            _vista = vista;
        }

        public void Inicializar()
        {
            try
            {
                _vista.MostrarTipos(_actividades.ListarTipos());
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible cargar los tipos de actividad. Revise la conexión con la base de datos.", true);
                return;
            }

            Refrescar();
        }

        public void Buscar()
        {
            CargarListado();
        }

        public void VerDetalle(int actividadId)
        {
            _vista.ActividadSeleccionadaId = actividadId;
            MostrarDetalleActual();
        }

        public void Inscribirse()
        {
            ResultadoOperacion r = _inscripciones.Inscribir(_vista.ActividadSeleccionadaId, _vista.UsuarioActual);
            Refrescar();
            Informar(r);
        }

        public void CancelarInscripcion(int actividadId)
        {
            ResultadoOperacion r = _inscripciones.Cancelar(actividadId, _vista.UsuarioActual);
            Refrescar();
            Informar(r);
        }

        public void PublicarResena()
        {
            int calificacion;
            if (!int.TryParse(_vista.CalificacionResena, NumberStyles.Integer, CultureInfo.InvariantCulture, out calificacion))
            {
                calificacion = 0;
            }

            ResultadoOperacion r = _resenas.Publicar(_vista.ActividadSeleccionadaId, calificacion, _vista.ComentarioResena, _vista.UsuarioActual);

            if (r.Exitoso)
            {
                _vista.LimpiarResena();
            }

            Refrescar();
            Informar(r);
        }

        // ------------------------------------------------------------
        private void Informar(ResultadoOperacion r)
        {
            if (r.Exitoso)
            {
                _vista.MostrarMensaje(r.Mensaje, false);
            }
            else
            {
                _vista.MostrarErrores(r.Errores);
            }
        }

        private void Refrescar()
        {
            CargarListado();
            CargarMisInscripciones();

            if (_vista.ActividadSeleccionadaId > 0)
            {
                MostrarDetalleActual();
            }
        }

        private void CargarListado()
        {
            List<string> errores = new List<string>();
            DateTime? desde = LeerFecha(_vista.FiltroDesde, "fecha «desde»", errores);
            DateTime? hasta = LeerFecha(_vista.FiltroHasta, "fecha «hasta»", errores);

            if (errores.Count > 0)
            {
                _vista.MostrarErrores(errores);
                return;
            }

            try
            {
                _vista.MostrarActividades(_actividades.Buscar(_vista.FiltroTexto, _vista.FiltroTipoId, _vista.FiltroEstado, desde, hasta));
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible cargar las actividades. Intente nuevamente.", true);
            }
        }

        private void CargarMisInscripciones()
        {
            try
            {
                _vista.MostrarMisInscripciones(_inscripciones.ListarMisInscripciones(_vista.UsuarioActual));
            }
            catch (ReglaNegocioException ex)
            {
                _vista.MostrarMensaje(ex.Message, true);
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible cargar sus inscripciones. Intente nuevamente.", true);
            }
        }

        private void MostrarDetalleActual()
        {
            int id = _vista.ActividadSeleccionadaId;
            UsuarioSesion actor = _vista.UsuarioActual;

            try
            {
                ActividadDto actividad = _actividades.ObtenerPorId(id);

                if (actividad == null || actividad.Estado == "Inactiva")
                {
                    _vista.ActividadSeleccionadaId = 0;
                    _vista.OcultarDetalle();
                    return;
                }

                bool inscrito = _inscripciones.ListarMisInscripciones(actor)
                    .Any(i => i.ActividadId == id && i.Estado == "Activa");

                List<ResenaDto> resenas = _resenas.ListarPorActividad(id);
                bool yaReseno = actor != null && resenas.Any(r => r.RegistroId == actor.RegistroId);

                _vista.MostrarDetalle(
                    actividad,
                    InscripcionService.PuedeInscribirse(actividad, inscrito),
                    inscrito,
                    ResenaService.PuedeResenar(actividad, inscrito, yaReseno));
                _vista.MostrarResenas(resenas);

                _vista.MostrarInscritos(actor != null && actor.EsOrganizador
                    ? _inscripciones.ListarInscritos(id, actor)
                    : null);
            }
            catch (ReglaNegocioException ex)
            {
                _vista.MostrarMensaje(ex.Message, true);
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible cargar el detalle de la actividad. Intente nuevamente.", true);
            }
        }

        private static DateTime? LeerFecha(string texto, string nombre, List<string> errores)
        {
            texto = (texto ?? string.Empty).Trim();
            if (texto.Length == 0) return null;

            DateTime fecha;
            if (DateTime.TryParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                return fecha;
            }

            errores.Add("La " + nombre + " no tiene un formato válido.");
            return null;
        }
    }
}
