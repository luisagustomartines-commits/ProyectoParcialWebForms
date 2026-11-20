using System;

namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>Totales generales para el reporte.</summary>
    public class ResumenGeneralDto
    {
        public int TotalActividades { get; set; }
        public int ActividadesActivas { get; set; }
        public int ActividadesProximas { get; set; }
        public int ActividadesFinalizadas { get; set; }
        public int TotalParticipantes { get; set; }
        public int InscripcionesActivas { get; set; }
        public int TotalResenas { get; set; }
        public double PromedioGeneral { get; set; }
    }

    /// <summary>Una fila del reporte por actividad.</summary>
    public class ReporteActividadDto
    {
        public int ActividadId { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string Estado { get; set; }
        public int Cupo { get; set; }
        public int Inscritos { get; set; }
        public int TotalResenas { get; set; }
        public double Promedio { get; set; }

        public int Disponibles
        {
            get { return Math.Max(0, Cupo - Inscritos); }
        }

        public int OcupacionPorcentaje
        {
            get { return Cupo > 0 ? (int)Math.Round(100.0 * Inscritos / Cupo) : 0; }
        }
    }
}
