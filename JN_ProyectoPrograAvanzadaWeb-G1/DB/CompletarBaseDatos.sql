USE [DBInventario];
GO

------------------------------------------------------
-- SCRIPT PARA COMPLETAR LA BASE DE DATOS
-- Este script agrega todo lo necesario que falta
-- basándose en la base de datos actual
------------------------------------------------------

PRINT 'Iniciando completado de base de datos...';
GO

------------------------------------------------------
-- 1. TABLAS DE AUDITORÍA Y BITÁCORA
------------------------------------------------------

-- Tabla de Bitácora de Errores
IF OBJECT_ID('inv.BitacoraErrores','U') IS NULL
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

    PRINT 'Tabla BitacoraErrores creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla BitacoraErrores ya existe';
END
GO

-- Tabla de Auditoría
IF OBJECT_ID('inv.Auditoria','U') IS NULL
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

    PRINT 'Tabla Auditoria creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla Auditoria ya existe';
END
GO

-- Tabla de Stock Mínimo
IF OBJECT_ID('inv.StockMinimo','U') IS NULL
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

    PRINT 'Tabla StockMinimo creada exitosamente';
END
ELSE
BEGIN
    PRINT 'Tabla StockMinimo ya existe';
END
GO

------------------------------------------------------
-- 2. AGREGAR COLUMNAS FALTANTES A TABLAS EXISTENTES
------------------------------------------------------

-- Usuarios: BodegaID (para asignar técnicos a bodegas)
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'BodegaID')
BEGIN
    ALTER TABLE inv.Usuarios
    ADD BodegaID INT NULL;
    PRINT 'Columna BodegaID agregada a inv.Usuarios';
END
ELSE
BEGIN
    PRINT 'Columna BodegaID ya existe en inv.Usuarios';
END
GO

-- FK de Usuarios.BodegaID
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'BodegaID')
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

-- Bodegas: Ubicacion, FechaCreacion
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Bodegas') AND name = 'Ubicacion')
BEGIN
    ALTER TABLE inv.Bodegas
    ADD Ubicacion NVARCHAR(200) NULL;
    PRINT 'Columna Ubicacion agregada a inv.Bodegas';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Bodegas') AND name = 'FechaCreacion')
BEGIN
    ALTER TABLE inv.Bodegas
    ADD FechaCreacion DATETIME2 NULL CONSTRAINT DF_Bodegas_FechaCreacion DEFAULT (SYSUTCDATETIME());
    PRINT 'Columna FechaCreacion agregada a inv.Bodegas';
END
GO

-- Productos: Descripcion, Activo, FechaCreacion
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Productos') AND name = 'Descripcion')
BEGIN
    ALTER TABLE inv.Productos
    ADD Descripcion NVARCHAR(500) NULL;
    PRINT 'Columna Descripcion agregada a inv.Productos';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Productos') AND name = 'Activo')
BEGIN
    ALTER TABLE inv.Productos
    ADD Activo BIT NOT NULL CONSTRAINT DF_Productos_Activo DEFAULT(1);
    PRINT 'Columna Activo agregada a inv.Productos';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Productos') AND name = 'FechaCreacion')
BEGIN
    ALTER TABLE inv.Productos
    ADD FechaCreacion DATETIME2 NULL CONSTRAINT DF_Productos_FechaCreacion DEFAULT (SYSUTCDATETIME());
    PRINT 'Columna FechaCreacion agregada a inv.Productos';
END
GO

-- Solicitudes: Campos adicionales
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'BodegaID')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD BodegaID INT NULL;
    PRINT 'Columna BodegaID agregada a inv.Solicitudes';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'UsuarioID')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD UsuarioID INT NULL;
    PRINT 'Columna UsuarioID agregada a inv.Solicitudes';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'Comentarios')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD Comentarios NVARCHAR(500) NULL;
    PRINT 'Columna Comentarios agregada a inv.Solicitudes';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'UsuarioAprobadorID')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD UsuarioAprobadorID INT NULL;
    PRINT 'Columna UsuarioAprobadorID agregada a inv.Solicitudes';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'FechaAprobacionUTC')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD FechaAprobacionUTC DATETIME2 NULL;
    PRINT 'Columna FechaAprobacionUTC agregada a inv.Solicitudes';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'FechaEnvioUTC')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD FechaEnvioUTC DATETIME2 NULL;
    PRINT 'Columna FechaEnvioUTC agregada a inv.Solicitudes';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'FechaEntregaUTC')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD FechaEntregaUTC DATETIME2 NULL;
    PRINT 'Columna FechaEntregaUTC agregada a inv.Solicitudes';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'DespachoID')
BEGIN
    ALTER TABLE inv.Solicitudes
    ADD DespachoID INT NULL;
    PRINT 'Columna DespachoID agregada a inv.Solicitudes';
END
GO

-- SolicitudDetalle: CantidadEnviada
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.SolicitudDetalle') AND name = 'CantidadEnviada')
BEGIN
    ALTER TABLE inv.SolicitudDetalle
    ADD CantidadEnviada DECIMAL(18,4) NULL;
    PRINT 'Columna CantidadEnviada agregada a inv.SolicitudDetalle';
END
GO

-- Movimientos: SolicitudID
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Movimientos') AND name = 'SolicitudID')
BEGIN
    ALTER TABLE inv.Movimientos
    ADD SolicitudID INT NULL;
    PRINT 'Columna SolicitudID agregada a inv.Movimientos';
END
GO

-- Ubicaciones: BodegaID
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Ubicaciones') AND name = 'BodegaID')
BEGIN
    ALTER TABLE inv.Ubicaciones
    ADD BodegaID INT NULL;
    PRINT 'Columna BodegaID agregada a inv.Ubicaciones';
END
GO

-- Despachos: FechaRecepcionUTC
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Despachos') AND name = 'FechaRecepcionUTC')
BEGIN
    ALTER TABLE inv.Despachos
    ADD FechaRecepcionUTC DATETIME2 NULL;
    PRINT 'Columna FechaRecepcionUTC agregada a inv.Despachos';
END
GO

------------------------------------------------------
-- 3. AGREGAR FOREIGN KEYS FALTANTES
------------------------------------------------------

-- FK Solicitudes -> Bodegas
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'BodegaID')
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

-- FK Solicitudes -> Usuarios (creador)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'UsuarioID')
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

-- FK Solicitudes -> Usuarios (aprobador)
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'UsuarioAprobadorID')
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

-- FK Solicitudes -> Despachos
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Solicitudes') AND name = 'DespachoID')
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

-- FK Movimientos -> Solicitudes
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Movimientos') AND name = 'SolicitudID')
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

-- FK Ubicaciones -> Bodegas
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Ubicaciones') AND name = 'BodegaID')
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
-- 4. CREAR ÍNDICES ADICIONALES PARA OPTIMIZACIÓN
------------------------------------------------------

-- Índice para Usuarios.BodegaID
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Usuarios_BodegaID' AND object_id = OBJECT_ID('inv.Usuarios'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Usuarios_BodegaID
        ON inv.Usuarios(BodegaID)
        WHERE BodegaID IS NOT NULL;
    PRINT 'Índice IX_Usuarios_BodegaID creado';
END
GO

-- Índice para Solicitudes.BodegaID
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Solicitudes_BodegaID' AND object_id = OBJECT_ID('inv.Solicitudes'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Solicitudes_BodegaID
        ON inv.Solicitudes(BodegaID)
        WHERE BodegaID IS NOT NULL;
    PRINT 'Índice IX_Solicitudes_BodegaID creado';
END
GO

-- Índice para Solicitudes.UsuarioID
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Solicitudes_UsuarioID' AND object_id = OBJECT_ID('inv.Solicitudes'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Solicitudes_UsuarioID
        ON inv.Solicitudes(UsuarioID)
        WHERE UsuarioID IS NOT NULL;
    PRINT 'Índice IX_Solicitudes_UsuarioID creado';
END
GO

-- Índice para Movimientos.SolicitudID
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Movimientos_SolicitudID' AND object_id = OBJECT_ID('inv.Movimientos'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Movimientos_SolicitudID
        ON inv.Movimientos(SolicitudID)
        WHERE SolicitudID IS NOT NULL;
    PRINT 'Índice IX_Movimientos_SolicitudID creado';
END
GO

-- Índice para Ubicaciones.BodegaID
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ubicaciones_BodegaID' AND object_id = OBJECT_ID('inv.Ubicaciones'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Ubicaciones_BodegaID
        ON inv.Ubicaciones(BodegaID)
        WHERE BodegaID IS NOT NULL;
    PRINT 'Índice IX_Ubicaciones_BodegaID creado';
END
GO

-- Índice para Productos.Activo (búsquedas de productos activos)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Productos_Activo' AND object_id = OBJECT_ID('inv.Productos'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Productos_Activo
        ON inv.Productos(Activo, Nombre)
        WHERE Activo = 1;
    PRINT 'Índice IX_Productos_Activo creado';
END
GO

------------------------------------------------------
-- 5. ACTUALIZAR VISTA DE SALDO (si necesita mejoras)
------------------------------------------------------
-- La vista actual es un stub. Aquí puedes agregar la lógica real
-- cuando implementes el cálculo de saldo desde movimientos

PRINT 'Script completado exitosamente.';
PRINT 'La base de datos está lista para el proyecto completo.';
GO

