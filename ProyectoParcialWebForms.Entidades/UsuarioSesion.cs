using System;

namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>
    /// Usuario autenticado. Se guarda en la sesión del servidor: el identificador de quien
    /// inscribe, reseña o administra siempre sale de aquí y nunca de un control de la página.
    /// </summary>
    [Serializable]
    public class UsuarioSesion
    {
        public const string RolOrganizador = "Organizador";
        public const string RolParticipante = "Participante";

        public int RegistroId { get; set; }
        public string Identificacion { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Rol { get; set; }

        public string NombreCompleto
        {
            get { return ((Nombres ?? string.Empty) + " " + (Apellidos ?? string.Empty)).Trim(); }
        }

        public bool EsOrganizador
        {
            get { return Rol == RolOrganizador; }
        }
    }
}
