using System;
using System.Collections.Generic;
using ProyectoParcialWebForms.Datos;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Negocio
{
    /// <summary>
    /// Reglas de reseñas: solo puede reseñar una persona autenticada que tenga una inscripción
    /// activa en la actividad, cuando esta ya inició; una sola reseña por persona y actividad;
    /// calificación entera de 1 a 5.
    /// </summary>
    public class ResenaService
    {
        private const int ComentarioMaximo = 1000;

        private readonly ActividadRepository _actividades = new ActividadRepository();
        private readonly InscripcionRepository _inscripciones = new InscripcionRepository();
        private readonly ResenaRepository _resenas = new ResenaRepository();

        public ResultadoOperacion Publicar(int actividadId, int calificacion, string comentario, UsuarioSesion actor)
        {
            if (actor == null)
            {
                return ResultadoOperacion.Falla("Debe iniciar sesión para publicar una reseña.");
            }

            List<string> errores = new List<string>();
            comentario = (comentario ?? string.Empty).Trim();

            if (calificacion < 1 || calificacion > 5) errores.Add("La calificación debe ser un número entero de 1 a 5.");
            if (comentario.Length > ComentarioMaximo) errores.Add("El comentario no puede superar " + ComentarioMaximo + " caracteres.");
            if (errores.Count > 0)
            {
                return ResultadoOperacion.Falla(errores);
            }

            try
            {
                ActividadDto actividad = _actividades.ObtenerPorId(actividadId);
                if (actividad == null || actividad.Estado == "Inactiva")
                {
                    return ResultadoOperacion.Falla("La actividad no existe.");
                }

                if (!_inscripciones.TieneInscripcionActiva(actividadId, actor.RegistroId))
                {
                    return ResultadoOperacion.Falla("Solo las personas inscritas en la actividad pueden reseñarla.");
                }

                if (!ActividadYaInicio(actividad))
                {
                    return ResultadoOperacion.Falla("Podrá reseñar la actividad cuando haya iniciado.");
                }

                _resenas.Insertar(new ResenaDto
                {
                    ActividadId = actividadId,
                    RegistroId = actor.RegistroId,
                    Calificacion = calificacion,
                    Comentario = comentario
                });

                return ResultadoOperacion.Exito("Reseña publicada. ¡Gracias por su valoración!", actividadId);
            }
            catch (ResenaDuplicadaException ex)
            {
                return ResultadoOperacion.Falla(ex.Message);
            }
            catch (AccesoDatosException)
            {
                return ResultadoOperacion.Falla("No fue posible publicar la reseña. Intente nuevamente.");
            }
        }

        public List<ResenaDto> ListarPorActividad(int actividadId)
        {
            return actividadId <= 0 ? new List<ResenaDto>() : _resenas.ListarPorActividad(actividadId);
        }

        /// <summary>Indica si se debe mostrar el formulario de reseña a esta persona.</summary>
        public static bool PuedeResenar(ActividadDto actividad, bool inscrito, bool yaReseno)
        {
            return actividad != null && inscrito && !yaReseno && actividad.Estado != "Inactiva" && ActividadYaInicio(actividad);
        }

        private static bool ActividadYaInicio(ActividadDto a)
        {
            return a.Estado == "Finalizada" || (a.FechaInicio.HasValue && a.FechaInicio.Value.Date <= DateTime.Today);
        }
    }
}
