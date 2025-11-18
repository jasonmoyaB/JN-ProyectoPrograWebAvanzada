-- Script de diagnóstico para problemas de login
-- Ejecuta este script en tu base de datos DBInventario

USE DBInventario;
GO

PRINT '========================================';
PRINT 'DIAGNÓSTICO DE LOGIN';
PRINT '========================================';
GO

-- 1. Verificar si la columna BodegaID existe en Usuarios
PRINT '';
PRINT '1. Verificando columna BodegaID en inv.Usuarios...';
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'BodegaID')
BEGIN
    PRINT '   ✓ La columna BodegaID EXISTE en inv.Usuarios';
END
ELSE
BEGIN
    PRINT '   ✗ La columna BodegaID NO EXISTE en inv.Usuarios';
    PRINT '   → Ejecuta el script CompletarBaseDatos.sql para agregarla';
END
GO

-- 2. Verificar columnas en Usuarios
PRINT '';
PRINT '2. Columnas en inv.Usuarios:';
SELECT 
    c.name AS NombreColumna,
    t.name AS TipoDatos,
    c.is_nullable AS PermiteNULL
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('inv.Usuarios')
ORDER BY c.column_id;
GO

-- 3. Verificar usuarios existentes
PRINT '';
PRINT '3. Usuarios en la base de datos:';
SELECT 
    UsuarioID,
    Nombre,
    CorreoElectronico,
    CASE WHEN LEN(ContrasenaHash) > 0 THEN 'Sí tiene hash' ELSE 'NO tiene hash' END AS TieneContrasena,
    RolID,
    Activo,
    BodegaID,
    FechaRegistro
FROM inv.Usuarios;
GO

-- 4. Verificar roles
PRINT '';
PRINT '4. Roles disponibles:';
SELECT RolID, NombreRol FROM inv.Roles ORDER BY RolID;
GO

-- 5. Verificar bodegas
PRINT '';
PRINT '5. Bodegas disponibles:';
SELECT BodegaID, Nombre, Activo FROM inv.Bodegas ORDER BY BodegaID;
GO

-- 6. Probar query de login (usar un correo que exista)
PRINT '';
PRINT '6. Test de query de login (ajusta el correo):';
DECLARE @Email NVARCHAR(100) = 'josalazar09@hotmail.com'; -- CAMBIAR ESTO
DECLARE @PasswordHash NVARCHAR(255) = ''; -- Este hash debe coincidir con el almacenado

-- Si BodegaID existe
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('inv.Usuarios') AND name = 'BodegaID')
BEGIN
    SELECT 
        u.UsuarioID, 
        u.Nombre, 
        u.CorreoElectronico, 
        u.RolID, 
        u.Activo, 
        u.BodegaID,
        r.NombreRol AS RolNombre,
        b.Nombre AS BodegaNombre
    FROM inv.Usuarios u
    LEFT JOIN inv.Roles r ON u.RolID = r.RolID
    LEFT JOIN inv.Bodegas b ON u.BodegaID = b.BodegaID
    WHERE u.CorreoElectronico = @Email AND u.Activo = 1;
END
ELSE
BEGIN
    SELECT 
        u.UsuarioID, 
        u.Nombre, 
        u.CorreoElectronico, 
        u.RolID, 
        u.Activo,
        r.NombreRol AS RolNombre
    FROM inv.Usuarios u
    LEFT JOIN inv.Roles r ON u.RolID = r.RolID
    WHERE u.CorreoElectronico = @Email AND u.Activo = 1;
END
GO

-- 7. Verificar hash de contraseña
PRINT '';
PRINT '7. Para generar un hash de contraseña, usa este código C#:';
PRINT '   PasswordHelper.HashPassword("tu_contraseña")';
PRINT '';
PRINT '   O ejecuta este script SQL para generar un hash SHA256:';
PRINT '   (Nota: SQL Server no tiene SHA256 nativo, usa el código C#)';
GO

PRINT '';
PRINT '========================================';
PRINT 'DIAGNÓSTICO COMPLETADO';
PRINT '========================================';
GO

