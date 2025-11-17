-- ===========================================
-- CREACIÓN BASE Y ESQUEMA
-- ===========================================
IF DB_ID('DB_Progra_Av_Web') IS NULL
    CREATE DATABASE DB_Progra_Av_Web;
GO

USE [DB_Progra_Av_Web];
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'inv')
    EXEC('CREATE SCHEMA inv');
GO

-- ===========================================
-- TABLAS BÁSICAS MÍNIMAS
-- ===========================================

IF OBJECT_ID('inv.Roles') IS NULL
CREATE TABLE inv.Roles(
    RolID INT IDENTITY PRIMARY KEY,
    NombreRol NVARCHAR(50) NOT NULL UNIQUE
);
GO

IF OBJECT_ID('inv.Usuarios') IS NULL
CREATE TABLE inv.Usuarios(
    UsuarioID INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    RolID INT NULL,
    Activo BIT DEFAULT 1,
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (RolID) REFERENCES inv.Roles(RolID)
);
GO

-- Insertar roles base si no existen
IF NOT EXISTS (SELECT 1 FROM inv.Roles WHERE NombreRol = 'Administrador')
INSERT INTO inv.Roles (NombreRol) VALUES ('Administrador');
IF NOT EXISTS (SELECT 1 FROM inv.Roles WHERE NombreRol = 'Vendedor')
INSERT INTO inv.Roles (NombreRol) VALUES ('Vendedor');
GO

-- Insertar usuarios de ejemplo
IF NOT EXISTS (SELECT 1 FROM inv.Usuarios)
BEGIN
    DECLARE @RolAdmin INT = (SELECT RolID FROM inv.Roles WHERE NombreRol='Administrador');
    DECLARE @RolVend INT = (SELECT RolID FROM inv.Roles WHERE NombreRol='Vendedor');

    INSERT INTO inv.Usuarios (Nombre, RolID, Activo)
    VALUES ('Walter Admin', @RolAdmin, 1),
           ('Pedro Vendedor', @RolVend, 1);
END
GO

-- ===========================================
-- RESTO DE TABLAS
-- ===========================================

IF OBJECT_ID('inv.Bodegas') IS NULL
CREATE TABLE inv.Bodegas(
    BodegaID INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100),
    Activo BIT DEFAULT 1
);
GO

IF OBJECT_ID('inv.Proveedores') IS NULL
CREATE TABLE inv.Proveedores(
    ProveedorID INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100),
    Activo BIT DEFAULT 1
);
GO

IF OBJECT_ID('inv.UnidadesMedida') IS NULL
CREATE TABLE inv.UnidadesMedida(
    UnidadID INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50)
);
GO

IF OBJECT_ID('inv.Productos') IS NULL
CREATE TABLE inv.Productos(
    ProductoID INT IDENTITY PRIMARY KEY,
    SKU NVARCHAR(50),
    Nombre NVARCHAR(100),
    EsSerializado BIT DEFAULT 0
);
GO

IF OBJECT_ID('inv.TiposMovimiento') IS NULL
CREATE TABLE inv.TiposMovimiento(
    TipoMovimientoID INT IDENTITY PRIMARY KEY,
    Codigo NVARCHAR(50),
    Naturaleza INT
);
GO

IF OBJECT_ID('inv.CierresAuditoria') IS NULL
CREATE TABLE inv.CierresAuditoria(
    CierreAuditoriaID INT IDENTITY PRIMARY KEY,
    FechaHastaUTC DATETIME2,
    Activo BIT
);
GO

IF OBJECT_ID('inv.Movimientos') IS NULL
CREATE TABLE inv.Movimientos(
    MovimientoID INT IDENTITY PRIMARY KEY,
    BodegaID INT,
    TipoMovimientoID INT,
    UsuarioID INT,
    FechaMovimientoUTC DATETIME2,
    Referencia NVARCHAR(100),
    Observaciones NVARCHAR(255),
    BodegaRelacionadaID INT NULL,
    ProveedorID INT NULL
);
GO

IF OBJECT_ID('inv.MovimientoDetalle') IS NULL
CREATE TABLE inv.MovimientoDetalle(
    MovimientoDetalleID INT IDENTITY PRIMARY KEY,
    MovimientoID INT,
    ProductoID INT,
    UbicacionID INT NULL,
    UnidadID INT,
    Cantidad DECIMAL(18,4),
    CostoUnitario DECIMAL(18,4),
    MotivoAjusteID INT NULL
);
GO

IF OBJECT_ID('inv.Series') IS NULL
CREATE TABLE inv.Series(
    SerieID INT IDENTITY PRIMARY KEY,
    ProductoID INT,
    NumeroSerie NVARCHAR(120)
);
GO

IF OBJECT_ID('inv.MovimientoDetalleSeries') IS NULL
CREATE TABLE inv.MovimientoDetalleSeries(
    MovimientoDetalleSerieID INT IDENTITY PRIMARY KEY,
    MovimientoDetalleID INT,
    SerieID INT
);
GO

IF OBJECT_ID('inv.Despachos') IS NULL
CREATE TABLE inv.Despachos(
    DespachoID INT IDENTITY PRIMARY KEY,
    SolicitudID INT NULL,
    NumeroEnvio NVARCHAR(60),
    BodegaOrigenID INT,
    BodegaDestinoID INT,
    Estado NVARCHAR(50),
    UsuarioCreadorID INT,
    FechaCreacionUTC DATETIME2,
    FechaDespachoUTC DATETIME2
);
GO

IF OBJECT_ID('inv.DespachoDetalle') IS NULL
CREATE TABLE inv.DespachoDetalle(
    DespachoDetalleID INT IDENTITY PRIMARY KEY,
    DespachoID INT,
    ProductoID INT,
    Cantidad DECIMAL(18,4)
);
GO

-- ===========================================
-- VISTA DE SALDO DE INVENTARIO
-- ===========================================
IF OBJECT_ID('inv.v_SaldoInventario') IS NOT NULL DROP VIEW inv.v_SaldoInventario;
GO
CREATE VIEW inv.v_SaldoInventario AS
SELECT 1 AS BodegaID, 1 AS ProductoID, 100.0 AS Cantidad;
GO

-- ===========================================
-- TABLAS PARA KPI
-- ===========================================
IF OBJECT_ID('inv.Solicitudes') IS NULL
CREATE TABLE inv.Solicitudes(
    SolicitudID INT IDENTITY PRIMARY KEY,
    EstadoSolicitudID INT,
    FechaCreacionUTC DATETIME2 DEFAULT SYSUTCDATETIME()
);
GO

IF OBJECT_ID('inv.SolicitudDetalle') IS NULL
CREATE TABLE inv.SolicitudDetalle(
    SolicitudDetalleID INT IDENTITY PRIMARY KEY,
    SolicitudID INT,
    ProductoID INT,
    CantidadSolicitada DECIMAL(18,4)
);
GO

IF OBJECT_ID('inv.EstadosSolicitud') IS NULL
CREATE TABLE inv.EstadosSolicitud(
    EstadoSolicitudID INT IDENTITY PRIMARY KEY,
    Codigo NVARCHAR(50)
);
GO

-- ===========================================
-- TIPOS DE TABLA
-- ===========================================
IF TYPE_ID(N'inv.tt_MovimientoDetalle') IS NULL
CREATE TYPE inv.tt_MovimientoDetalle AS TABLE(
    ProductoID      INT         NOT NULL,
    UbicacionID     INT         NULL,
    UnidadID        INT         NOT NULL,
    Cantidad        DECIMAL(18,4) NOT NULL,
    CostoUnitario   DECIMAL(18,4) NULL
);
GO

IF TYPE_ID(N'inv.tt_Serie') IS NULL
CREATE TYPE inv.tt_Serie AS TABLE(
    ProductoID   INT           NOT NULL,
    NumeroSerie  NVARCHAR(120) NOT NULL
);
GO

-- ===========================================
-- PROCEDIMIENTO: Asignar rol a usuario
-- ===========================================
IF OBJECT_ID('inv.sp_AsignarRolUsuario','P') IS NOT NULL DROP PROCEDURE inv.sp_AsignarRolUsuario;
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
        RAISERROR('El rol especificado no existe.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM inv.Usuarios WHERE UsuarioID = @UsuarioID)
    BEGIN
        RAISERROR('Usuario no existe.', 16, 1);
        RETURN;
    END

    UPDATE inv.Usuarios SET RolID = @RolID WHERE UsuarioID = @UsuarioID;
    PRINT 'Rol asignado correctamente.';
END
GO

-- ===========================================
-- VISTAS KPI
-- ===========================================
IF OBJECT_ID(N'inv.v_KPI_ProductosMasSolicitados','V') IS NOT NULL DROP VIEW inv.v_KPI_ProductosMasSolicitados;
GO
CREATE VIEW inv.v_KPI_ProductosMasSolicitados AS
SELECT p.ProductoID, p.SKU, p.Nombre,
       SUM(sd.CantidadSolicitada) AS TotalSolicitado,
       COUNT(DISTINCT s.SolicitudID) AS NumeroSolicitudes
FROM inv.SolicitudDetalle sd
JOIN inv.Solicitudes s ON s.SolicitudID=sd.SolicitudID
JOIN inv.EstadosSolicitud es ON es.EstadoSolicitudID=s.EstadoSolicitudID
JOIN inv.Productos p ON p.ProductoID=sd.ProductoID
WHERE es.Codigo IN (N'Aprobada',N'Enviada',N'Entregada')
GROUP BY p.ProductoID, p.SKU, p.Nombre;
GO

IF OBJECT_ID(N'inv.v_KPI_ProductosMasSolicitados90d','V') IS NOT NULL DROP VIEW inv.v_KPI_ProductosMasSolicitados90d;
GO
CREATE VIEW inv.v_KPI_ProductosMasSolicitados90d AS
SELECT p.ProductoID, p.SKU, p.Nombre,
       SUM(sd.CantidadSolicitada) AS TotalSolicitado90d,
       COUNT(DISTINCT s.SolicitudID) AS NumeroSolicitudes90d
FROM inv.SolicitudDetalle sd
JOIN inv.Solicitudes s ON s.SolicitudID=sd.SolicitudID
JOIN inv.EstadosSolicitud es ON es.EstadoSolicitudID=s.EstadoSolicitudID
JOIN inv.Productos p ON p.ProductoID=sd.ProductoID
WHERE es.Codigo IN (N'Aprobada',N'Enviada',N'Entregada')
  AND s.FechaCreacionUTC >= DATEADD(DAY,-90,SYSUTCDATETIME())
GROUP BY p.ProductoID, p.SKU, p.Nombre;
GO

IF OBJECT_ID(N'inv.v_KPI_Rotacion90d','V') IS NOT NULL DROP VIEW inv.v_KPI_Rotacion90d;
GO
CREATE VIEW inv.v_KPI_Rotacion90d AS
WITH Sal AS (
    SELECT m.BodegaID, d.ProductoID, SUM(d.Cantidad) AS Salidas90d
    FROM inv.Movimientos m
    JOIN inv.MovimientoDetalle d ON d.MovimientoID=m.MovimientoID
    JOIN inv.TiposMovimiento tm ON tm.TipoMovimientoID=m.TipoMovimientoID
    WHERE tm.Naturaleza=-1 AND m.FechaMovimientoUTC >= DATEADD(DAY,-90,SYSUTCDATETIME())
    GROUP BY m.BodegaID, d.ProductoID
),
Stock AS (
    SELECT BodegaID, ProductoID, SUM(Cantidad) AS StockActual
    FROM inv.v_SaldoInventario
    GROUP BY BodegaID, ProductoID
)
SELECT COALESCE(s.BodegaID, st.BodegaID) AS BodegaID,
       COALESCE(s.ProductoID, st.ProductoID) AS ProductoID,
       p.SKU, p.Nombre,
       ISNULL(Salidas90d,0) AS Salidas90d,
       ISNULL(StockActual,0) AS StockActual,
       CASE WHEN ISNULL(StockActual,0)>0 THEN CAST(ISNULL(Salidas90d,0) AS DECIMAL(18,4))/NULLIF(StockActual,0) END AS Rotacion90d
FROM Sal s
FULL OUTER JOIN Stock st ON st.BodegaID=s.BodegaID AND st.ProductoID=s.ProductoID
JOIN inv.Productos p ON p.ProductoID=COALESCE(s.ProductoID, st.ProductoID);
