using System;
using ProyectoParcialWebForms.Datos;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Negocio
{
    /// <summary>
    /// Valida a una persona registrada con su identificación y su correo y devuelve el usuario
    /// de la sesión, con su rol. El rol viene de la base de datos: nunca se toma de un formulario.
    /// </summary>
    public class AutenticacionService
    {
        private readonly RegistroRepository _repositorio = new RegistroRepository();

        public UsuarioSesion Ingresar(string identificacion, string correo)
        {
            identificacion = (identificacion ?? string.Empty).Trim();
            correo = (correo ?? string.Empty).Trim();

            if (identificacion.Length == 0 || correo.Length == 0)
            {
                throw new ReglaNegocioException("Ingrese su identificación y su correo.");
            }

            Registro registro = _repositorio.ObtenerPorIdentificacion(identificacion);

            // Mensaje único: no se revela si falló la identificación o el correo.
            if (registro == null || !string.Equals(registro.Correo, correo, StringComparison.OrdinalIgnoreCase))
            {
                throw new ReglaNegocioException("La identificación o el correo no son correctos.");
            }

            return new UsuarioSesion
            {
                RegistroId = registro.Id,
                Identificacion = registro.Identificacion,
                Nombres = registro.Nombres,
                Apellidos = registro.Apellidos,
                Rol = registro.Rol == UsuarioSesion.RolOrganizador ? UsuarioSesion.RolOrganizador : UsuarioSesion.RolParticipante
            };
        }
    }
}
