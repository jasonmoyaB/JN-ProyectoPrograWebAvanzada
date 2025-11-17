-- Script para agregar el rol "Técnico" a la tabla inv.Roles
-- Ejecuta este script en tu base de datos DBInventario

USE DBInventario;
GO

-- Verificar si el rol "Técnico" existe (con tilde)
IF NOT EXISTS (SELECT 1 FROM [inv].[Roles] WHERE [NombreRol] = 'Técnico')
BEGIN
    -- Verificar si existe "Tecnico" sin tilde y actualizarlo
    IF EXISTS (SELECT 1 FROM [inv].[Roles] WHERE [NombreRol] = 'Tecnico')
    BEGIN
        UPDATE [inv].[Roles]
        SET [NombreRol] = 'Técnico'
        WHERE [NombreRol] = 'Tecnico';
        
        PRINT 'Rol "Tecnico" actualizado a "Técnico"';
    END
    ELSE
    BEGIN
        -- Insertar el rol "Técnico" si no existe
        -- Primero verificamos cuál es el siguiente ID disponible
        DECLARE @NuevoRolID INT;
        SELECT @NuevoRolID = ISNULL(MAX([RolID]), 0) + 1 FROM [inv].[Roles];
        
        -- Si el ID 2 está disponible, lo usamos, sino usamos el siguiente
        IF NOT EXISTS (SELECT 1 FROM [inv].[Roles] WHERE [RolID] = 2)
        BEGIN
            SET IDENTITY_INSERT [inv].[Roles] ON;
            INSERT INTO [inv].[Roles] ([RolID], [NombreRol]) VALUES (2, 'Técnico');
            SET IDENTITY_INSERT [inv].[Roles] OFF;
        END
        ELSE
        BEGIN
            INSERT INTO [inv].[Roles] ([NombreRol]) VALUES ('Técnico');
        END
        
        PRINT 'Rol "Técnico" agregado exitosamente';
    END
END
ELSE
BEGIN
    PRINT 'El rol "Técnico" ya existe';
END
GO

-- Verificar que los roles principales existan
IF NOT EXISTS (SELECT 1 FROM [inv].[Roles] WHERE [NombreRol] = 'Administrador')
BEGIN
    DECLARE @AdminRolID INT;
    SELECT @AdminRolID = ISNULL(MAX([RolID]), 0) + 1 FROM [inv].[Roles];
    
    IF NOT EXISTS (SELECT 1 FROM [inv].[Roles] WHERE [RolID] = 1)
    BEGIN
        SET IDENTITY_INSERT [inv].[Roles] ON;
        INSERT INTO [inv].[Roles] ([RolID], [NombreRol]) VALUES (1, 'Administrador');
        SET IDENTITY_INSERT [inv].[Roles] OFF;
    END
    ELSE
    BEGIN
        INSERT INTO [inv].[Roles] ([NombreRol]) VALUES ('Administrador');
    END
    
    PRINT 'Rol "Administrador" agregado';
END
GO

-- Mostrar los roles existentes
SELECT [RolID], [NombreRol] FROM [inv].[Roles] ORDER BY [RolID];
GO

PRINT '========================================';
PRINT 'Script completado exitosamente';
PRINT 'El rol "Técnico" está disponible para nuevos registros';
PRINT '========================================';

