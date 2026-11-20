using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ProyectoParcialWebForms.Datos;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Negocio
{
    /// <summary>
    /// Servicio de negocio de participantes. Aplica las reglas del sistema y decide cuándo llamar a la capa de datos.
    /// No conoce controles de la página ni SQL.
    /// </summary>
    public class RegistroService
    {
        private static readonly Regex ReIdentificacion = new Regex(@"^[0-9]{5,12}$");
        private static readonly Regex ReCorreo = new Regex(@"^[\w\.-]+@[\w\.-]+\.\w{2,}$");
        private static readonly Regex ReTelefono = new Regex(@"^[0-9]{7,10}$");

        private readonly RegistroRepository _repositorio = new RegistroRepository();

        /// <summary>Valida y registra a una persona nueva. Devuelve el código generado.</summary>
        public string Registrar(Registro registro)
        {
            Normalizar(registro);
            Validar(registro);
            return _repositorio.Insertar(registro);
        }

        /// <summary>Valida y actualiza un registro existente (registro.Id obligatorio). Devuelve su código.</summary>
        public string Actualizar(Registro registro)
        {
            if (registro.Id <= 0)
            {
                throw new ReglaNegocioException("No se indicó qué registro se debe actualizar.");
            }

            Normalizar(registro);
            Validar(registro);

            string codigo = _repositorio.Actualizar(registro);
            if (codigo == null)
            {
                throw new RegistroNoEncontradoException();
            }
            return codigo;
        }

        /// <summary>
        /// Busca un registro para editarlo. Exige la identificación Y el correo registrado, para que
        /// nadie pueda ver o modificar el registro de otra persona solo conociendo su documento.
        /// Devuelve null si no hay coincidencia (sin revelar cuál de los dos datos falló).
        /// </summary>
        public Registro BuscarParaEditar(string identificacion, string correo)
        {
            identificacion = (identificacion ?? string.Empty).Trim();
            correo = (correo ?? string.Empty).Trim();

            if (!ReIdentificacion.IsMatch(identificacion))
            {
                throw new ReglaNegocioException("Ingrese un número de identificación válido (5 a 12 dígitos).");
            }

            if (!ReCorreo.IsMatch(correo))
            {
                throw new ReglaNegocioException("Ingrese el correo electrónico con el que se registró.");
            }

            Registro registro = _repositorio.ObtenerPorIdentificacion(identificacion);

            if (registro == null || !string.Equals(registro.Correo, correo, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return registro;
        }

        /// <summary>Lista de participantes. Solo para organizadores.</summary>
        public List<Registro> Listar(UsuarioSesion actor)
        {
            if (actor == null || !actor.EsOrganizador)
            {
                throw new ReglaNegocioException("No tiene permisos para consultar los participantes.");
            }

            return _repositorio.Listar();
        }

        // ------------------------------------------------------------
        private static void Normalizar(Registro r)
        {
            r.Identificacion = (r.Identificacion ?? string.Empty).Trim();
            r.Nombres = (r.Nombres ?? string.Empty).Trim();
            r.Apellidos = (r.Apellidos ?? string.Empty).Trim();
            r.Correo = (r.Correo ?? string.Empty).Trim();
            r.Telefono = (r.Telefono ?? string.Empty).Trim();
            r.Programa = (r.Programa ?? string.Empty).Trim();
            r.Semestre = (r.Semestre ?? string.Empty).Trim();
            r.Participacion = (r.Participacion ?? string.Empty).Trim();
            r.Jornada = (r.Jornada ?? string.Empty).Trim();
            r.Temas = (r.Temas ?? string.Empty).Trim();
            r.Comentarios = (r.Comentarios ?? string.Empty).Trim();
        }

        private static void Validar(Registro r)
        {
            if (!ReIdentificacion.IsMatch(r.Identificacion))
                throw new ReglaNegocioException("La identificación debe tener entre 5 y 12 dígitos numéricos.");

            if (r.Nombres.Length == 0 || r.Nombres.Length > 80)
                throw new ReglaNegocioException("Los nombres son obligatorios (máximo 80 caracteres).");

            if (r.Apellidos.Length == 0 || r.Apellidos.Length > 80)
                throw new ReglaNegocioException("Los apellidos son obligatorios (máximo 80 caracteres).");

            if (r.Correo.Length > 120 || !ReCorreo.IsMatch(r.Correo))
                throw new ReglaNegocioException("El correo electrónico no es válido.");

            if (!ReTelefono.IsMatch(r.Telefono))
                throw new ReglaNegocioException("El teléfono debe tener entre 7 y 10 dígitos.");

            if (r.Programa.Length == 0)
                throw new ReglaNegocioException("Debe seleccionar un programa.");

            if (r.Semestre.Length == 0)
                throw new ReglaNegocioException("Debe seleccionar el semestre.");

            if (r.Participacion.Length == 0)
                throw new ReglaNegocioException("Debe seleccionar el tipo de participación.");

            if (r.Jornada.Length == 0)
                throw new ReglaNegocioException("Debe seleccionar la jornada.");

            if (r.Temas.Length == 0)
                throw new ReglaNegocioException("Debe seleccionar al menos un tema de interés.");

            if (r.Temas.Length > 500)
                throw new ReglaNegocioException("La lista de temas es demasiado larga.");

            if (r.Comentarios.Length > 400)
                throw new ReglaNegocioException("Los comentarios no pueden superar 400 caracteres.");
        }
    }
}
