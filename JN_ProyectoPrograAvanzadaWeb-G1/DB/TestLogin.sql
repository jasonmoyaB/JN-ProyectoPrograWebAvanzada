-- Script de prueba para diagnosticar problemas de login
-- Ejecuta este script en tu base de datos DBInventario

USE DBInventario;
GO

PRINT '========================================';
PRINT 'PRUEBA DE LOGIN - DIAGNÓSTICO';
PRINT '========================================';
GO

-- Verificar usuarios existentes
PRINT '';
PRINT '1. Usuarios en la base de datos:';
SELECT 
    UsuarioID,
    Nombre,
    CorreoElectronico,
    LEN(ContrasenaHash) AS LongitudHash,
    LEFT(ContrasenaHash, 20) + '...' AS HashPreview,
    CASE 
        WHEN ContrasenaHash LIKE '%[^0-9a-fA-F]%' THEN 'Puede ser Base64 (nuevo)'
        WHEN LEN(ContrasenaHash) BETWEEN 32 AND 128 THEN 'Hexadecimal (legacy)'
        ELSE 'Formato desconocido'
    END AS TipoHash,
    RolID,
    Activo,
    BodegaID
FROM inv.Usuarios
ORDER BY UsuarioID;
GO

-- Verificar un usuario específico
DECLARE @EmailTest NVARCHAR(100) = 'joss9226@gmail.com'; -- CAMBIAR POR TU EMAIL
DECLARE @UsuarioID INT;

SELECT @UsuarioID = UsuarioID 
FROM inv.Usuarios 
WHERE CorreoElectronico = @EmailTest;

IF @UsuarioID IS NOT NULL
BEGIN
    PRINT '';
    PRINT '2. Información del usuario con email: ' + @EmailTest;
    SELECT 
        u.UsuarioID,
        u.Nombre,
        u.CorreoElectronico,
        u.ContrasenaHash,
        LEN(u.ContrasenaHash) AS LongitudHash,
        u.RolID,
        r.NombreRol,
        u.Activo,
        u.BodegaID,
        u.FechaRegistro
    FROM inv.Usuarios u
    LEFT JOIN inv.Roles r ON u.RolID = r.RolID
    WHERE u.UsuarioID = @UsuarioID;
    
    PRINT '';
    PRINT '3. Verificación de formato de hash:';
    DECLARE @Hash NVARCHAR(MAX);
    SELECT @Hash = ContrasenaHash FROM inv.Usuarios WHERE UsuarioID = @UsuarioID;
    
    IF @Hash IS NULL
    BEGIN
        PRINT '   ✗ ERROR: El hash de contraseña está NULL';
    END
    ELSE IF LEN(@Hash) = 0
    BEGIN
        PRINT '   ✗ ERROR: El hash de contraseña está vacío';
    END
    ELSE IF @Hash LIKE '%[^0-9a-fA-F]%'
    BEGIN
        PRINT '   ✓ Hash parece ser Base64 (formato nuevo con salt)';
        PRINT '   Longitud: ' + CAST(LEN(@Hash) AS NVARCHAR(10));
    END
    ELSE
    BEGIN
        PRINT '   ✓ Hash es hexadecimal (formato legacy SHA256 sin salt)';
        PRINT '   Longitud: ' + CAST(LEN(@Hash) AS NVARCHAR(10));
        IF LEN(@Hash) < 64
        BEGIN
            PRINT '   ⚠ ADVERTENCIA: El hash parece estar truncado (SHA256 debería tener 64 caracteres)';
        END
    END
END
ELSE
BEGIN
    PRINT '';
    PRINT '✗ ERROR: No se encontró usuario con email: ' + @EmailTest;
    PRINT '   Verifica que el email sea correcto y que esté en minúsculas';
END
GO

-- Verificar todos los correos electrónicos
PRINT '';
PRINT '4. Todos los correos electrónicos registrados:';
SELECT 
    UsuarioID,
    Nombre,
    CorreoElectronico,
    CASE 
        WHEN CorreoElectronico = LOWER(CorreoElectronico) THEN 'Minúsculas ✓'
        ELSE 'Mezcla de mayúsculas/minúsculas'
    END AS FormatoEmail
FROM inv.Usuarios
ORDER BY CorreoElectronico;
GO

-- Verificar roles
PRINT '';
PRINT '5. Roles disponibles:';
SELECT RolID, NombreRol FROM inv.Roles ORDER BY RolID;
GO

PRINT '';
PRINT '========================================';
PRINT 'DIAGNÓSTICO COMPLETADO';
PRINT '========================================';
PRINT '';
PRINT 'NOTA: Si los hashes están truncados, necesitarás regenerarlos.';
PRINT 'NOTA: Asegúrate de que los emails estén en minúsculas.';
PRINT 'NOTA: El formato de hash legacy (hexadecimal) es compatible.';
GO

