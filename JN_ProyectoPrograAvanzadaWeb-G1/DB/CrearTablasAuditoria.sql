USE [DBInventario];
GO

------------------------------------------------------
-- 0. Asegurar que el esquema inv existe
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'inv')
BEGIN
    EXEC('CREATE SCHEMA inv');
END
GO

------------------------------------------------------
-- Tabla de Bitácora de Errores
------------------------------------------------------
IF OBJECT_ID('inv.BitacoraErrores','U') IS NULL
BEGIN
    -- Verificar que la tabla Usuarios existe antes de crear FK
    IF OBJECT_ID('inv.Usuarios','U') IS NOT NULL
    BEGIN
        CREATE TABLE inv.BitacoraErrores(
            BitacoraErrorID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BitacoraErrores PRIMARY KEY,
            UsuarioID       INT               NULL,
            Error           NVARCHAR(MAX)     NOT NULL,
            StackTrace      NVARCHAR(MAX)     NULL,
            Controlador     NVARCHAR(100)     NULL,
            Accion          NVARCHAR(100)     NULL,
            FechaUTC        DATETIME2         NOT NULL CONSTRAINT DF_BitacoraErrores_Fecha DEFAULT (SYSUTCDATETIME()),
            RequestPath     NVARCHAR(500)     NULL,
            UserAgent       NVARCHAR(500)     NULL,
            CONSTRAINT FK_BitacoraErrores_Usuarios 
                FOREIGN KEY (UsuarioID) REFERENCES inv.Usuarios(UsuarioID)
        );

        CREATE NONCLUSTERED INDEX IX_BitacoraErrores_Usuario_Fecha 
            ON inv.BitacoraErrores(UsuarioID, FechaUTC DESC);

        CREATE NONCLUSTERED INDEX IX_BitacoraErrores_Fecha 
            ON inv.BitacoraErrores(FechaUTC DESC);
    END
    ELSE
    BEGIN
        -- Crear sin FK si Usuarios no existe
        CREATE TABLE inv.BitacoraErrores(
            BitacoraErrorID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BitacoraErrores PRIMARY KEY,
            UsuarioID       INT               NULL,
            Error           NVARCHAR(MAX)     NOT NULL,
            StackTrace      NVARCHAR(MAX)     NULL,
            Controlador     NVARCHAR(100)     NULL,
            Accion          NVARCHAR(100)     NULL,
            FechaUTC        DATETIME2         NOT NULL CONSTRAINT DF_BitacoraErrores_Fecha DEFAULT (SYSUTCDATETIME()),
            RequestPath     NVARCHAR(500)     NULL,
            UserAgent       NVARCHAR(500)     NULL
        );

        CREATE NONCLUSTERED INDEX IX_BitacoraErrores_Usuario_Fecha 
            ON inv.BitacoraErrores(UsuarioID, FechaUTC DESC);

        CREATE NONCLUSTERED INDEX IX_BitacoraErrores_Fecha 
            ON inv.BitacoraErrores(FechaUTC DESC);

        PRINT 'Tabla BitacoraErrores creada sin FK (tabla Usuarios no existe aún)';
    END
END
GO

------------------------------------------------------
-- Tabla de Auditoría
------------------------------------------------------
IF OBJECT_ID('inv.Auditoria','U') IS NULL
BEGIN
    -- Verificar que la tabla Usuarios existe antes de crear FK
    IF OBJECT_ID('inv.Usuarios','U') IS NOT NULL
    BEGIN
        CREATE TABLE inv.Auditoria(
            AuditoriaID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Auditoria PRIMARY KEY,
            UsuarioID   INT               NOT NULL,
            Accion      NVARCHAR(200)     NOT NULL,
            Tabla       NVARCHAR(100)     NULL,
            RegistroID  INT               NULL,
            DatosAntes  NVARCHAR(MAX)     NULL,
            DatosDespues NVARCHAR(MAX)    NULL,
            FechaUTC    DATETIME2         NOT NULL CONSTRAINT DF_Auditoria_Fecha DEFAULT (SYSUTCDATETIME()),
            IPAddress   NVARCHAR(50)      NULL,
            UserAgent   NVARCHAR(500)     NULL,
            CONSTRAINT FK_Auditoria_Usuarios 
                FOREIGN KEY (UsuarioID) REFERENCES inv.Usuarios(UsuarioID)
        );

        CREATE NONCLUSTERED INDEX IX_Auditoria_Usuario_Fecha 
            ON inv.Auditoria(UsuarioID, FechaUTC DESC);

        CREATE NONCLUSTERED INDEX IX_Auditoria_Accion_Fecha 
            ON inv.Auditoria(Accion, FechaUTC DESC);

        CREATE NONCLUSTERED INDEX IX_Auditoria_Tabla_Registro 
            ON inv.Auditoria(Tabla, RegistroID);
    END
    ELSE
    BEGIN
        -- Crear sin FK si Usuarios no existe
        CREATE TABLE inv.Auditoria(
            AuditoriaID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Auditoria PRIMARY KEY,
            UsuarioID   INT               NOT NULL,
            Accion      NVARCHAR(200)     NOT NULL,
            Tabla       NVARCHAR(100)     NULL,
            RegistroID  INT               NULL,
            DatosAntes  NVARCHAR(MAX)     NULL,
            DatosDespues NVARCHAR(MAX)    NULL,
            FechaUTC    DATETIME2         NOT NULL CONSTRAINT DF_Auditoria_Fecha DEFAULT (SYSUTCDATETIME()),
            IPAddress   NVARCHAR(50)      NULL,
            UserAgent   NVARCHAR(500)     NULL
        );

        CREATE NONCLUSTERED INDEX IX_Auditoria_Usuario_Fecha 
            ON inv.Auditoria(UsuarioID, FechaUTC DESC);

        CREATE NONCLUSTERED INDEX IX_Auditoria_Accion_Fecha 
            ON inv.Auditoria(Accion, FechaUTC DESC);

        CREATE NONCLUSTERED INDEX IX_Auditoria_Tabla_Registro 
            ON inv.Auditoria(Tabla, RegistroID);

        PRINT 'Tabla Auditoria creada sin FK (tabla Usuarios no existe aún)';
    END
END
GO

------------------------------------------------------
-- Tabla de Stock Mínimo
------------------------------------------------------
IF OBJECT_ID('inv.StockMinimo','U') IS NULL
BEGIN
    -- Verificar que las tablas Productos y Bodegas existen
    IF OBJECT_ID('inv.Productos','U') IS NOT NULL AND OBJECT_ID('inv.Bodegas','U') IS NOT NULL
    BEGIN
        CREATE TABLE inv.StockMinimo(
            StockMinimoID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_StockMinimo PRIMARY KEY,
            ProductoID      INT               NOT NULL,
            BodegaID        INT               NOT NULL,
            CantidadMinima  DECIMAL(18,4)     NOT NULL,
            Activo          BIT               NOT NULL CONSTRAINT DF_StockMinimo_Activo DEFAULT(1),
            FechaActualizacion DATETIME2      NULL CONSTRAINT DF_StockMinimo_Fecha DEFAULT (SYSUTCDATETIME()),
            CONSTRAINT FK_StockMinimo_Productos 
                FOREIGN KEY (ProductoID) REFERENCES inv.Productos(ProductoID),
            CONSTRAINT FK_StockMinimo_Bodegas 
                FOREIGN KEY (BodegaID) REFERENCES inv.Bodegas(BodegaID),
            CONSTRAINT UQ_StockMinimo_Producto_Bodega UNIQUE (ProductoID, BodegaID)
        );

        CREATE NONCLUSTERED INDEX IX_StockMinimo_Producto_Bodega 
            ON inv.StockMinimo(ProductoID, BodegaID);
    END
    ELSE
    BEGIN
        PRINT 'Tabla StockMinimo no se pudo crear: Productos o Bodegas no existen aún';
    END
END
GO

------------------------------------------------------
-- Actualizar tabla Usuarios para incluir CorreoElectronico y ContrasenaHash si no existen
------------------------------------------------------
IF OBJECT_ID('inv.Usuarios','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'CorreoElectronico')
    BEGIN
        ALTER TABLE inv.Usuarios
        ADD CorreoElectronico NVARCHAR(100) NULL;
        PRINT 'Columna CorreoElectronico agregada a inv.Usuarios';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'ContrasenaHash')
    BEGIN
        ALTER TABLE inv.Usuarios
        ADD ContrasenaHash NVARCHAR(255) NULL;
        PRINT 'Columna ContrasenaHash agregada a inv.Usuarios';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'FechaRegistro')
    BEGIN
        ALTER TABLE inv.Usuarios
        ADD FechaRegistro DATETIME2 NULL CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT (SYSUTCDATETIME());
        PRINT 'Columna FechaRegistro agregada a inv.Usuarios';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'BodegaID')
    BEGIN
        ALTER TABLE inv.Usuarios
        ADD BodegaID INT NULL;
        PRINT 'Columna BodegaID agregada a inv.Usuarios';
    END
END
ELSE
BEGIN
    PRINT 'Tabla inv.Usuarios no existe. Se deben ejecutar primero los scripts de creación de tablas base.';
END
GO

-- Agregar FK de Usuarios.BodegaID si existe la tabla Bodegas
IF OBJECT_ID('inv.Usuarios','U') IS NOT NULL 
   AND OBJECT_ID('inv.Bodegas','U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'BodegaID')
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.Usuarios') 
                   AND name = 'FK_Usuarios_Bodegas')
BEGIN
    ALTER TABLE inv.Usuarios
    ADD CONSTRAINT FK_Usuarios_Bodegas 
        FOREIGN KEY (BodegaID) REFERENCES inv.Bodegas(BodegaID);
    PRINT 'FK FK_Usuarios_Bodegas agregada exitosamente';
END
GO

------------------------------------------------------
-- Actualizar tabla Bodegas para incluir Ubicacion si no existe
------------------------------------------------------
IF OBJECT_ID('inv.Bodegas','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Bodegas') AND name = 'Ubicacion')
    BEGIN
        ALTER TABLE inv.Bodegas
        ADD Ubicacion NVARCHAR(200) NULL;
        PRINT 'Columna Ubicacion agregada a inv.Bodegas';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Bodegas') AND name = 'FechaCreacion')
    BEGIN
        ALTER TABLE inv.Bodegas
        ADD FechaCreacion DATETIME2 NULL CONSTRAINT DF_Bodegas_FechaCreacion DEFAULT (SYSUTCDATETIME());
        PRINT 'Columna FechaCreacion agregada a inv.Bodegas';
    END
END
ELSE
BEGIN
    PRINT 'Tabla inv.Bodegas no existe. Se deben ejecutar primero los scripts de creación de tablas base.';
END
GO

------------------------------------------------------
-- Actualizar tabla Solicitudes para incluir campos faltantes
------------------------------------------------------
IF OBJECT_ID('inv.Solicitudes','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'BodegaID')
    BEGIN
        ALTER TABLE inv.Solicitudes
        ADD BodegaID INT NULL;
        PRINT 'Columna BodegaID agregada a inv.Solicitudes';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'UsuarioID')
    BEGIN
        ALTER TABLE inv.Solicitudes
        ADD UsuarioID INT NULL;
        PRINT 'Columna UsuarioID agregada a inv.Solicitudes';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'Comentarios')
    BEGIN
        ALTER TABLE inv.Solicitudes
        ADD Comentarios NVARCHAR(500) NULL;
        PRINT 'Columna Comentarios agregada a inv.Solicitudes';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'UsuarioAprobadorID')
    BEGIN
        ALTER TABLE inv.Solicitudes
        ADD UsuarioAprobadorID INT NULL;
        PRINT 'Columna UsuarioAprobadorID agregada a inv.Solicitudes';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'FechaAprobacionUTC')
    BEGIN
        ALTER TABLE inv.Solicitudes
        ADD FechaAprobacionUTC DATETIME2 NULL;
        PRINT 'Columna FechaAprobacionUTC agregada a inv.Solicitudes';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'FechaEnvioUTC')
    BEGIN
        ALTER TABLE inv.Solicitudes
        ADD FechaEnvioUTC DATETIME2 NULL;
        PRINT 'Columna FechaEnvioUTC agregada a inv.Solicitudes';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'FechaEntregaUTC')
    BEGIN
        ALTER TABLE inv.Solicitudes
        ADD FechaEntregaUTC DATETIME2 NULL;
        PRINT 'Columna FechaEntregaUTC agregada a inv.Solicitudes';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'DespachoID')
    BEGIN
        ALTER TABLE inv.Solicitudes
        ADD DespachoID INT NULL;
        PRINT 'Columna DespachoID agregada a inv.Solicitudes';
    END
END
ELSE
BEGIN
    PRINT 'Tabla inv.Solicitudes no existe. Se deben ejecutar primero los scripts de creación de tablas base.';
END
GO

-- Agregar FKs de Solicitudes si existen las tablas relacionadas
IF OBJECT_ID('inv.Solicitudes','U') IS NOT NULL AND OBJECT_ID('inv.Bodegas','U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'BodegaID')
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.Solicitudes') 
                   AND name = 'FK_Solicitudes_Bodegas')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD CONSTRAINT FK_Solicitudes_Bodegas 
        FOREIGN KEY (BodegaID) REFERENCES inv.Bodegas(BodegaID);
    PRINT 'FK FK_Solicitudes_Bodegas agregada exitosamente';
END
GO

IF OBJECT_ID('inv.Solicitudes','U') IS NOT NULL AND OBJECT_ID('inv.Usuarios','U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'UsuarioID')
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.Solicitudes') 
                   AND name = 'FK_Solicitudes_Usuarios')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD CONSTRAINT FK_Solicitudes_Usuarios 
        FOREIGN KEY (UsuarioID) REFERENCES inv.Usuarios(UsuarioID);
    PRINT 'FK FK_Solicitudes_Usuarios agregada exitosamente';
END
GO

IF OBJECT_ID('inv.Solicitudes','U') IS NOT NULL AND OBJECT_ID('inv.Usuarios','U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'UsuarioAprobadorID')
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.Solicitudes') 
                   AND name = 'FK_Solicitudes_UsuariosAprobador')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD CONSTRAINT FK_Solicitudes_UsuariosAprobador 
        FOREIGN KEY (UsuarioAprobadorID) REFERENCES inv.Usuarios(UsuarioID);
    PRINT 'FK FK_Solicitudes_UsuariosAprobador agregada exitosamente';
END
GO

IF OBJECT_ID('inv.Solicitudes','U') IS NOT NULL AND OBJECT_ID('inv.Despachos','U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'DespachoID')
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.Solicitudes') 
                   AND name = 'FK_Solicitudes_Despachos')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD CONSTRAINT FK_Solicitudes_Despachos 
        FOREIGN KEY (DespachoID) REFERENCES inv.Despachos(DespachoID);
    PRINT 'FK FK_Solicitudes_Despachos agregada exitosamente';
END
GO

IF OBJECT_ID('inv.SolicitudDetalle','U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.SolicitudDetalle') AND name = 'CantidadEnviada')
BEGIN
    ALTER TABLE inv.SolicitudDetalle
    ADD CantidadEnviada DECIMAL(18,4) NULL;
    PRINT 'Columna CantidadEnviada agregada a inv.SolicitudDetalle';
END
GO

------------------------------------------------------
-- Actualizar tabla Productos para incluir campos faltantes
------------------------------------------------------
IF OBJECT_ID('inv.Productos','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Productos') AND name = 'Descripcion')
    BEGIN
        ALTER TABLE inv.Productos
        ADD Descripcion NVARCHAR(500) NULL;
        PRINT 'Columna Descripcion agregada a inv.Productos';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Productos') AND name = 'Activo')
    BEGIN
        ALTER TABLE inv.Productos
        ADD Activo BIT NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT(1);
        PRINT 'Columna Activo agregada a inv.Productos';
    END

    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Productos') AND name = 'FechaCreacion')
    BEGIN
        ALTER TABLE inv.Productos
        ADD FechaCreacion DATETIME2 NULL CONSTRAINT DF_Productos_FechaCreacion DEFAULT (SYSUTCDATETIME());
        PRINT 'Columna FechaCreacion agregada a inv.Productos';
    END
END
ELSE
BEGIN
    PRINT 'Tabla inv.Productos no existe. Se deben ejecutar primero los scripts de creación de tablas base.';
END
GO

------------------------------------------------------
-- Actualizar tabla Movimientos para incluir SolicitudID si no existe
------------------------------------------------------
IF OBJECT_ID('inv.Movimientos','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Movimientos') AND name = 'SolicitudID')
    BEGIN
        ALTER TABLE inv.Movimientos
        ADD SolicitudID INT NULL;
        PRINT 'Columna SolicitudID agregada a inv.Movimientos';
    END
END
ELSE
BEGIN
    PRINT 'Tabla inv.Movimientos no existe. Se deben ejecutar primero los scripts de creación de tablas base.';
END
GO

-- Agregar FK de Movimientos.SolicitudID si existe la tabla Solicitudes
IF OBJECT_ID('inv.Movimientos','U') IS NOT NULL 
   AND OBJECT_ID('inv.Solicitudes','U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Movimientos') AND name = 'SolicitudID')
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.Movimientos') 
                   AND name = 'FK_Movimientos_Solicitudes')
BEGIN
    ALTER TABLE inv.Movimientos
    ADD CONSTRAINT FK_Movimientos_Solicitudes 
        FOREIGN KEY (SolicitudID) REFERENCES inv.Solicitudes(SolicitudID);
    PRINT 'FK FK_Movimientos_Solicitudes agregada exitosamente';
END
GO

------------------------------------------------------
-- Actualizar tabla Ubicaciones para incluir BodegaID si no existe
------------------------------------------------------
IF OBJECT_ID('inv.Ubicaciones','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Ubicaciones') AND name = 'BodegaID')
    BEGIN
        ALTER TABLE inv.Ubicaciones
        ADD BodegaID INT NULL;
        PRINT 'Columna BodegaID agregada a inv.Ubicaciones';
    END
END
ELSE
BEGIN
    PRINT 'Tabla inv.Ubicaciones no existe. Se deben ejecutar primero los scripts de creación de tablas base.';
END
GO

-- Agregar FK de Ubicaciones.BodegaID si existe la tabla Bodegas
IF OBJECT_ID('inv.Ubicaciones','U') IS NOT NULL 
   AND OBJECT_ID('inv.Bodegas','U') IS NOT NULL
   AND EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Ubicaciones') AND name = 'BodegaID')
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.Ubicaciones') 
                   AND name = 'FK_Ubicaciones_Bodegas')
BEGIN
    ALTER TABLE inv.Ubicaciones
    ADD CONSTRAINT FK_Ubicaciones_Bodegas 
        FOREIGN KEY (BodegaID) REFERENCES inv.Bodegas(BodegaID);
    PRINT 'FK FK_Ubicaciones_Bodegas agregada exitosamente';
END
GO

------------------------------------------------------
-- Actualizar tabla Despachos para incluir FechaRecepcionUTC si no existe
------------------------------------------------------
IF OBJECT_ID('inv.Despachos','U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Despachos') AND name = 'FechaRecepcionUTC')
    BEGIN
        ALTER TABLE inv.Despachos
        ADD FechaRecepcionUTC DATETIME2 NULL;
        PRINT 'Columna FechaRecepcionUTC agregada a inv.Despachos';
    END
END
ELSE
BEGIN
    PRINT 'Tabla inv.Despachos no existe. Se deben ejecutar primero los scripts de creación de tablas base.';
END
GO

------------------------------------------------------
-- Verificar que las FKs pendientes se puedan agregar ahora
------------------------------------------------------

-- Agregar FK de BitacoraErrores si Usuarios existe ahora
IF OBJECT_ID('inv.BitacoraErrores','U') IS NOT NULL 
   AND OBJECT_ID('inv.Usuarios','U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.BitacoraErrores') 
                   AND name = 'FK_BitacoraErrores_Usuarios')
BEGIN
    ALTER TABLE inv.BitacoraErrores
    ADD CONSTRAINT FK_BitacoraErrores_Usuarios 
        FOREIGN KEY (UsuarioID) REFERENCES inv.Usuarios(UsuarioID);
    PRINT 'FK FK_BitacoraErrores_Usuarios agregada exitosamente';
END
GO

-- Agregar FK de Auditoria si Usuarios existe ahora
IF OBJECT_ID('inv.Auditoria','U') IS NOT NULL 
   AND OBJECT_ID('inv.Usuarios','U') IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM sys.foreign_keys 
                   WHERE parent_object_id = OBJECT_ID('inv.Auditoria') 
                   AND name = 'FK_Auditoria_Usuarios')
BEGIN
    ALTER TABLE inv.Auditoria
    ADD CONSTRAINT FK_Auditoria_Usuarios 
        FOREIGN KEY (UsuarioID) REFERENCES inv.Usuarios(UsuarioID);
    PRINT 'FK FK_Auditoria_Usuarios agregada exitosamente';
END
GO

PRINT 'Script completado. Tablas de auditoría y campos adicionales creados/actualizados.';
PRINT 'NOTA: Si alguna tabla base no existe, ejecute primero DBInventario.sql';
GO

