# Gestión de Actividades Académicas

Aplicación web para registrar y administrar actividades académicas, inscribir participantes con control de cupos y recoger valoraciones. Desarrollada con **C#, ASP.NET Web Forms (.NET Framework 4.8.1), SQL Server y ADO.NET**, con **arquitectura de tres capas** y patrón **MVP** en toda la capa de presentación.

- **Asignatura:** Desarrollo de Aplicaciones Web II · Corporación Universitaria Americana
- **Docente:** Ingeniera Cindy Fuentes
- **Integrantes:** [Nombre del estudiante]  *(completar)*
- **Entrega:** Final (100 %)

## Arquitectura

```
Web (Presentación + MVP)  ->  Negocio  ->  Datos  ->  SQL Server
        |                        |           |
        +---------- Entidades (DTO, resultados y excepciones compartidos) ----------+
```

| Proyecto | Responsabilidad | Contenido principal |
|---|---|---|
| `ProyectoParcialWebForms` (Web) | Interfaz, eventos y sesión | 6 páginas `.aspx`, contratos `Views/I...View`, `Presenters/...Presenter`, `Seguridad/SesionWeb` |
| `ProyectoParcialWebForms.Negocio` | Reglas, validaciones y permisos | `ActividadService`, `InscripcionService`, `ResenaService`, `ReporteService`, `AutenticacionService`, `RegistroService` |
| `ProyectoParcialWebForms.Datos` | Acceso a SQL Server con ADO.NET | `ActividadRepository`, `InscripcionRepository`, `ResenaRepository`, `TipoActividadRepository`, `ReporteRepository`, `RegistroRepository` |
| `ProyectoParcialWebForms.Entidades` | Objetos compartidos | DTO, `ResultadoOperacion`, `UsuarioSesion`, excepciones propias |

Referencias: **Web -> Negocio y Entidades**, **Negocio -> Datos y Entidades**, **Datos -> Entidades**. La capa de datos no depende de la web y ninguna página ejecuta SQL.

**MVP:** cada página es una *vista* que implementa su contrato (`IActividadesView`, `ICatalogoView`, `IRegistroView`, `IAccesoView`, `IReportesView`, `IListaRegistrosView`); su *Presenter* coordina las acciones y llama a los servicios; los DTO son el *modelo*.

Diagramas en `Documentacion/`: `Diagrama_Arquitectura_3Capas_MVP.png` y `Diagrama_ER_Actualizado.png`.

## Funcionalidades

- [x] Registrar actividades (título, descripción, tipo, fechas, horarios, modalidad, lugar, cupo y estado)
- [x] Listar y buscar por título/lugar, tipo, estado y rango de fechas
- [x] Consultar el detalle de una actividad
- [x] Actualizar y desactivar actividades (eliminación lógica)
- [x] Inscripción con control de cupos en una transacción (sin duplicados ni sobrecupo)
- [x] Cancelación de inscripciones y listado «Mis inscripciones»
- [x] Reseñas y valoraciones (1 a 5) solo de personas inscritas, una por actividad
- [x] Reportes: totales generales y ocupación/valoración por actividad
- [x] Registro de participantes con identificación única y edición verificada con identificación + correo
- [x] Acceso con sesión y roles (Participante / Organizador)

## Seguridad y reglas

- Consultas **parametrizadas con tipos** en toda la capa de datos.
- **Validaciones en el servidor** (capa de negocio), además de las de la página.
- El usuario que inscribe, reseña o administra **sale de la sesión del servidor**, nunca de un control de la página.
- El **rol** proviene de la base de datos (columna `Rol`); no se puede elegir desde un formulario.
- Permisos en el servicio: gestionar actividades, reportes y listado de participantes son solo para organizadores.
- Texto del usuario siempre codificado al mostrarse (`Server.HtmlEncode`). Mensajes de error sin detalles técnicos.
- La cadena de conexión usa autenticación de Windows: **no contiene usuarios ni contraseñas**.
- Limitación conocida: el ingreso usa identificación + correo registrado (sin contraseña). Es suficiente para el alcance académico; en producción se usaría autenticación con contraseña o proveedor de identidad.

## Requisitos

- Windows con **Visual Studio 2022 o superior** (carga de trabajo *Desarrollo ASP.NET y web*)
- **.NET Framework 4.8.1** (paquete de destino; Visual Studio ofrece instalarlo)
- **SQL Server** (Express, Developer o LocalDB) y **SQL Server Management Studio**
- Conexión a internet la primera vez (NuGet restaura los paquetes solo)

## Configuración y ejecución

1. Abrir `ProyectoParcialWebForms.slnx` en Visual Studio.
2. Abrir `Database/BaseDatos.sql` en SSMS y ejecutarlo. Evoluciona la base `RegistroDB` (crea lo que falta y carga datos de prueba); se puede ejecutar varias veces.
3. En `ProyectoParcialWebForms/Web.config` ajustar `Data Source` al servidor propio:

   | Instalación | Data Source |
   |---|---|
   | Instancia predeterminada | `localhost` |
   | SQL Server Express | `.\SQLEXPRESS` |
   | LocalDB | `(localdb)\MSSQLLocalDB` |
4. **Compilar > Recompilar solución** y ejecutar con **F5**.

## Usuarios de prueba (datos ficticios)

| Rol | Identificación | Correo |
|---|---|---|
| Organizador | `1000000003` | `maria.torres@correo.com` |
| Participante | `1000000001` | `laura.martinez@correo.com` |
| Participante | `1000000002` | `carlos.gomez@correo.com` |

## Páginas

| Ruta | Acceso | Descripción |
|---|---|---|
| `/Default` | Público | Registro de participantes y edición verificada |
| `/Acceso` | Público | Ingreso con identificación y correo |
| `/Catalogo` | Sesión iniciada | Listado con filtros, detalle, inscripción, reseñas y «Mis inscripciones» |
| `/Actividades` | Organizador | Gestión de actividades (registrar, editar, desactivar, filtrar) |
| `/Reportes` | Organizador | Resumen general y reporte por actividad |
| `/ListaRegistros` | Organizador | Participantes registrados |

## Datos de prueba de la base

5 actividades, 3 participantes, inscripciones al «Taller de Programación Web» y al «Encuentro de Investigación» (finalizado) y 2 reseñas.

## Pruebas funcionales

Ver el informe técnico (`01_Informe_Tecnico.pdf`): casos válidos, inválidos y límite para actividades, inscripciones (cupo agotado, duplicado, cancelación), reseñas (sin inscripción, duplicada, calificación fuera de rango), permisos por rol, inyección SQL y XSS.

## Estructura del repositorio

```
ProyectoParcialWebForms.slnx
README.md
Database/BaseDatos.sql
Documentacion/                      Diagramas de arquitectura y ER
ProyectoParcialWebForms/            Web (presentación + MVP + sesión)
ProyectoParcialWebForms.Negocio/    Servicios y reglas
ProyectoParcialWebForms.Datos/      Repositorios ADO.NET
ProyectoParcialWebForms.Entidades/  DTO, resultados y excepciones
```
