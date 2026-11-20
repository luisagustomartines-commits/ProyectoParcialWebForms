namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>Resultado de intentar inscribir a una persona en una actividad.</summary>
    public enum ResultadoInscripcion
    {
        Exitosa = 0,
        ActividadNoExiste = 1,
        ActividadNoDisponible = 2,
        YaInscrito = 3,
        CupoAgotado = 4
    }
}
