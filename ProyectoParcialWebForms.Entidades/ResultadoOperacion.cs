using System.Collections.Generic;

namespace ProyectoParcialWebForms.Entidades
{
    /// <summary>
    /// Resultado comprensible de una operación de negocio: éxito con mensaje,
    /// o una lista de errores que la vista puede mostrar tal cual.
    /// </summary>
    public class ResultadoOperacion
    {
        public ResultadoOperacion()
        {
            Errores = new List<string>();
        }

        public bool Exitoso
        {
            get { return Errores.Count == 0; }
        }

        public string Mensaje { get; set; }
        public int Id { get; set; }
        public List<string> Errores { get; private set; }

        public static ResultadoOperacion Exito(string mensaje, int id = 0)
        {
            return new ResultadoOperacion { Mensaje = mensaje, Id = id };
        }

        public static ResultadoOperacion Falla(string error)
        {
            ResultadoOperacion r = new ResultadoOperacion();
            r.Errores.Add(error);
            return r;
        }

        public static ResultadoOperacion Falla(IEnumerable<string> errores)
        {
            ResultadoOperacion r = new ResultadoOperacion();
            r.Errores.AddRange(errores);
            return r;
        }
    }
}
