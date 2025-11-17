-- Script para actualizar la tabla inv.Usuarios con las columnas faltantes
-- Ejecuta este script en tu base de datos DBInventario

USE DBInventario;
GO

-- Verificar y agregar columna CorreoElectronico
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') AND name = 'CorreoElectronico')
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ADD [CorreoElectronico] NVARCHAR(100) NOT NULL DEFAULT('');
    PRINT 'Columna CorreoElectronico agregada';
END
ELSE
BEGIN
    PRINT 'La columna CorreoElectronico ya existe';
END
GO

-- Verificar y agregar columna ContrasenaHash
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') AND name = 'ContrasenaHash')
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ADD [ContrasenaHash] NVARCHAR(255) NOT NULL DEFAULT('');
    PRINT 'Columna ContrasenaHash agregada';
END
ELSE
BEGIN
    PRINT 'La columna ContrasenaHash ya existe';
END
GO

-- Verificar y agregar columna FechaRegistro
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[inv].[Usuarios]') AND name = 'FechaRegistro')
BEGIN
    ALTER TABLE [inv].[Usuarios]
    ADD [FechaRegistro] DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME());
    PRINT 'Columna FechaRegistro agregada';
END
ELSE
BEGIN
    PRINT 'La columna FechaRegistro ya existe';
END
GO

-- Crear índice único para CorreoElectronico si no existe
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_CorreoElectronico' AND object_id = OBJECT_ID(N'[inv].[Usuarios]'))
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

-- Actualizar registros existentes si tienen valores por defecto vacíos
-- (Solo si es necesario, ajusta según tus datos)
UPDATE [inv].[Usuarios]
SET [CorreoElectronico] = LOWER(REPLACE([Nombre], ' ', '.')) + '@ejemplo.com'
WHERE [CorreoElectronico] = '' OR [CorreoElectronico] IS NULL;
GO

PRINT 'Script de actualización completado exitosamente';

