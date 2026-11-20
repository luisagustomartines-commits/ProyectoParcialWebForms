using System.Collections.Generic;
using ProyectoParcialWebForms.Datos;
using ProyectoParcialWebForms.Entidades;

namespace ProyectoParcialWebForms.Negocio
{
    /// <summary>Reportes y resúmenes. Solo para organizadores.</summary>
    public class ReporteService
    {
        private readonly ReporteRepository _reportes = new ReporteRepository();

        public ResumenGeneralDto ObtenerResumen(UsuarioSesion actor)
        {
            Verificar(actor);
            return _reportes.ObtenerResumen();
        }

        public List<ReporteActividadDto> ObtenerPorActividad(UsuarioSesion actor)
        {
            Verificar(actor);
            return _reportes.ObtenerPorActividad();
        }

        private static void Verificar(UsuarioSesion actor)
        {
            if (actor == null || !actor.EsOrganizador)
            {
                throw new ReglaNegocioException("No tiene permisos para consultar los reportes.");
            }
        }
    }
}
