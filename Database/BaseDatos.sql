/* =====================================================================
   Proyecto : Gestión de actividades académicas (ASP.NET Web Forms + ADO.NET)
   Versión  : ENTREGA FINAL (incluye inscripciones, reseñas y roles)
   Motor    : SQL Server
   Archivo  : BaseDatos.sql  (entrega: 03_BaseDatos.sql)
   Uso      : abrir en SQL Server Management Studio y presionar Ejecutar (F5).
              Se puede ejecutar varias veces: no duplica tablas ni datos.
   Nota     : se EVOLUCIONA la base existente RegistroDB (tabla Registros). No se crea
              otra base ni otra tabla de personas; solo se agregan las entidades
              que faltaban: TiposActividad, Actividades, Inscripciones y Resenas, y la
              columna Rol en Registros (para distinguir organizadores de participantes).
   ===================================================================== */

IF DB_ID(N'RegistroDB') IS NULL
    CREATE DATABASE RegistroDB;
GO

USE RegistroDB;
GO

/* ---------------------------------------------------------------------
   1. Tabla existente: Registros  (se reutiliza como participante/estudiante)
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.Registros', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Registros(
        Id             INT           NOT NULL IDENTITY(1,1),
        Codigo         VARCHAR(20)   NULL,                      -- EVT-AAAA-#### (se genera a partir del Id)
        Identificacion VARCHAR(12)   NOT NULL,                  -- documento de la persona (único)
        Nombres        VARCHAR(80)   NOT NULL,
        Apellidos      VARCHAR(80)   NOT NULL,
        Correo         VARCHAR(120)  NOT NULL,
        Telefono       VARCHAR(10)   NOT NULL,
        Programa       VARCHAR(100)  NOT NULL,
        Semestre       VARCHAR(10)   NOT NULL,
        Participacion  VARCHAR(30)   NOT NULL,                  -- Ponente / Asistente / Organizador
        Jornada        VARCHAR(20)   NOT NULL,                  -- Mañana / Tarde / Noche
        Temas          VARCHAR(500)  NOT NULL,                  -- temas separados por coma
        Comentarios    VARCHAR(400)  NULL,
        FechaRegistro  DATETIME      NOT NULL CONSTRAINT DF_Registros_Fecha DEFAULT GETDATE(),
        Rol            NVARCHAR(20)  NOT NULL CONSTRAINT DF_Registros_Rol DEFAULT (N'Participante'),
        CONSTRAINT PK_Registros PRIMARY KEY (Id),
        CONSTRAINT UQ_Registros_Identificacion UNIQUE (Identificacion),
        CONSTRAINT CK_Registros_Rol CHECK (Rol IN (N'Participante', N'Organizador'))
    );
END
GO

-- Si la tabla ya existía (versión anterior), se agrega el rol sin perder datos
IF COL_LENGTH(N'dbo.Registros', N'Rol') IS NULL
    ALTER TABLE dbo.Registros ADD Rol NVARCHAR(20) NOT NULL CONSTRAINT DF_Registros_Rol DEFAULT (N'Participante');
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_Registros_Rol')
    ALTER TABLE dbo.Registros ADD CONSTRAINT CK_Registros_Rol CHECK (Rol IN (N'Participante', N'Organizador'));
GO

/* ---------------------------------------------------------------------
   2. Nuevas entidades
   --------------------------------------------------------------------- */
IF OBJECT_ID(N'dbo.TiposActividad', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TiposActividad(
        TipoId      INT           NOT NULL IDENTITY(1,1),
        Nombre      NVARCHAR(60)  NOT NULL,
        Descripcion NVARCHAR(200) NULL,
        CONSTRAINT PK_TiposActividad PRIMARY KEY (TipoId),
        CONSTRAINT UQ_TiposActividad_Nombre UNIQUE (Nombre)
    );
END
GO

IF OBJECT_ID(N'dbo.Actividades', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Actividades(
        ActividadId   INT            NOT NULL IDENTITY(1,1),
        Titulo        NVARCHAR(120)  NOT NULL,
        Descripcion   NVARCHAR(1000) NOT NULL,
        TipoId        INT            NOT NULL,
        FechaInicio   DATE           NOT NULL,
        FechaFin      DATE           NOT NULL,
        HoraInicio    TIME(0)        NOT NULL,
        HoraFin       TIME(0)        NOT NULL,
        Modalidad     NVARCHAR(20)   NOT NULL,
        Lugar         NVARCHAR(150)  NOT NULL,
        Cupo          INT            NOT NULL,
        Estado        NVARCHAR(20)   NOT NULL CONSTRAINT DF_Actividades_Estado DEFAULT (N'Activa'),
        FechaCreacion DATETIME2(0)   NOT NULL CONSTRAINT DF_Actividades_Creacion DEFAULT SYSDATETIME(),
        CONSTRAINT PK_Actividades PRIMARY KEY (ActividadId),
        CONSTRAINT FK_Actividades_Tipo FOREIGN KEY (TipoId) REFERENCES dbo.TiposActividad(TipoId),
        CONSTRAINT UQ_Actividades_Titulo_Fecha UNIQUE (Titulo, FechaInicio),
        CONSTRAINT CK_Actividades_Cupo CHECK (Cupo > 0),
        CONSTRAINT CK_Actividades_Fechas CHECK (FechaFin >= FechaInicio),
        CONSTRAINT CK_Actividades_Horas CHECK (HoraFin > HoraInicio),
        CONSTRAINT CK_Actividades_Modalidad CHECK (Modalidad IN (N'Presencial', N'Virtual', N'Híbrida')),
        CONSTRAINT CK_Actividades_Estado CHECK (Estado IN (N'Activa', N'Próxima', N'Finalizada', N'Inactiva'))
    );
END
GO

-- Inscripciones con control de cupos (se inscribe dentro de una transacción)
IF OBJECT_ID(N'dbo.Inscripciones', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Inscripciones(
        InscripcionId    INT          NOT NULL IDENTITY(1,1),
        ActividadId      INT          NOT NULL,
        RegistroId       INT          NOT NULL,
        FechaInscripcion DATETIME2(0) NOT NULL CONSTRAINT DF_Inscripciones_Fecha DEFAULT SYSDATETIME(),
        Estado           NVARCHAR(20) NOT NULL CONSTRAINT DF_Inscripciones_Estado DEFAULT (N'Activa'),
        CONSTRAINT PK_Inscripciones PRIMARY KEY (InscripcionId),
        CONSTRAINT FK_Inscripciones_Actividad FOREIGN KEY (ActividadId) REFERENCES dbo.Actividades(ActividadId),
        CONSTRAINT FK_Inscripciones_Registro  FOREIGN KEY (RegistroId)  REFERENCES dbo.Registros(Id),
        CONSTRAINT UQ_Inscripciones_Actividad_Registro UNIQUE (ActividadId, RegistroId),
        CONSTRAINT CK_Inscripciones_Estado CHECK (Estado IN (N'Activa', N'Cancelada'))
    );
END
GO

IF OBJECT_ID(N'dbo.Resenas', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Resenas(
        ResenaId     INT           NOT NULL IDENTITY(1,1),
        ActividadId  INT           NOT NULL,
        RegistroId   INT           NOT NULL,
        Calificacion TINYINT       NOT NULL,
        Comentario   NVARCHAR(1000) NULL,
        Fecha        DATETIME2(0)  NOT NULL CONSTRAINT DF_Resenas_Fecha DEFAULT SYSDATETIME(),
        CONSTRAINT PK_Resenas PRIMARY KEY (ResenaId),
        CONSTRAINT FK_Resenas_Actividad FOREIGN KEY (ActividadId) REFERENCES dbo.Actividades(ActividadId),
        CONSTRAINT FK_Resenas_Registro  FOREIGN KEY (RegistroId)  REFERENCES dbo.Registros(Id),
        CONSTRAINT UQ_Resenas_Actividad_Registro UNIQUE (ActividadId, RegistroId),
        CONSTRAINT CK_Resenas_Calificacion CHECK (Calificacion BETWEEN 1 AND 5)
    );
END
GO

/* ---------------------------------------------------------------------
   3. Datos de prueba (ficticios, reproducibles)
   --------------------------------------------------------------------- */
-- Participantes
IF NOT EXISTS (SELECT 1 FROM dbo.Registros WHERE Identificacion = '1000000001')
    INSERT INTO dbo.Registros(Identificacion, Nombres, Apellidos, Correo, Telefono, Programa, Semestre, Participacion, Jornada, Temas, Comentarios)
    VALUES ('1000000001', N'Laura Sofía', N'Martínez Pérez', 'laura.martinez@correo.com', '3001234567',
            N'Ingeniería de Sistemas', '5', 'Asistente', N'Mañana',
            N'Inteligencia Artificial, Desarrollo Web', N'Me interesa conocer aplicaciones reales de IA.');

IF NOT EXISTS (SELECT 1 FROM dbo.Registros WHERE Identificacion = '1000000002')
    INSERT INTO dbo.Registros(Identificacion, Nombres, Apellidos, Correo, Telefono, Programa, Semestre, Participacion, Jornada, Temas, Comentarios)
    VALUES ('1000000002', N'Carlos Andrés', N'Gómez Rivera', 'carlos.gomez@correo.com', '3109876543',
            N'Ingeniería Industrial', '7', 'Ponente', 'Tarde',
            N'Bases de Datos, Cloud Computing', N'');

IF NOT EXISTS (SELECT 1 FROM dbo.Registros WHERE Identificacion = '1000000003')
    INSERT INTO dbo.Registros(Identificacion, Nombres, Apellidos, Correo, Telefono, Programa, Semestre, Participacion, Jornada, Temas, Comentarios)
    VALUES ('1000000003', N'María Fernanda', N'Torres Díaz', 'maria.torres@correo.com', '3205551234',
            N'Psicología', '3', 'Organizador', 'Noche',
            N'Innovación Educativa', N'Puedo apoyar la logística del evento.');
GO

UPDATE dbo.Registros
SET Codigo = 'EVT-' + CAST(YEAR(FechaRegistro) AS VARCHAR(4)) + '-' + RIGHT('0000' + CAST(Id AS VARCHAR(10)), 4)
WHERE Codigo IS NULL;
GO

-- Organizador de prueba (el rol solo se asigna desde la base de datos, nunca desde un formulario)
UPDATE dbo.Registros SET Rol = N'Organizador' WHERE Identificacion = '1000000003' AND Rol <> N'Organizador';
GO

-- Tipos de actividad
INSERT INTO dbo.TiposActividad(Nombre, Descripcion)
SELECT v.Nombre, v.Descripcion
FROM (VALUES
    (N'Taller',      N'Actividad práctica guiada'),
    (N'Conferencia', N'Charla magistral con ponente invitado'),
    (N'Seminario',   N'Sesión de estudio de un tema específico'),
    (N'Curso',       N'Formación de varias sesiones'),
    (N'Encuentro',   N'Reunión académica o de investigación')
) AS v(Nombre, Descripcion)
WHERE NOT EXISTS (SELECT 1 FROM dbo.TiposActividad t WHERE t.Nombre = v.Nombre);
GO

-- Actividades
INSERT INTO dbo.Actividades(Titulo, Descripcion, TipoId, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupo, Estado)
SELECT v.Titulo, v.Descripcion, t.TipoId, v.FechaInicio, v.FechaFin, v.HoraInicio, v.HoraFin, v.Modalidad, v.Lugar, v.Cupo, v.Estado
FROM (VALUES
    (N'Taller de Programación Web',   N'Espacio práctico para reforzar conceptos de programación web con ASP.NET.', N'Taller',      '2026-11-24', '2026-11-25', '08:00', '12:00', N'Presencial', N'Laboratorio 3',      30,  N'Activa'),
    (N'Conferencia de IA',            N'Panorama actual de la inteligencia artificial y sus aplicaciones.',          N'Conferencia', '2026-12-02', '2026-12-02', '09:00', '11:00', N'Virtual',    N'Plataforma Teams',   100, N'Próxima'),
    (N'Seminario de Bases de Datos',  N'Modelado relacional, consultas y buenas prácticas con SQL Server.',          N'Seminario',   '2026-12-09', '2026-12-09', '14:00', '17:00', N'Híbrida',    N'Auditorio 1',       50,  N'Próxima'),
    (N'Curso de Diseño UX/UI',        N'Fundamentos de diseño de interfaces centrado en el usuario.',                N'Curso',       '2026-12-15', '2026-12-17', '08:00', '12:00', N'Presencial', N'Sala de diseño 5',   40,  N'Próxima'),
    (N'Encuentro de Investigación',   N'Presentación de avances de los semilleros de investigación.',                N'Encuentro',   '2026-10-15', '2026-10-15', '08:00', '16:00', N'Presencial', N'Auditorio principal', 80,  N'Finalizada')
) AS v(Titulo, Descripcion, Tipo, FechaInicio, FechaFin, HoraInicio, HoraFin, Modalidad, Lugar, Cupo, Estado)
INNER JOIN dbo.TiposActividad t ON t.Nombre = v.Tipo
WHERE NOT EXISTS (SELECT 1 FROM dbo.Actividades a WHERE a.Titulo = v.Titulo AND a.FechaInicio = CAST(v.FechaInicio AS DATE));
GO

-- Inscripciones de ejemplo
INSERT INTO dbo.Inscripciones(ActividadId, RegistroId)
SELECT a.ActividadId, r.Id
FROM dbo.Actividades a
INNER JOIN dbo.Registros r ON r.Identificacion IN ('1000000001', '1000000002', '1000000003')
WHERE a.Titulo = N'Taller de Programación Web'
  AND NOT EXISTS (SELECT 1 FROM dbo.Inscripciones i WHERE i.ActividadId = a.ActividadId AND i.RegistroId = r.Id);
GO

INSERT INTO dbo.Inscripciones(ActividadId, RegistroId)
SELECT a.ActividadId, r.Id
FROM dbo.Actividades a
INNER JOIN dbo.Registros r ON r.Identificacion IN ('1000000001', '1000000002')
WHERE a.Titulo = N'Encuentro de Investigación'
  AND NOT EXISTS (SELECT 1 FROM dbo.Inscripciones i WHERE i.ActividadId = a.ActividadId AND i.RegistroId = r.Id);
GO

-- Reseñas de ejemplo (actividad ya finalizada, de personas inscritas)
INSERT INTO dbo.Resenas(ActividadId, RegistroId, Calificacion, Comentario)
SELECT a.ActividadId, r.Id, v.Calificacion, v.Comentario
FROM (VALUES
    ('1000000001', 5, N'Excelente organización y muy buenas ponencias.'),
    ('1000000002', 4, N'Buen espacio para compartir avances de investigación.')
) AS v(Identificacion, Calificacion, Comentario)
INNER JOIN dbo.Registros r ON r.Identificacion = v.Identificacion
INNER JOIN dbo.Actividades a ON a.Titulo = N'Encuentro de Investigación'
WHERE NOT EXISTS (SELECT 1 FROM dbo.Resenas x WHERE x.ActividadId = a.ActividadId AND x.RegistroId = r.Id);
GO

/* ---------------------------------------------------------------------
   4. Consultas de comprobación (ejecutar para las capturas)
   --------------------------------------------------------------------- */
-- SELECT * FROM dbo.TiposActividad;
-- SELECT a.ActividadId, a.Titulo, t.Nombre AS Tipo, a.FechaInicio, a.Cupo, a.Estado
--   FROM dbo.Actividades a JOIN dbo.TiposActividad t ON t.TipoId = a.TipoId;
-- SELECT * FROM dbo.Registros;
-- SELECT * FROM dbo.Inscripciones;
-- SELECT * FROM dbo.Resenas;
-- Cupos por actividad:
-- SELECT a.Titulo, a.Cupo, COUNT(i.InscripcionId) AS Inscritos
--   FROM dbo.Actividades a LEFT JOIN dbo.Inscripciones i ON i.ActividadId = a.ActividadId AND i.Estado = N'Activa'
--   GROUP BY a.Titulo, a.Cupo;
