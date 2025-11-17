-- Script para agregar las columnas faltantes a la tabla inv.Usuarios
-- Ejecuta este script en tu base de datos DBInventario

USE DBInventario;
GO

-- Agregar columna CorreoElectronico
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') 
               AND name = 'CorreoElectronico')
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ADD [CorreoElectronico] NVARCHAR(100) NULL;
    
    PRINT 'Columna CorreoElectronico agregada';
END
ELSE
BEGIN
    PRINT 'La columna CorreoElectronico ya existe';
END
GO

-- Agregar columna ContrasenaHash
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') 
               AND name = 'ContrasenaHash')
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ADD [ContrasenaHash] NVARCHAR(255) NULL;
    
    PRINT 'Columna ContrasenaHash agregada';
END
ELSE
BEGIN
    PRINT 'La columna ContrasenaHash ya existe';
END
GO

-- Agregar columna FechaRegistro
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') 
               AND name = 'FechaRegistro')
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ADD [FechaRegistro] DATETIME2 NULL DEFAULT (SYSUTCDATETIME());
    
    PRINT 'Columna FechaRegistro agregada';
END
ELSE
BEGIN
    PRINT 'La columna FechaRegistro ya existe';
END
GO

-- Actualizar registros existentes con valores por defecto si están NULL
UPDATE [inv].[Usuarios]
SET [CorreoElectronico] = LOWER(REPLACE(REPLACE([Nombre], ' ', '.'), '''', '')) + '@ejemplo.com'
WHERE [CorreoElectronico] IS NULL;

UPDATE [inv].[Usuarios]
SET [ContrasenaHash] = ''
WHERE [ContrasenaHash] IS NULL;

UPDATE [inv].[Usuarios]
SET [FechaRegistro] = SYSUTCDATETIME()
WHERE [FechaRegistro] IS NULL;
GO

-- Ahora hacer las columnas NOT NULL (después de actualizar los valores)
IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') 
           AND name = 'CorreoElectronico' 
           AND is_nullable = 1)
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ALTER COLUMN [CorreoElectronico] NVARCHAR(100) NOT NULL;
    
    PRINT 'Columna CorreoElectronico configurada como NOT NULL';
END
GO

IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') 
           AND name = 'ContrasenaHash' 
           AND is_nullable = 1)
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ALTER COLUMN [ContrasenaHash] NVARCHAR(255) NOT NULL;
    
    PRINT 'Columna ContrasenaHash configurada como NOT NULL';
END
GO

IF EXISTS (SELECT * FROM sys.columns 
           WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') 
           AND name = 'FechaRegistro' 
           AND is_nullable = 1)
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ALTER COLUMN [FechaRegistro] DATETIME2 NOT NULL;
    
    PRINT 'Columna FechaRegistro configurada como NOT NULL';
END
GO

-- Crear índice único para CorreoElectronico si no existe
IF NOT EXISTS (SELECT * FROM sys.indexes 
               WHERE name = 'IX_Usuarios_CorreoElectronico' 
               AND object_id = OBJECT_ID(N'[inv].[Usuarios]'))
BEGIN
    CREATE UNIQUE INDEX [IX_Usuarios_CorreoElectronico] 
    ON [inv].[Usuarios] ([CorreoElectronico]);
    
    PRINT 'Índice único para CorreoElectronico creado';
END
ELSE
BEGIN
    PRINT 'El índice para CorreoElectronico ya existe';
END
GO

PRINT '========================================';
PRINT 'Script completado exitosamente';
PRINT 'Las columnas han sido agregadas a inv.Usuarios';
PRINT '========================================';

