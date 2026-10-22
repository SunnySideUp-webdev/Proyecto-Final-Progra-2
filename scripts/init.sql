-- Prepara las bases de datos locales y el usuario de la aplicación.
--   TallerMecanicoDB: entorno de prueba de conexión (pruebas/PruebaConexionSQL).
--   AutoManagerDMS:   base compartida del equipo; sus tablas se crean con los
--                     scripts numerados de la carpeta database/.
-- La contraseña llega desde .env como variable de sqlcmd (TALLER_APP_PASSWORD).
-- El script se puede ejecutar varias veces sin duplicar nada.

IF DB_ID(N'TallerMecanicoDB') IS NULL
BEGIN
    CREATE DATABASE TallerMecanicoDB;
END
GO

IF DB_ID(N'AutoManagerDMS') IS NULL
BEGIN
    CREATE DATABASE AutoManagerDMS;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'taller_app')
BEGIN
    CREATE LOGIN taller_app
        WITH PASSWORD = N'$(TALLER_APP_PASSWORD)',
             DEFAULT_DATABASE = TallerMecanicoDB,
             CHECK_POLICY = ON;
END
ELSE
BEGIN
    ALTER LOGIN taller_app WITH PASSWORD = N'$(TALLER_APP_PASSWORD)';
END
GO

USE TallerMecanicoDB;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'taller_app')
BEGIN
    CREATE USER taller_app FOR LOGIN taller_app;
END
GO

-- Permisos limitados: leer y escribir datos, sin crear ni borrar tablas.
ALTER ROLE db_datareader ADD MEMBER taller_app;
ALTER ROLE db_datawriter ADD MEMBER taller_app;
IF IS_ROLEMEMBER(N'db_owner', N'taller_app') = 1
BEGIN
    ALTER ROLE db_owner DROP MEMBER taller_app;
END
GO

USE AutoManagerDMS;
GO

-- Evita saltos de 1000 en las identidades cuando el contenedor se detiene.
-- No garantiza números consecutivos: las transacciones deshechas también consumen valores.
ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF;
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'taller_app')
BEGIN
    CREATE USER taller_app FOR LOGIN taller_app;
END
GO

ALTER ROLE db_datareader ADD MEMBER taller_app;
ALTER ROLE db_datawriter ADD MEMBER taller_app;
IF IS_ROLEMEMBER(N'db_owner', N'taller_app') = 1
BEGIN
    ALTER ROLE db_owner DROP MEMBER taller_app;
END
GO

PRINT N'TallerMecanicoDB, AutoManagerDMS y el usuario taller_app están listos.';
GO
