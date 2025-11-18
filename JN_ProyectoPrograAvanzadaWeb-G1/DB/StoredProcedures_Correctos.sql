USE DBInventario;
GO

IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'inv')
BEGIN
    EXEC('CREATE SCHEMA inv');
END
GO

-- ===========================================
-- STORED PROCEDURES PARA USUARIOS
-- ===========================================

IF OBJECT_ID('inv.sp_Usuario_GetById', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_GetById;
GO

CREATE PROCEDURE inv.sp_Usuario_GetById
    @UsuarioID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT u.UsuarioID, u.Nombre, u.CorreoElectronico, u.ContrasenaHash, 
           u.RolID, u.Activo, u.FechaRegistro,
           r.RolID, r.NombreRol
    FROM inv.Usuarios u
    LEFT JOIN inv.Roles r ON u.RolID = r.RolID
    WHERE u.UsuarioID = @UsuarioID;
END
GO

IF OBJECT_ID('inv.sp_Usuario_GetByEmail', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_GetByEmail;
GO

CREATE PROCEDURE inv.sp_Usuario_GetByEmail
    @Email NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT u.UsuarioID, u.Nombre, u.CorreoElectronico, u.ContrasenaHash, 
           u.RolID, u.Activo, u.FechaRegistro,
           r.RolID, r.NombreRol
    FROM inv.Usuarios u
    LEFT JOIN inv.Roles r ON u.RolID = r.RolID
    WHERE LOWER(LTRIM(RTRIM(u.CorreoElectronico))) = LOWER(LTRIM(RTRIM(@Email)));
END
GO

IF OBJECT_ID('inv.sp_Usuario_GetByEmailAndPassword', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_GetByEmailAndPassword;
GO

CREATE PROCEDURE inv.sp_Usuario_GetByEmailAndPassword
    @Email NVARCHAR(100),
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT u.UsuarioID, u.Nombre, u.CorreoElectronico, u.ContrasenaHash, 
           u.RolID, u.Activo, u.FechaRegistro,
           r.RolID AS Rol_RolID, r.NombreRol AS Rol_NombreRol
    FROM inv.Usuarios u
    LEFT JOIN inv.Roles r ON u.RolID = r.RolID
    WHERE u.CorreoElectronico = @Email 
      AND u.ContrasenaHash = @PasswordHash 
      AND u.Activo = 1;
END
GO

IF OBJECT_ID('inv.sp_Usuario_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_GetAll;
GO

CREATE PROCEDURE inv.sp_Usuario_GetAll
    @Activo BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT u.UsuarioID, u.Nombre, u.CorreoElectronico, u.ContrasenaHash, 
           u.RolID, u.Activo, u.FechaRegistro,
           r.RolID AS Rol_RolID, r.NombreRol AS Rol_NombreRol
    FROM inv.Usuarios u
    LEFT JOIN inv.Roles r ON u.RolID = r.RolID
    WHERE (@Activo IS NULL OR u.Activo = @Activo)
    ORDER BY u.Nombre;
END
GO

IF OBJECT_ID('inv.sp_Usuario_GetByRolId', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_GetByRolId;
GO

CREATE PROCEDURE inv.sp_Usuario_GetByRolId
    @RolID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT u.UsuarioID, u.Nombre, u.CorreoElectronico, u.ContrasenaHash, 
           u.RolID, u.Activo, u.FechaRegistro,
           r.RolID, r.NombreRol
    FROM inv.Usuarios u
    LEFT JOIN inv.Roles r ON u.RolID = r.RolID
    WHERE u.RolID = @RolID AND u.Activo = 1
    ORDER BY u.Nombre;
END
GO

IF OBJECT_ID('inv.sp_Usuario_GetByBodegaId', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_GetByBodegaId;
GO

CREATE PROCEDURE inv.sp_Usuario_GetByBodegaId
    @BodegaID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT NULL AS UsuarioID, NULL AS Nombre, NULL AS CorreoElectronico, NULL AS ContrasenaHash, 
           NULL AS RolID, NULL AS Activo, NULL AS FechaRegistro,
           NULL AS RolID, NULL AS NombreRol
    WHERE 1 = 0;
END
GO

IF OBJECT_ID('inv.sp_Usuario_Create', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_Create;
GO

CREATE PROCEDURE inv.sp_Usuario_Create
    @Nombre NVARCHAR(100),
    @CorreoElectronico NVARCHAR(100),
    @ContrasenaHash NVARCHAR(255),
    @RolID INT,
    @Activo BIT = 1,
    @BodegaID INT = NULL,
    @UsuarioID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO inv.Usuarios (Nombre, CorreoElectronico, ContrasenaHash, RolID, Activo, FechaRegistro)
    VALUES (@Nombre, @CorreoElectronico, @ContrasenaHash, @RolID, @Activo, SYSUTCDATETIME());
    
    SET @UsuarioID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('inv.sp_Usuario_Update', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_Update;
GO

CREATE PROCEDURE inv.sp_Usuario_Update
    @UsuarioID INT,
    @Nombre NVARCHAR(100),
    @CorreoElectronico NVARCHAR(100),
    @RolID INT,
    @Activo BIT,
    @BodegaID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE inv.Usuarios
    SET Nombre = @Nombre,
        CorreoElectronico = @CorreoElectronico,
        RolID = @RolID,
        Activo = @Activo
    WHERE UsuarioID = @UsuarioID;
END
GO

IF OBJECT_ID('inv.sp_Usuario_UpdatePassword', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_UpdatePassword;
GO

CREATE PROCEDURE inv.sp_Usuario_UpdatePassword
    @UsuarioID INT,
    @PasswordHash NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE inv.Usuarios
    SET ContrasenaHash = @PasswordHash
    WHERE UsuarioID = @UsuarioID;
END
GO

IF OBJECT_ID('inv.sp_Usuario_ToggleActivo', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_ToggleActivo;
GO

CREATE PROCEDURE inv.sp_Usuario_ToggleActivo
    @UsuarioID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE inv.Usuarios
    SET Activo = CASE WHEN Activo = 1 THEN 0 ELSE 1 END
    WHERE UsuarioID = @UsuarioID;
END
GO

IF OBJECT_ID('inv.sp_Usuario_AssignarBodega', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Usuario_AssignarBodega;
GO

CREATE PROCEDURE inv.sp_Usuario_AssignarBodega
    @UsuarioID INT,
    @BodegaID INT
AS
BEGIN
    SET NOCOUNT ON;
    
END
GO

-- ===========================================
-- STORED PROCEDURES PARA BODEGAS
-- ===========================================

IF OBJECT_ID('inv.sp_Bodega_GetById', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Bodega_GetById;
GO

CREATE PROCEDURE inv.sp_Bodega_GetById
    @BodegaID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT BodegaID, Nombre, Activo
    FROM inv.Bodegas
    WHERE BodegaID = @BodegaID;
END
GO

IF OBJECT_ID('inv.sp_Bodega_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Bodega_GetAll;
GO

CREATE PROCEDURE inv.sp_Bodega_GetAll
    @Activo BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT BodegaID, Nombre, Activo
    FROM inv.Bodegas
    WHERE (@Activo IS NULL OR Activo = @Activo)
    ORDER BY Nombre;
END
GO

IF OBJECT_ID('inv.sp_Bodega_Create', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Bodega_Create;
GO

CREATE PROCEDURE inv.sp_Bodega_Create
    @Nombre NVARCHAR(100),
    @Ubicacion NVARCHAR(255) = NULL,
    @Activo BIT = 1,
    @BodegaID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO inv.Bodegas (Nombre, Activo)
    VALUES (@Nombre, @Activo);
    
    SET @BodegaID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('inv.sp_Bodega_Update', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Bodega_Update;
GO

CREATE PROCEDURE inv.sp_Bodega_Update
    @BodegaID INT,
    @Nombre NVARCHAR(100),
    @Ubicacion NVARCHAR(255) = NULL,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE inv.Bodegas
    SET Nombre = @Nombre,
        Activo = @Activo
    WHERE BodegaID = @BodegaID;
END
GO

IF OBJECT_ID('inv.sp_Bodega_ToggleActivo', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Bodega_ToggleActivo;
GO

CREATE PROCEDURE inv.sp_Bodega_ToggleActivo
    @BodegaID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE inv.Bodegas
    SET Activo = CASE WHEN Activo = 1 THEN 0 ELSE 1 END
    WHERE BodegaID = @BodegaID;
END
GO

IF OBJECT_ID('inv.sp_Bodega_Exists', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Bodega_Exists;
GO

CREATE PROCEDURE inv.sp_Bodega_Exists
    @BodegaID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1)
    FROM inv.Bodegas
    WHERE BodegaID = @BodegaID;
END
GO

-- ===========================================
-- STORED PROCEDURES PARA ROLES
-- ===========================================

IF OBJECT_ID('inv.sp_Rol_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Rol_GetAll;
GO

CREATE PROCEDURE inv.sp_Rol_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT RolID, NombreRol
    FROM inv.Roles
    ORDER BY NombreRol;
END
GO

IF OBJECT_ID('inv.sp_Rol_GetById', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Rol_GetById;
GO

CREATE PROCEDURE inv.sp_Rol_GetById
    @RolID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT RolID, NombreRol
    FROM inv.Roles
    WHERE RolID = @RolID;
END
GO

IF OBJECT_ID('inv.sp_Rol_GetByNombre', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Rol_GetByNombre;
GO

CREATE PROCEDURE inv.sp_Rol_GetByNombre
    @NombreRol NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT RolID, NombreRol
    FROM inv.Roles
    WHERE NombreRol = @NombreRol;
END
GO

-- ===========================================
-- STORED PROCEDURES PARA PRODUCTOS
-- ===========================================

IF OBJECT_ID('inv.sp_Producto_GetById', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Producto_GetById;
GO

CREATE PROCEDURE inv.sp_Producto_GetById
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ProductoID, SKU, Nombre, EsSerializado
    FROM inv.Productos
    WHERE ProductoID = @ProductoID;
END
GO

IF OBJECT_ID('inv.sp_Producto_GetBySku', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Producto_GetBySku;
GO

CREATE PROCEDURE inv.sp_Producto_GetBySku
    @SKU NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ProductoID, SKU, Nombre, EsSerializado
    FROM inv.Productos
    WHERE SKU = @SKU;
END
GO

IF OBJECT_ID('inv.sp_Producto_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Producto_GetAll;
GO

CREATE PROCEDURE inv.sp_Producto_GetAll
    @Activo BIT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ProductoID, SKU, Nombre, EsSerializado
    FROM inv.Productos
    ORDER BY Nombre;
END
GO

IF OBJECT_ID('inv.sp_Producto_Search', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Producto_Search;
GO

CREATE PROCEDURE inv.sp_Producto_Search
    @SearchTerm NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT ProductoID, SKU, Nombre, EsSerializado
    FROM inv.Productos
    WHERE (Nombre LIKE @SearchTerm OR SKU LIKE @SearchTerm)
    ORDER BY Nombre;
END
GO

IF OBJECT_ID('inv.sp_Producto_Create', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Producto_Create;
GO

CREATE PROCEDURE inv.sp_Producto_Create
    @SKU NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(500) = NULL,
    @EsSerializado BIT = 0,
    @Activo BIT = 1,
    @ProductoID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO inv.Productos (SKU, Nombre, EsSerializado)
    VALUES (@SKU, @Nombre, @EsSerializado);
    
    SET @ProductoID = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('inv.sp_Producto_Update', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Producto_Update;
GO

CREATE PROCEDURE inv.sp_Producto_Update
    @ProductoID INT,
    @SKU NVARCHAR(50),
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(500) = NULL,
    @EsSerializado BIT,
    @Activo BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE inv.Productos
    SET SKU = @SKU,
        Nombre = @Nombre,
        EsSerializado = @EsSerializado
    WHERE ProductoID = @ProductoID;
END
GO

IF OBJECT_ID('inv.sp_Producto_ToggleActivo', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Producto_ToggleActivo;
GO

CREATE PROCEDURE inv.sp_Producto_ToggleActivo
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;
    
END
GO

IF OBJECT_ID('inv.sp_Producto_Exists', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Producto_Exists;
GO

CREATE PROCEDURE inv.sp_Producto_Exists
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT COUNT(1)
    FROM inv.Productos
    WHERE ProductoID = @ProductoID;
END
GO

-- ===========================================
-- STORED PROCEDURES PARA INVENTARIO
-- ===========================================

IF OBJECT_ID('inv.sp_Inventario_GetSaldoByBodega', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Inventario_GetSaldoByBodega;
GO

CREATE PROCEDURE inv.sp_Inventario_GetSaldoByBodega
    @BodegaID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.v_SaldoInventario', 'V') IS NOT NULL
    BEGIN
        SELECT si.BodegaID, b.Nombre AS BodegaNombre, 
               si.ProductoID, p.Nombre AS ProductoNombre, p.SKU AS ProductoSKU,
               si.Cantidad,
               NULL AS StockMinimo,
               0 AS AlertaStockBajo
        FROM inv.v_SaldoInventario si
        INNER JOIN inv.Bodegas b ON si.BodegaID = b.BodegaID
        INNER JOIN inv.Productos p ON si.ProductoID = p.ProductoID
        WHERE si.BodegaID = @BodegaID
        ORDER BY p.Nombre;
    END
END
GO

IF OBJECT_ID('inv.sp_Inventario_GetSaldoByBodegaAndProducto', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Inventario_GetSaldoByBodegaAndProducto;
GO

CREATE PROCEDURE inv.sp_Inventario_GetSaldoByBodegaAndProducto
    @BodegaID INT,
    @ProductoID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.v_SaldoInventario', 'V') IS NOT NULL
    BEGIN
        SELECT si.BodegaID, b.Nombre AS BodegaNombre, 
               si.ProductoID, p.Nombre AS ProductoNombre, p.SKU AS ProductoSKU,
               si.Cantidad,
               NULL AS StockMinimo,
               0 AS AlertaStockBajo
        FROM inv.v_SaldoInventario si
        INNER JOIN inv.Bodegas b ON si.BodegaID = b.BodegaID
        INNER JOIN inv.Productos p ON si.ProductoID = p.ProductoID
        WHERE si.BodegaID = @BodegaID AND si.ProductoID = @ProductoID;
    END
END
GO

IF OBJECT_ID('inv.sp_Inventario_GetSaldosConAlertaStockBajo', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Inventario_GetSaldosConAlertaStockBajo;
GO

CREATE PROCEDURE inv.sp_Inventario_GetSaldosConAlertaStockBajo
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.v_SaldoInventario', 'V') IS NOT NULL
    BEGIN
        SELECT si.BodegaID, b.Nombre AS BodegaNombre, 
               si.ProductoID, p.Nombre AS ProductoNombre, p.SKU AS ProductoSKU,
               si.Cantidad,
               NULL AS StockMinimo,
               0 AS AlertaStockBajo
        FROM inv.v_SaldoInventario si
        INNER JOIN inv.Bodegas b ON si.BodegaID = b.BodegaID
        INNER JOIN inv.Productos p ON si.ProductoID = p.ProductoID
        WHERE 1 = 0;
    END
END
GO

-- ===========================================
-- STORED PROCEDURES PARA AUDITORIA
-- ===========================================

IF OBJECT_ID('inv.sp_Auditoria_Registrar', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Auditoria_Registrar;
GO

CREATE PROCEDURE inv.sp_Auditoria_Registrar
    @UsuarioID INT = NULL,
    @Accion NVARCHAR(50),
    @Tabla NVARCHAR(100),
    @RegistroID INT = NULL,
    @DatosAntes NVARCHAR(MAX) = NULL,
    @DatosDespues NVARCHAR(MAX) = NULL,
    @IPAddress NVARCHAR(50) = NULL,
    @UserAgent NVARCHAR(500) = NULL,
    @AuditoriaID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.Auditoria', 'U') IS NOT NULL
    BEGIN
        INSERT INTO inv.Auditoria (UsuarioID, Accion, Tabla, RegistroID, DatosAntes, DatosDespues, FechaUTC, IPAddress, UserAgent)
        VALUES (@UsuarioID, @Accion, @Tabla, @RegistroID, @DatosAntes, @DatosDespues, SYSUTCDATETIME(), @IPAddress, @UserAgent);
        
        SET @AuditoriaID = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        SET @AuditoriaID = 0;
    END
END
GO

IF OBJECT_ID('inv.sp_Auditoria_GetByUsuario', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Auditoria_GetByUsuario;
GO

CREATE PROCEDURE inv.sp_Auditoria_GetByUsuario
    @UsuarioID INT,
    @Desde DATETIME2 = NULL,
    @Hasta DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.Auditoria', 'U') IS NOT NULL
    BEGIN
        SELECT AuditoriaID, UsuarioID, Accion, Tabla, RegistroID, DatosAntes, DatosDespues, FechaUTC, IPAddress, UserAgent
        FROM inv.Auditoria
        WHERE UsuarioID = @UsuarioID
          AND (@Desde IS NULL OR FechaUTC >= @Desde)
          AND (@Hasta IS NULL OR FechaUTC <= @Hasta)
        ORDER BY FechaUTC DESC;
    END
END
GO

IF OBJECT_ID('inv.sp_Auditoria_GetByAccion', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Auditoria_GetByAccion;
GO

CREATE PROCEDURE inv.sp_Auditoria_GetByAccion
    @Accion NVARCHAR(50),
    @Desde DATETIME2 = NULL,
    @Hasta DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.Auditoria', 'U') IS NOT NULL
    BEGIN
        SELECT AuditoriaID, UsuarioID, Accion, Tabla, RegistroID, DatosAntes, DatosDespues, FechaUTC, IPAddress, UserAgent
        FROM inv.Auditoria
        WHERE Accion = @Accion
          AND (@Desde IS NULL OR FechaUTC >= @Desde)
          AND (@Hasta IS NULL OR FechaUTC <= @Hasta)
        ORDER BY FechaUTC DESC;
    END
END
GO

IF OBJECT_ID('inv.sp_Auditoria_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_Auditoria_GetAll;
GO

CREATE PROCEDURE inv.sp_Auditoria_GetAll
    @Desde DATETIME2 = NULL,
    @Hasta DATETIME2 = NULL,
    @Limit INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.Auditoria', 'U') IS NOT NULL
    BEGIN
        SELECT TOP (ISNULL(@Limit, 1000))
               AuditoriaID, UsuarioID, Accion, Tabla, RegistroID, DatosAntes, DatosDespues, FechaUTC, IPAddress, UserAgent
        FROM inv.Auditoria
        WHERE (@Desde IS NULL OR FechaUTC >= @Desde)
          AND (@Hasta IS NULL OR FechaUTC <= @Hasta)
        ORDER BY FechaUTC DESC;
    END
END
GO

-- ===========================================
-- STORED PROCEDURES PARA BITACORA ERRORES
-- ===========================================

IF OBJECT_ID('inv.sp_BitacoraErrores_Registrar', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_BitacoraErrores_Registrar;
GO

CREATE PROCEDURE inv.sp_BitacoraErrores_Registrar
    @UsuarioID INT = NULL,
    @Error NVARCHAR(MAX),
    @StackTrace NVARCHAR(MAX) = NULL,
    @Controlador NVARCHAR(100) = NULL,
    @Accion NVARCHAR(100) = NULL,
    @RequestPath NVARCHAR(500) = NULL,
    @UserAgent NVARCHAR(500) = NULL,
    @BitacoraErrorID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.BitacoraErrores', 'U') IS NOT NULL
    BEGIN
        INSERT INTO inv.BitacoraErrores (UsuarioID, Error, StackTrace, Controlador, Accion, FechaUTC, RequestPath, UserAgent)
        VALUES (@UsuarioID, @Error, @StackTrace, @Controlador, @Accion, SYSUTCDATETIME(), @RequestPath, @UserAgent);
        
        SET @BitacoraErrorID = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        SET @BitacoraErrorID = 0;
    END
END
GO

IF OBJECT_ID('inv.sp_BitacoraErrores_GetByUsuario', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_BitacoraErrores_GetByUsuario;
GO

CREATE PROCEDURE inv.sp_BitacoraErrores_GetByUsuario
    @UsuarioID INT = NULL,
    @Desde DATETIME2 = NULL,
    @Hasta DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.BitacoraErrores', 'U') IS NOT NULL
    BEGIN
        SELECT BitacoraErrorID, UsuarioID, Error, StackTrace, Controlador, Accion, FechaUTC, RequestPath, UserAgent
        FROM inv.BitacoraErrores
        WHERE (@UsuarioID IS NULL OR UsuarioID = @UsuarioID)
          AND (@Desde IS NULL OR FechaUTC >= @Desde)
          AND (@Hasta IS NULL OR FechaUTC <= @Hasta)
        ORDER BY FechaUTC DESC;
    END
END
GO

IF OBJECT_ID('inv.sp_BitacoraErrores_GetAll', 'P') IS NOT NULL
    DROP PROCEDURE inv.sp_BitacoraErrores_GetAll;
GO

CREATE PROCEDURE inv.sp_BitacoraErrores_GetAll
    @Desde DATETIME2 = NULL,
    @Hasta DATETIME2 = NULL,
    @Limit INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    
    IF OBJECT_ID('inv.BitacoraErrores', 'U') IS NOT NULL
    BEGIN
        SELECT TOP (@Limit)
               BitacoraErrorID, UsuarioID, Error, StackTrace, Controlador, Accion, FechaUTC, RequestPath, UserAgent
        FROM inv.BitacoraErrores
        WHERE (@Desde IS NULL OR FechaUTC >= @Desde)
          AND (@Hasta IS NULL OR FechaUTC <= @Hasta)
        ORDER BY FechaUTC DESC;
    END
END
GO

PRINT 'Stored Procedures creados exitosamente basados en la estructura actual de la base de datos';
GO

