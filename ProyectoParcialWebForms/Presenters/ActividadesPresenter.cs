using System;
using System.Collections.Generic;
using System.Globalization;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Negocio;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms.Presenters
{
    /// <summary>
    /// Presenter de la página de actividades. Coordina: recibe las acciones de la vista,
    /// convierte los datos del formulario, llama al servicio de negocio y le indica a la vista
    /// qué mostrar. No contiene SQL ni referencias a controles.
    /// </summary>
    public class ActividadesPresenter
    {
        private readonly IActividadesView _vista;
        private readonly ActividadService _servicio;

        public ActividadesPresenter(IActividadesView vista)
        {
            _vista = vista;
            _servicio = new ActividadService();
        }

        /// <summary>Primera carga de la página: tipos y listado.</summary>
        public void Inicializar()
        {
            try
            {
                _vista.MostrarTipos(_servicio.ListarTipos());
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible cargar los tipos de actividad. Revise la conexión a la base de datos.", true);
                return;
            }

            CargarListado();
        }

        /// <summary>Guardar: registra una actividad nueva o actualiza la que se está editando.</summary>
        public void Guardar()
        {
            List<string> erroresFormato = new List<string>();
            ActividadDto dto = Convertir(_vista.ObtenerFormulario(), erroresFormato);

            if (erroresFormato.Count > 0)
            {
                _vista.MostrarErrores(erroresFormato);
                return;
            }

            ResultadoOperacion resultado = _servicio.Guardar(dto, _vista.UsuarioActual);

            if (!resultado.Exitoso)
            {
                _vista.MostrarErrores(resultado.Errores);
                return;
            }

            _vista.LimpiarFormulario();
            _vista.MostrarMensaje(resultado.Mensaje, false);
            CargarListado();
        }

        public void Buscar()
        {
            CargarListado();
        }

        public void Limpiar()
        {
            _vista.LimpiarFormulario();
        }

        /// <summary>Carga una actividad en el formulario para modificarla.</summary>
        public void Editar(int actividadId)
        {
            try
            {
                ActividadDto actividad = _servicio.ObtenerPorId(actividadId);

                if (actividad == null)
                {
                    _vista.MostrarMensaje("La actividad seleccionada ya no existe.", true);
                    CargarListado();
                    return;
                }

                _vista.MostrarEnFormulario(actividad);
                _vista.MostrarMensaje("Editando la actividad: " + actividad.Titulo + ". Modifique los datos y presione Actualizar.", false);
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible consultar la actividad. Intente nuevamente.", true);
            }
        }

        public void Desactivar(int actividadId)
        {
            ResultadoOperacion resultado = _servicio.Desactivar(actividadId, _vista.UsuarioActual);

            if (!resultado.Exitoso)
            {
                _vista.MostrarErrores(resultado.Errores);
                return;
            }

            _vista.MostrarMensaje(resultado.Mensaje, false);
            CargarListado();
        }

        // ------------------------------------------------------------
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

            if (desde.HasValue && hasta.HasValue && hasta.Value < desde.Value)
            {
                _vista.MostrarMensaje("La fecha «hasta» no puede ser anterior a la fecha «desde».", true);
                return;
            }

            try
            {
                _vista.MostrarActividades(_servicio.Buscar(_vista.FiltroTexto, _vista.FiltroTipoId, _vista.FiltroEstado, desde, hasta));
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarMensaje("No fue posible cargar las actividades. Intente nuevamente.", true);
            }
        }

        /// <summary>Convierte el texto del formulario en un DTO. Los campos vacíos quedan nulos y los valida el negocio.</summary>
        private static ActividadDto Convertir(ActividadFormulario f, List<string> errores)
        {
            ActividadDto dto = new ActividadDto();

            int id;
            dto.ActividadId = int.TryParse(f.ActividadId, out id) ? id : 0;

            int tipoId;
            dto.TipoId = int.TryParse(f.TipoId, out tipoId) ? tipoId : 0;

            dto.Titulo = f.Titulo;
            dto.Descripcion = f.Descripcion;
            dto.Modalidad = f.Modalidad;
            dto.Lugar = f.Lugar;
            dto.Estado = f.Estado;

            dto.FechaInicio = LeerFecha(f.FechaInicio, "fecha de inicio", errores);
            dto.FechaFin = LeerFecha(f.FechaFin, "fecha de fin", errores);
            dto.HoraInicio = LeerHora(f.HoraInicio, "hora de inicio", errores);
            dto.HoraFin = LeerHora(f.HoraFin, "hora de fin", errores);

            string cupoTexto = (f.Cupo ?? string.Empty).Trim();
            if (cupoTexto.Length == 0)
            {
                dto.Cupo = 0;
            }
            else
            {
                int cupo;
                if (int.TryParse(cupoTexto, NumberStyles.Integer, CultureInfo.InvariantCulture, out cupo))
                {
                    dto.Cupo = cupo;
                }
                else
                {
                    errores.Add("El cupo debe ser un número entero.");
                }
            }

            return dto;
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

        private static TimeSpan? LeerHora(string texto, string nombre, List<string> errores)
        {
            texto = (texto ?? string.Empty).Trim();
            if (texto.Length == 0) return null;

            TimeSpan hora;
            if (TimeSpan.TryParse(texto, CultureInfo.InvariantCulture, out hora) && hora >= TimeSpan.Zero && hora < TimeSpan.FromHours(24))
            {
                return hora;
            }

            errores.Add("La " + nombre + " no tiene un formato válido.");
            return null;
        }
    }
}
