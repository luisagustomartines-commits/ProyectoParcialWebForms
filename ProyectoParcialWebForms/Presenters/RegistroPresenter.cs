using System.Collections.Generic;
using System.Text;
using ProyectoParcialWebForms.Entidades;
using ProyectoParcialWebForms.Negocio;
using ProyectoParcialWebForms.Views;

namespace ProyectoParcialWebForms.Presenters
{
    /// <summary>Coordina el formulario de participantes: registrar, buscar y actualizar.</summary>
    public class RegistroPresenter
    {
        private readonly IRegistroView _vista;
        private readonly RegistroService _servicio = new RegistroService();

        public RegistroPresenter(IRegistroView vista)
        {
            _vista = vista;
        }

        public void Registrar()
        {
            _vista.MostrarError(string.Empty);

            RegistroFormulario f = _vista.ObtenerFormulario();
            Registro registro = new Registro
            {
                Identificacion = f.Identificacion,
                Nombres = f.Nombres,
                Apellidos = f.Apellidos,
                Correo = f.Correo,
                Telefono = f.Telefono,
                Programa = f.Programa,
                Semestre = f.Semestre,
                Participacion = f.Participacion,
                Jornada = f.Jornada,
                Temas = string.Join(", ", f.Temas ?? new List<string>()),
                Comentarios = f.Comentarios
            };

            bool editando = _vista.RegistroEditandoId.HasValue;
            string codigo;

            try
            {
                if (editando)
                {
                    registro.Id = _vista.RegistroEditandoId.Value;
                    codigo = _servicio.Actualizar(registro);
                }
                else
                {
                    codigo = _servicio.Registrar(registro);
                }
            }
            catch (IdentificacionDuplicadaException)
            {
                _vista.MostrarError("Esa identificación ya está registrada. Use «Consulta y edita tu registro» (con su identificación y su correo) para modificar sus respuestas.");
                return;
            }
            catch (RegistroNoEncontradoException ex)
            {
                _vista.LimpiarFormulario();
                _vista.MostrarError(ex.Message);
                return;
            }
            catch (ReglaNegocioException ex)
            {
                _vista.MostrarError(ex.Message);
                return;
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarError("No fue posible guardar el registro. Revise la conexión con la base de datos.");
                return;
            }

            string mensaje = "Hola, " + registro.Nombres + " " + registro.Apellidos + ". " +
                (editando
                    ? "Tus respuestas fueron actualizadas correctamente."
                    : "Tu participación fue registrada correctamente. Para editar tus respuestas más adelante, búscalas con tu identificación y tu correo.");

            StringBuilder resumen = new StringBuilder();
            resumen.Append("Identificación: ").Append(registro.Identificacion);
            resumen.Append(" · Programa: ").Append(registro.Programa);
            resumen.Append(" · Semestre: ").Append(registro.Semestre);
            resumen.Append(" · Participación: ").Append(registro.Participacion);
            resumen.Append(" · Jornada: ").Append(registro.Jornada);
            resumen.Append(" · Temas: ").Append(registro.Temas);
            if (!string.IsNullOrEmpty(registro.Comentarios))
            {
                resumen.Append(" · Comentarios: ").Append(registro.Comentarios);
            }

            _vista.LimpiarFormulario();
            _vista.MostrarResultado(mensaje, "Código de registro: " + codigo, resumen.ToString());
        }

        public void Buscar()
        {
            _vista.MostrarError(string.Empty);
            _vista.OcultarResultado();

            try
            {
                Registro registro = _servicio.BuscarParaEditar(_vista.BuscarIdentificacion, _vista.BuscarCorreo);

                if (registro == null)
                {
                    _vista.MostrarBusqueda("No se encontró un registro con esa identificación y ese correo.", false);
                    return;
                }

                _vista.MostrarRegistroEnFormulario(registro);
                _vista.MostrarBusqueda("Registro encontrado. Modifica tus respuestas y presiona ACTUALIZAR REGISTRO.", true);
            }
            catch (ReglaNegocioException ex)
            {
                _vista.MostrarBusqueda(ex.Message, false);
            }
            catch (AccesoDatosException)
            {
                _vista.MostrarBusqueda("No fue posible consultar la base de datos. Revise la conexión.", false);
            }
        }

        public void Limpiar()
        {
            _vista.LimpiarFormulario();
            _vista.OcultarResultado();
        }
    }
}
