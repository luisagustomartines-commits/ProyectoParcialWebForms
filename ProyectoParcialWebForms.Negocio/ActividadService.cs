using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoParcialWebForms.Datos;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Negocio
{
    /// <summary>
    /// Servicio de negocio de actividades académicas. Concentra las validaciones y reglas
    /// (campos obligatorios, fechas, horas, cupo, estado, permisos) y coordina al repositorio.
    /// No conoce controles de la página ni escribe SQL.
    /// </summary>
    public class ActividadService
    {
        public static readonly string[] Modalidades = { "Presencial", "Virtual", "Híbrida" };
        public static readonly string[] Estados = { "Activa", "Próxima", "Finalizada", "Inactiva" };

        private const int CupoMaximo = 1000;
        private const string SinPermiso = "No tiene permisos para realizar esta operación.";

        private readonly ActividadRepository _actividades = new ActividadRepository();
        private readonly TipoActividadRepository _tipos = new TipoActividadRepository();

        public List<TipoActividadDto> ListarTipos()
        {
            return _tipos.Listar();
        }

        public List<ActividadDto> Buscar(string texto, int tipoId, string estado, DateTime? desde, DateTime? hasta)
        {
            return _actividades.Buscar((texto ?? string.Empty).Trim(), tipoId < 0 ? 0 : tipoId, (estado ?? string.Empty).Trim(), desde, hasta);
        }

        public ActividadDto ObtenerPorId(int actividadId)
        {
            return actividadId <= 0 ? null : _actividades.ObtenerPorId(actividadId);
        }

        /// <summary>Registra (ActividadId = 0) o actualiza una actividad. Solo organizadores.</summary>
        public ResultadoOperacion Guardar(ActividadDto actividad, UsuarioSesion actor)
        {
            if (actor == null || !actor.EsOrganizador)
            {
                return ResultadoOperacion.Falla(SinPermiso);
            }

            List<string> errores = Validar(actividad);

            try
            {
                // Regla: no se puede bajar el cupo por debajo de las inscripciones activas.
                if (errores.Count == 0 && actividad.ActividadId > 0)
                {
                    int inscritos = _actividades.ContarInscritosActivos(actividad.ActividadId);
                    if (actividad.Cupo < inscritos)
                    {
                        errores.Add("El cupo no puede ser menor que las " + inscritos + " inscripciones activas.");
                    }
                }

                if (errores.Count > 0)
                {
                    return ResultadoOperacion.Falla(errores);
                }

                if (actividad.ActividadId <= 0)
                {
                    int id = _actividades.Insertar(actividad);
                    return ResultadoOperacion.Exito("Actividad registrada correctamente.", id);
                }

                if (!_actividades.Actualizar(actividad))
                {
                    return ResultadoOperacion.Falla("La actividad que intenta modificar ya no existe.");
                }
                return ResultadoOperacion.Exito("Actividad actualizada correctamente.", actividad.ActividadId);
            }
            catch (ActividadDuplicadaException ex)
            {
                return ResultadoOperacion.Falla(ex.Message);
            }
            catch (AccesoDatosException)
            {
                return ResultadoOperacion.Falla("No fue posible guardar la actividad. Intente nuevamente.");
            }
        }

        /// <summary>Desactiva una actividad (eliminación lógica: conserva el historial). Solo organizadores.</summary>
        public ResultadoOperacion Desactivar(int actividadId, UsuarioSesion actor)
        {
            if (actor == null || !actor.EsOrganizador)
            {
                return ResultadoOperacion.Falla(SinPermiso);
            }

            if (actividadId <= 0)
            {
                return ResultadoOperacion.Falla("No se indicó qué actividad desactivar.");
            }

            try
            {
                return _actividades.Desactivar(actividadId)
                    ? ResultadoOperacion.Exito("Actividad desactivada correctamente.", actividadId)
                    : ResultadoOperacion.Falla("La actividad no existe o ya estaba inactiva.");
            }
            catch (AccesoDatosException)
            {
                return ResultadoOperacion.Falla("No fue posible desactivar la actividad. Intente nuevamente.");
            }
        }

        /// <summary>Valida todas las reglas y devuelve la lista completa de errores (vacía si es válida).</summary>
        public List<string> Validar(ActividadDto a)
        {
            List<string> e = new List<string>();

            if (a == null)
            {
                e.Add("No se recibieron los datos de la actividad.");
                return e;
            }

            a.Titulo = (a.Titulo ?? string.Empty).Trim();
            a.Descripcion = (a.Descripcion ?? string.Empty).Trim();
            a.Modalidad = (a.Modalidad ?? string.Empty).Trim();
            a.Lugar = (a.Lugar ?? string.Empty).Trim();
            a.Estado = (a.Estado ?? string.Empty).Trim();

            if (a.Titulo.Length == 0) e.Add("El título es obligatorio.");
            else if (a.Titulo.Length > 120) e.Add("El título no puede superar 120 caracteres.");

            if (a.Descripcion.Length == 0) e.Add("La descripción es obligatoria.");
            else if (a.Descripcion.Length > 1000) e.Add("La descripción no puede superar 1000 caracteres.");

            if (a.TipoId <= 0) e.Add("Seleccione el tipo de actividad.");

            if (!a.FechaInicio.HasValue) e.Add("La fecha de inicio es obligatoria.");
            if (!a.FechaFin.HasValue) e.Add("La fecha de fin es obligatoria.");
            if (a.FechaInicio.HasValue && a.FechaFin.HasValue && a.FechaFin.Value.Date < a.FechaInicio.Value.Date)
            {
                e.Add("La fecha de fin no puede ser anterior a la fecha de inicio.");
            }

            if (!a.HoraInicio.HasValue) e.Add("La hora de inicio es obligatoria.");
            if (!a.HoraFin.HasValue) e.Add("La hora de fin es obligatoria.");
            if (a.HoraInicio.HasValue && a.HoraFin.HasValue && a.HoraFin.Value <= a.HoraInicio.Value)
            {
                e.Add("La hora de fin debe ser posterior a la hora de inicio.");
            }

            if (!Modalidades.Contains(a.Modalidad)) e.Add("Seleccione una modalidad válida.");

            if (a.Lugar.Length == 0) e.Add("El lugar es obligatorio.");
            else if (a.Lugar.Length > 150) e.Add("El lugar no puede superar 150 caracteres.");

            if (a.Cupo <= 0) e.Add("El cupo debe ser un número entero mayor que cero.");
            else if (a.Cupo > CupoMaximo) e.Add("El cupo no puede superar " + CupoMaximo + " personas.");

            if (!Estados.Contains(a.Estado)) e.Add("Seleccione un estado válido.");

            return e;
        }
    }
}
