-- Script para crear las tablas de Usuarios y Roles en DBInventario
-- Ejecuta este script en SQL Server Management Studio o Azure Data Studio
-- Conectado a la base de datos: DBInventario

USE DBInventario;
GO

-- Crear tabla Roles si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Roles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [RolID] INT IDENTITY(1,1) NOT NULL,
        [NombreRol] NVARCHAR(MAX) NOT NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([RolID])
    );
    
    -- Insertar roles iniciales
    INSERT INTO [dbo].[Roles] ([NombreRol]) VALUES ('Administrador');
    INSERT INTO [dbo].[Roles] ([NombreRol]) VALUES ('Vendedor');
    
    PRINT 'Tabla Roles creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla Roles ya existe';
    
    -- Verificar que existan los roles
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [NombreRol] = 'Administrador')
        INSERT INTO [dbo].[Roles] ([NombreRol]) VALUES ('Administrador');
    
    IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [NombreRol] = 'Vendedor')
        INSERT INTO [dbo].[Roles] ([NombreRol]) VALUES ('Vendedor');
END
GO

-- Crear tabla Usuarios si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Usuarios]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Usuarios] (
        [UsuarioID] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(100) NOT NULL,
        [CorreoElectronico] NVARCHAR(100) NOT NULL,
        [ContrasenaHash] NVARCHAR(255) NOT NULL,
        [FechaRegistro] DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME()),
        [Activo] BIT NOT NULL DEFAULT 1,
        [RolID] INT NOT NULL,
        CONSTRAINT [PK_Usuarios] PRIMARY KEY ([UsuarioID]),
        CONSTRAINT [FK_Usuarios_Roles_RolID] FOREIGN KEY ([RolID]) REFERENCES [dbo].[Roles] ([RolID]) ON DELETE CASCADE
    );
    
    -- Crear índice
    CREATE INDEX [IX_Usuarios_RolID] ON [dbo].[Usuarios] ([RolID]);
    
    -- Crear índice único para correo electrónico
    CREATE UNIQUE INDEX [IX_Usuarios_CorreoElectronico] ON [dbo].[Usuarios] ([CorreoElectronico]);
    
    PRINT 'Tabla Usuarios creada exitosamente';
END
ELSE
BEGIN
    PRINT 'La tabla Usuarios ya existe';
END
GO

PRINT 'Script ejecutado correctamente. Las tablas están listas para usar.';
