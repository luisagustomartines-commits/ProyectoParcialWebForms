using System;
using System.Collections.Generic;
using ProyectoParcialWebForms.Datos;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Negocio
{
    /// <summary>
    /// Reglas de inscripción: usuario autenticado, actividad disponible, sin duplicados y con cupo.
    /// El identificador de la persona siempre llega desde la sesión (UsuarioSesion).
    /// </summary>
    public class InscripcionService
    {
        private readonly InscripcionRepository _inscripciones = new InscripcionRepository();

        public ResultadoOperacion Inscribir(int actividadId, UsuarioSesion actor)
        {
            if (actor == null)
            {
                return ResultadoOperacion.Falla("Debe iniciar sesión para inscribirse.");
            }

            if (actividadId <= 0)
            {
                return ResultadoOperacion.Falla("No se indicó la actividad.");
            }

            try
            {
                switch (_inscripciones.Inscribir(actividadId, actor.RegistroId))
                {
                    case ResultadoInscripcion.Exitosa:
                        return ResultadoOperacion.Exito("Inscripción realizada correctamente.", actividadId);
                    case ResultadoInscripcion.ActividadNoExiste:
                        return ResultadoOperacion.Falla("La actividad no existe.");
                    case ResultadoInscripcion.ActividadNoDisponible:
                        return ResultadoOperacion.Falla("La actividad no está disponible para inscripciones.");
                    case ResultadoInscripcion.YaInscrito:
                        return ResultadoOperacion.Falla("Ya está inscrito en esta actividad.");
                    case ResultadoInscripcion.CupoAgotado:
                        return ResultadoOperacion.Falla("No quedan cupos disponibles en esta actividad.");
                    default:
                        return ResultadoOperacion.Falla("No fue posible completar la inscripción.");
                }
            }
            catch (AccesoDatosException)
            {
                return ResultadoOperacion.Falla("No fue posible completar la inscripción. Intente nuevamente.");
            }
        }

        public ResultadoOperacion Cancelar(int actividadId, UsuarioSesion actor)
        {
            if (actor == null)
            {
                return ResultadoOperacion.Falla("Debe iniciar sesión para cancelar una inscripción.");
            }

            try
            {
                return _inscripciones.Cancelar(actividadId, actor.RegistroId)
                    ? ResultadoOperacion.Exito("Inscripción cancelada. El cupo quedó disponible.", actividadId)
                    : ResultadoOperacion.Falla("No tiene una inscripción activa en esta actividad.");
            }
            catch (AccesoDatosException)
            {
                return ResultadoOperacion.Falla("No fue posible cancelar la inscripción. Intente nuevamente.");
            }
        }

        /// <summary>Inscripciones de la persona autenticada.</summary>
        public List<InscripcionDto> ListarMisInscripciones(UsuarioSesion actor)
        {
            if (actor == null)
            {
                throw new ReglaNegocioException("Debe iniciar sesión.");
            }

            return _inscripciones.ListarPorParticipante(actor.RegistroId);
        }

        /// <summary>Personas inscritas en una actividad. Solo organizadores.</summary>
        public List<InscripcionDto> ListarInscritos(int actividadId, UsuarioSesion actor)
        {
            if (actor == null || !actor.EsOrganizador)
            {
                throw new ReglaNegocioException("No tiene permisos para ver los inscritos.");
            }

            return _inscripciones.ListarPorActividad(actividadId);
        }

        /// <summary>Indica si la actividad admite una nueva inscripción de esta persona (para mostrar u ocultar el botón).</summary>
        public static bool PuedeInscribirse(ActividadDto actividad, bool yaInscrito)
        {
            return actividad != null
                && !yaInscrito
                && (actividad.Estado == "Activa" || actividad.Estado == "Próxima")
                && actividad.FechaFin.HasValue && actividad.FechaFin.Value.Date >= DateTime.Today
                && actividad.CuposDisponibles > 0;
        }
    }
}
