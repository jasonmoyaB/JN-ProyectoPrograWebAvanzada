USE [DBInventario];
GO

------------------------------------------------------
-- 0. Esquema de trabajo
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'inv')
    EXEC('CREATE SCHEMA inv');
GO

------------------------------------------------------
-- 1. Catalogos de seguridad
------------------------------------------------------
IF OBJECT_ID('inv.Roles','U') IS NULL
BEGIN
    CREATE TABLE inv.Roles(
        RolID      INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
        NombreRol  NVARCHAR(50)      NOT NULL,
        CONSTRAINT UQ_Roles_NombreRol UNIQUE (NombreRol)
    );
END
GO

IF OBJECT_ID('inv.Usuarios','U') IS NULL
BEGIN
    CREATE TABLE inv.Usuarios(
        UsuarioID  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
        Nombre     NVARCHAR(100)     NOT NULL,
        RolID      INT               NOT NULL,
        Activo     BIT               NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT(1),
        CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolID) REFERENCES inv.Roles(RolID)
    );
END
GO

------------------------------------------------------
-- 2. Catalogos de inventario
------------------------------------------------------
IF OBJECT_ID('inv.Bodegas','U') IS NULL
BEGIN
    CREATE TABLE inv.Bodegas(
        BodegaID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Bodegas PRIMARY KEY,
        Nombre   NVARCHAR(100)     NOT NULL,
        Activo   BIT               NOT NULL CONSTRAINT DF_Bodegas_Activo DEFAULT(1)
    );
END
GO

IF OBJECT_ID('inv.Proveedores','U') IS NULL
BEGIN
    CREATE TABLE inv.Proveedores(
        ProveedorID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Proveedores PRIMARY KEY,
        Nombre      NVARCHAR(100)     NOT NULL,
        Activo      BIT               NOT NULL CONSTRAINT DF_Proveedores_Activo DEFAULT(1)
    );
END
GO

IF OBJECT_ID('inv.UnidadesMedida','U') IS NULL
BEGIN
    CREATE TABLE inv.UnidadesMedida(
        UnidadID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_UnidadesMedida PRIMARY KEY,
        Nombre   NVARCHAR(50)      NOT NULL,
        CONSTRAINT UQ_UnidadesMedida_Nombre UNIQUE (Nombre)
    );
END
GO

IF OBJECT_ID('inv.Productos','U') IS NULL
BEGIN
    CREATE TABLE inv.Productos(
        ProductoID   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Productos PRIMARY KEY,
        SKU          NVARCHAR(50)      NOT NULL,
        Nombre       NVARCHAR(100)     NOT NULL,
        EsSerializado BIT              NOT NULL CONSTRAINT DF_Productos_EsSerializado DEFAULT(0),
        CONSTRAINT UQ_Productos_SKU UNIQUE (SKU)
    );
END
GO

IF OBJECT_ID('inv.TiposMovimiento','U') IS NULL
BEGIN
    CREATE TABLE inv.TiposMovimiento(
        TipoMovimientoID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TiposMovimiento PRIMARY KEY,
        Codigo           NVARCHAR(50)      NOT NULL,
        Naturaleza       SMALLINT          NOT NULL,   -- -1 salida, 1 entrada
        CONSTRAINT UQ_TiposMovimiento_Codigo UNIQUE (Codigo),
        CONSTRAINT CK_TiposMovimiento_Naturaleza CHECK (Naturaleza IN (-1,1))
    );
END
GO

IF OBJECT_ID('inv.CierresAuditoria','U') IS NULL
BEGIN
    CREATE TABLE inv.CierresAuditoria(
        CierreAuditoriaID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CierresAuditoria PRIMARY KEY,
        FechaHastaUTC     DATETIME2         NOT NULL,
        Activo            BIT               NOT NULL CONSTRAINT DF_CierresAuditoria_Activo DEFAULT(1)
    );
END
GO

-- Catalogo de ubicaciones 
IF OBJECT_ID('inv.Ubicaciones','U') IS NULL
BEGIN
    CREATE TABLE inv.Ubicaciones(
        UbicacionID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Ubicaciones PRIMARY KEY,
        Nombre      NVARCHAR(100)     NOT NULL,
        Activo      BIT               NOT NULL CONSTRAINT DF_Ubicaciones_Activo DEFAULT(1)
    );
END
GO

-- Catalogo de motivos de ajuste (para normalizar MotivoAjusteID)
IF OBJECT_ID('inv.MotivosAjuste','U') IS NULL
BEGIN
    CREATE TABLE inv.MotivosAjuste(
        MotivoAjusteID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MotivosAjuste PRIMARY KEY,
        Nombre         NVARCHAR(100)     NOT NULL,
        Activo         BIT               NOT NULL CONSTRAINT DF_MotivosAjuste_Activo DEFAULT(1)
    );
END
GO

------------------------------------------------------
-- 3. Cat�logo de estados de solicitud 
------------------------------------------------------
IF OBJECT_ID('inv.EstadosSolicitud','U') IS NULL
BEGIN
    CREATE TABLE inv.EstadosSolicitud(
        EstadoSolicitudID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_EstadosSolicitud PRIMARY KEY,
        Codigo            NVARCHAR(50)      NOT NULL,
        CONSTRAINT UQ_EstadosSolicitud_Codigo UNIQUE (Codigo)
    );
END
GO

------------------------------------------------------
-- 4. Tablas de Solicitudes (KPI)
------------------------------------------------------
IF OBJECT_ID('inv.Solicitudes','U') IS NULL
BEGIN
    CREATE TABLE inv.Solicitudes(
        SolicitudID       INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Solicitudes PRIMARY KEY,
        EstadoSolicitudID INT               NOT NULL,
        BodegaID          INT               NOT NULL,
        UsuarioID         INT               NOT NULL,
        FechaCreacionUTC  DATETIME2         NOT NULL CONSTRAINT DF_Solicitudes_Fecha DEFAULT (SYSUTCDATETIME()),
        FechaAprobacionUTC DATETIME2        NULL,
        FechaEnvioUTC     DATETIME2         NULL,
        FechaEntregaUTC   DATETIME2         NULL,
        Comentarios       NVARCHAR(255)     NULL,
        UsuarioAprobadorID INT              NULL,
        DespachoID        INT               NULL,
        CONSTRAINT FK_Solicitudes_EstadoSolicitud 
            FOREIGN KEY (EstadoSolicitudID) REFERENCES inv.EstadosSolicitud(EstadoSolicitudID),
        CONSTRAINT FK_Solicitudes_Bodega 
            FOREIGN KEY (BodegaID) REFERENCES inv.Bodegas(BodegaID),
        CONSTRAINT FK_Solicitudes_Usuario 
            FOREIGN KEY (UsuarioID) REFERENCES inv.Usuarios(UsuarioID),
        CONSTRAINT FK_Solicitudes_UsuarioAprobador 
            FOREIGN KEY (UsuarioAprobadorID) REFERENCES inv.Usuarios(UsuarioID)
    );
END
GO

IF OBJECT_ID('inv.SolicitudDetalle','U') IS NULL
BEGIN
    CREATE TABLE inv.SolicitudDetalle(
        SolicitudDetalleID  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SolicitudDetalle PRIMARY KEY,
        SolicitudID         INT               NOT NULL,
        ProductoID          INT               NOT NULL,
        CantidadSolicitada  DECIMAL(18,4)     NOT NULL,
        CONSTRAINT FK_SolicitudDetalle_Solicitudes 
            FOREIGN KEY (SolicitudID) REFERENCES inv.Solicitudes(SolicitudID),
        CONSTRAINT FK_SolicitudDetalle_Productos 
            FOREIGN KEY (ProductoID)  REFERENCES inv.Productos(ProductoID)
    );
END
GO

------------------------------------------------------
-- 5. Movimientos de inventario
------------------------------------------------------
IF OBJECT_ID('inv.Movimientos','U') IS NULL
BEGIN
    CREATE TABLE inv.Movimientos(
        MovimientoID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Movimientos PRIMARY KEY,
        BodegaID            INT               NOT NULL,
        TipoMovimientoID    INT               NOT NULL,
        UsuarioID           INT               NOT NULL,
        FechaMovimientoUTC  DATETIME2         NOT NULL,
        Referencia          NVARCHAR(100)     NULL,
        Observaciones       NVARCHAR(255)     NULL,
        BodegaRelacionadaID INT               NULL,
        ProveedorID         INT               NULL,
        CONSTRAINT FK_Movimientos_Bodega 
            FOREIGN KEY (BodegaID) REFERENCES inv.Bodegas(BodegaID),
        CONSTRAINT FK_Movimientos_TipoMovimiento 
            FOREIGN KEY (TipoMovimientoID) REFERENCES inv.TiposMovimiento(TipoMovimientoID),
        CONSTRAINT FK_Movimientos_Usuario 
            FOREIGN KEY (UsuarioID) REFERENCES inv.Usuarios(UsuarioID),
        CONSTRAINT FK_Movimientos_BodegaRelacionada 
            FOREIGN KEY (BodegaRelacionadaID) REFERENCES inv.Bodegas(BodegaID),
        CONSTRAINT FK_Movimientos_Proveedor 
            FOREIGN KEY (ProveedorID) REFERENCES inv.Proveedores(ProveedorID)
    );
END
GO

IF OBJECT_ID('inv.MovimientoDetalle','U') IS NULL
BEGIN
    CREATE TABLE inv.MovimientoDetalle(
        MovimientoDetalleID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MovimientoDetalle PRIMARY KEY,
        MovimientoID        INT               NOT NULL,
        ProductoID          INT               NOT NULL,
        UbicacionID         INT               NULL,
        UnidadID            INT               NOT NULL,
        Cantidad            DECIMAL(18,4)     NOT NULL,
        CostoUnitario       DECIMAL(18,4)     NULL,
        MotivoAjusteID      INT               NULL,
        CONSTRAINT FK_MovimientoDetalle_Movimientos 
            FOREIGN KEY (MovimientoID)   REFERENCES inv.Movimientos(MovimientoID),
        CONSTRAINT FK_MovimientoDetalle_Productos 
            FOREIGN KEY (ProductoID)     REFERENCES inv.Productos(ProductoID),
        CONSTRAINT FK_MovimientoDetalle_Unidades 
            FOREIGN KEY (UnidadID)       REFERENCES inv.UnidadesMedida(UnidadID),
        CONSTRAINT FK_MovimientoDetalle_Ubicaciones 
            FOREIGN KEY (UbicacionID)    REFERENCES inv.Ubicaciones(UbicacionID),
        CONSTRAINT FK_MovimientoDetalle_MotivosAjuste 
            FOREIGN KEY (MotivoAjusteID) REFERENCES inv.MotivosAjuste(MotivoAjusteID)
    );
END
GO

------------------------------------------------------
-- 6. Series
------------------------------------------------------
IF OBJECT_ID('inv.Series','U') IS NULL
BEGIN
    CREATE TABLE inv.Series(
        SerieID     INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Series PRIMARY KEY,
        ProductoID  INT               NOT NULL,
        NumeroSerie NVARCHAR(120)     NOT NULL,
        CONSTRAINT FK_Series_Productos 
            FOREIGN KEY (ProductoID) REFERENCES inv.Productos(ProductoID),
        CONSTRAINT UQ_Series_Producto_NumeroSerie UNIQUE (ProductoID, NumeroSerie)
    );
END
GO

IF OBJECT_ID('inv.MovimientoDetalleSeries','U') IS NULL
BEGIN
    CREATE TABLE inv.MovimientoDetalleSeries(
        MovimientoDetalleSerieID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_MovimientoDetalleSeries PRIMARY KEY,
        MovimientoDetalleID      INT               NOT NULL,
        SerieID                  INT               NOT NULL,
        CONSTRAINT FK_MovimientoDetalleSeries_Detalle 
            FOREIGN KEY (MovimientoDetalleID) REFERENCES inv.MovimientoDetalle(MovimientoDetalleID),
        CONSTRAINT FK_MovimientoDetalleSeries_Series 
            FOREIGN KEY (SerieID) REFERENCES inv.Series(SerieID)
    );
END
GO

------------------------------------------------------
-- 7. Despachos
------------------------------------------------------
IF OBJECT_ID('inv.Despachos','U') IS NULL
BEGIN
    CREATE TABLE inv.Despachos(
        DespachoID        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Despachos PRIMARY KEY,
        SolicitudID       INT               NULL,
        NumeroEnvio       NVARCHAR(60)      NOT NULL,
        BodegaOrigenID    INT               NOT NULL,
        BodegaDestinoID   INT               NOT NULL,
        Estado            NVARCHAR(50)      NOT NULL,
        UsuarioCreadorID  INT               NOT NULL,
        FechaCreacionUTC  DATETIME2         NOT NULL CONSTRAINT DF_Despachos_FechaCreacion DEFAULT (SYSUTCDATETIME()),
        FechaDespachoUTC  DATETIME2         NULL,
        CONSTRAINT FK_Despachos_Solicitudes 
            FOREIGN KEY (SolicitudID)      REFERENCES inv.Solicitudes(SolicitudID),
        CONSTRAINT FK_Despachos_BodegaOrigen 
            FOREIGN KEY (BodegaOrigenID)   REFERENCES inv.Bodegas(BodegaID),
        CONSTRAINT FK_Despachos_BodegaDestino 
            FOREIGN KEY (BodegaDestinoID)  REFERENCES inv.Bodegas(BodegaID),
        CONSTRAINT FK_Despachos_UsuarioCreador 
            FOREIGN KEY (UsuarioCreadorID) REFERENCES inv.Usuarios(UsuarioID)
    );
END
GO

IF OBJECT_ID('inv.DespachoDetalle','U') IS NULL
BEGIN
    CREATE TABLE inv.DespachoDetalle(
        DespachoDetalleID INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DespachoDetalle PRIMARY KEY,
        DespachoID        INT               NOT NULL,
        ProductoID        INT               NOT NULL,
        Cantidad          DECIMAL(18,4)     NOT NULL,
        CONSTRAINT FK_DespachoDetalle_Despachos 
            FOREIGN KEY (DespachoID) REFERENCES inv.Despachos(DespachoID),
        CONSTRAINT FK_DespachoDetalle_Productos 
            FOREIGN KEY (ProductoID) REFERENCES inv.Productos(ProductoID)
    );
END
GO

------------------------------------------------------
-- 8. VISTA de saldo de inventario
-- Calcula el inventario real basándose en los movimientos
------------------------------------------------------
IF OBJECT_ID('inv.v_SaldoInventario','V') IS NOT NULL 
    DROP VIEW inv.v_SaldoInventario;
GO

CREATE VIEW inv.v_SaldoInventario AS
SELECT 
    m.BodegaID,
    md.ProductoID,
    SUM(md.Cantidad * tm.Naturaleza) AS Cantidad
FROM inv.Movimientos m
INNER JOIN inv.MovimientoDetalle md ON md.MovimientoID = m.MovimientoID
INNER JOIN inv.TiposMovimiento tm ON tm.TipoMovimientoID = m.TipoMovimientoID
GROUP BY m.BodegaID, md.ProductoID
HAVING SUM(md.Cantidad * tm.Naturaleza) > 0;  -- Solo mostrar productos con stock positivo
GO

------------------------------------------------------
-- 9. Tipos de tabla
------------------------------------------------------
IF TYPE_ID(N'inv.tt_MovimientoDetalle') IS NULL
    CREATE TYPE inv.tt_MovimientoDetalle AS TABLE(
        ProductoID    INT           NOT NULL,
        UbicacionID   INT           NULL,
        UnidadID      INT           NOT NULL,
        Cantidad      DECIMAL(18,4) NOT NULL,
        CostoUnitario DECIMAL(18,4) NULL
    );
GO

IF TYPE_ID(N'inv.tt_Serie') IS NULL
    CREATE TYPE inv.tt_Serie AS TABLE(
        ProductoID  INT           NOT NULL,
        NumeroSerie NVARCHAR(120) NOT NULL
    );
GO

------------------------------------------------------
-- 10. SP: Asignar rol a usuario
------------------------------------------------------
IF OBJECT_ID('inv.sp_AsignarRolUsuario','P') IS NOT NULL 
    DROP PROCEDURE inv.sp_AsignarRolUsuario;
GO

CREATE PROCEDURE inv.sp_AsignarRolUsuario
    @UsuarioID INT,
    @NombreRol NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RolID INT = (SELECT RolID FROM inv.Roles WHERE NombreRol = @NombreRol);
    IF @RolID IS NULL
    BEGIN
        RAISERROR(N'El rol especificado no existe.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM inv.Usuarios WHERE UsuarioID = @UsuarioID)
    BEGIN
        RAISERROR(N'Usuario no existe.', 16, 1);
        RETURN;
    END

    UPDATE inv.Usuarios 
       SET RolID = @RolID 
     WHERE UsuarioID = @UsuarioID;
END
GO

------------------------------------------------------
-- 11. Datos de ejemplo para Roles y Usuarios
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM inv.Roles WHERE NombreRol = 'Administrador')
    INSERT INTO inv.Roles (NombreRol) VALUES ('Administrador');

IF NOT EXISTS (SELECT 1 FROM inv.Roles WHERE NombreRol = 'Tecnico')
    INSERT INTO inv.Roles (NombreRol) VALUES ('Tecnico');
GO

IF NOT EXISTS (SELECT 1 FROM inv.Usuarios)
BEGIN
    DECLARE @RolAdmin INT = (SELECT RolID FROM inv.Roles WHERE NombreRol = 'Administrador');
    DECLARE @RolTec   INT = (SELECT RolID FROM inv.Roles WHERE NombreRol = 'Tecnico');

    INSERT INTO inv.Usuarios (Nombre, RolID, Activo)
    VALUES ('Joseph Admin',  @RolAdmin, 1),
           ('Jason Tecnico', @RolTec,   1);
END
GO




------------------------------------------------------
-- iindices sobre catlogos
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Usuarios_RolID' AND object_id = OBJECT_ID('inv.Usuarios'))
    CREATE NONCLUSTERED INDEX IX_Usuarios_RolID
        ON inv.Usuarios(RolID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Bodegas_Nombre' AND object_id = OBJECT_ID('inv.Bodegas'))
    CREATE NONCLUSTERED INDEX IX_Bodegas_Nombre
        ON inv.Bodegas(Nombre);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Proveedores_Nombre' AND object_id = OBJECT_ID('inv.Proveedores'))
    CREATE NONCLUSTERED INDEX IX_Proveedores_Nombre
        ON inv.Proveedores(Nombre);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Productos_Nombre' AND object_id = OBJECT_ID('inv.Productos'))
    CREATE NONCLUSTERED INDEX IX_Productos_Nombre
        ON inv.Productos(Nombre);
GO

------------------------------------------------------
-- ondices para movimientos (Rotacion90d)
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Movimientos_Fecha_Tipo' AND object_id = OBJECT_ID('inv.Movimientos'))
    CREATE NONCLUSTERED INDEX IX_Movimientos_Fecha_Tipo
        ON inv.Movimientos(FechaMovimientoUTC, TipoMovimientoID, BodegaID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Movimientos_Usuario' AND object_id = OBJECT_ID('inv.Movimientos'))
    CREATE NONCLUSTERED INDEX IX_Movimientos_Usuario
        ON inv.Movimientos(UsuarioID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovimientoDetalle_Mov_Prod' AND object_id = OBJECT_ID('inv.MovimientoDetalle'))
    CREATE NONCLUSTERED INDEX IX_MovimientoDetalle_Mov_Prod
        ON inv.MovimientoDetalle(MovimientoID, ProductoID)
        INCLUDE (Cantidad, CostoUnitario);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_TiposMovimiento_Naturaleza' AND object_id = OBJECT_ID('inv.TiposMovimiento'))
    CREATE NONCLUSTERED INDEX IX_TiposMovimiento_Naturaleza
        ON inv.TiposMovimiento(Naturaleza);
GO

------------------------------------------------------
-- indices para series
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Series_NumeroSerie' AND object_id = OBJECT_ID('inv.Series'))
    CREATE NONCLUSTERED INDEX IX_Series_NumeroSerie
        ON inv.Series(NumeroSerie);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovDetalleSeries_MovDetalle' AND object_id = OBJECT_ID('inv.MovimientoDetalleSeries'))
    CREATE NONCLUSTERED INDEX IX_MovDetalleSeries_MovDetalle
        ON inv.MovimientoDetalleSeries(MovimientoDetalleID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MovDetalleSeries_Serie' AND object_id = OBJECT_ID('inv.MovimientoDetalleSeries'))
    CREATE NONCLUSTERED INDEX IX_MovDetalleSeries_Serie
        ON inv.MovimientoDetalleSeries(SerieID);
GO

------------------------------------------------------
-- indices para despachos
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Despachos_Solicitud' AND object_id = OBJECT_ID('inv.Despachos'))
    CREATE NONCLUSTERED INDEX IX_Despachos_Solicitud
        ON inv.Despachos(SolicitudID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Despachos_Bodegas' AND object_id = OBJECT_ID('inv.Despachos'))
    CREATE NONCLUSTERED INDEX IX_Despachos_Bodegas
        ON inv.Despachos(BodegaOrigenID, BodegaDestinoID);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Despachos_Fecha' AND object_id = OBJECT_ID('inv.Despachos'))
    CREATE NONCLUSTERED INDEX IX_Despachos_Fecha
        ON inv.Despachos(FechaCreacionUTC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_DespachoDetalle_Despacho' AND object_id = OBJECT_ID('inv.DespachoDetalle'))
    CREATE NONCLUSTERED INDEX IX_DespachoDetalle_Despacho
        ON inv.DespachoDetalle(DespachoID);
GO

------------------------------------------------------
-- indices para KPI de solicitudes
------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Solicitudes_Estado_Fecha' AND object_id = OBJECT_ID('inv.Solicitudes'))
    CREATE NONCLUSTERED INDEX IX_Solicitudes_Estado_Fecha
        ON inv.Solicitudes(EstadoSolicitudID, FechaCreacionUTC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SolicitudDetalle_Solicitud_Producto' AND object_id = OBJECT_ID('inv.SolicitudDetalle'))
    CREATE NONCLUSTERED INDEX IX_SolicitudDetalle_Solicitud_Producto
        ON inv.SolicitudDetalle(SolicitudID, ProductoID)
        INCLUDE (CantidadSolicitada);
GO
