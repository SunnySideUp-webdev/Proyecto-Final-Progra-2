
-- AutoManager DMS
-- Modulo 1: Clientes y vehiculos
-- Integrante: Sharon Consuegra
-- Crea las tablas Clientes y Vehiculos con sus restricciones e indices.
-- Se puede ejecutar varias veces: solo crea lo que todavia no existe.
-- Requiere que la base AutoManagerDMS ya exista.
-- Fechas: FechaRegistro se guarda en UTC (SYSUTCDATETIME) en cualquier servidor.
-- La aplicacion la convierte a la hora de Guatemala solo al mostrarla.

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON; -- Configuracion recomendada por SQL Server (igual que sqlcmd -I).
GO

USE AutoManagerDMS;
GO

IF OBJECT_ID(N'dbo.Clientes', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Clientes (
        IdCliente INT IDENTITY(1,1) NOT NULL,
        DPI CHAR(13) NOT NULL,
        Nombres NVARCHAR(80) NOT NULL,
        Apellidos NVARCHAR(80) NOT NULL,
        Telefono VARCHAR(15) NOT NULL,
        Correo NVARCHAR(254) NULL,
        Direccion NVARCHAR(200) NULL,
        FechaRegistro DATETIME2 NOT NULL
            CONSTRAINT DF_Clientes_FechaRegistro DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Clientes
            PRIMARY KEY (IdCliente),

        -- Criterio de duplicado: un DPI por cliente.
        CONSTRAINT UQ_Clientes_DPI
            UNIQUE (DPI),

        CONSTRAINT CK_Clientes_DPI
            CHECK (LEN(DPI) = 13 AND DPI NOT LIKE '%[^0-9]%'),

        CONSTRAINT CK_Clientes_Nombres
            CHECK (LEN(Nombres) > 0),

        CONSTRAINT CK_Clientes_Apellidos
            CHECK (LEN(Apellidos) > 0),

        CONSTRAINT CK_Clientes_Telefono
            CHECK (LEN(Telefono) BETWEEN 8 AND 15 AND Telefono NOT LIKE '%[^0-9]%'),

        CONSTRAINT CK_Clientes_Correo
            CHECK (Correo IS NULL OR Correo LIKE '_%@_%._%')
    );
END
GO

IF OBJECT_ID(N'dbo.Vehiculos', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Vehiculos (
        IdVehiculo INT IDENTITY(1,1) NOT NULL,
        IdCliente INT NOT NULL,
        Placa VARCHAR(10) NOT NULL,
        VIN VARCHAR(17) NOT NULL,
        Marca NVARCHAR(50) NOT NULL,
        Modelo NVARCHAR(50) NOT NULL,
        Anio SMALLINT NOT NULL,
        FechaRegistro DATETIME2 NOT NULL
            CONSTRAINT DF_Vehiculos_FechaRegistro DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Vehiculos
            PRIMARY KEY (IdVehiculo),

        CONSTRAINT FK_Vehiculos_Clientes
            FOREIGN KEY (IdCliente) REFERENCES dbo.Clientes (IdCliente),

        CONSTRAINT UQ_Vehiculos_Placa
            UNIQUE (Placa),

        -- El VIN es obligatorio y unico: identifica al vehiculo en las ordenes de
        -- reparacion (FK OrdenesReparacion.VIN -> Vehiculos.VIN).
        CONSTRAINT UQ_Vehiculos_VIN
            UNIQUE (VIN),

        -- Permite que OrdenesReparacion exija que IdVehiculo y VIN sean del mismo vehiculo.
        CONSTRAINT UQ_Vehiculos_IdVehiculo_VIN
            UNIQUE (IdVehiculo, VIN),

        -- La placa se guarda normalizada: mayusculas y numeros, sin espacios ni guiones.
        CONSTRAINT CK_Vehiculos_Placa
            CHECK (LEN(Placa) BETWEEN 5 AND 10
                AND Placa COLLATE Latin1_General_BIN NOT LIKE '%[^A-Z0-9]%'),

        -- VIN estandar: 17 caracteres sin las letras I, O ni Q.
        CONSTRAINT CK_Vehiculos_VIN
            CHECK (LEN(VIN) = 17
                AND VIN COLLATE Latin1_General_BIN NOT LIKE '%[^A-HJ-NPR-Z0-9]%'),

        CONSTRAINT CK_Vehiculos_Marca
            CHECK (LEN(Marca) > 0),

        CONSTRAINT CK_Vehiculos_Modelo
            CHECK (LEN(Modelo) > 0),

        -- Solo el limite fijo: desde 1981 los vehiculos tienen VIN de 17 caracteres.
        -- El maximo (anio actual + 1) se valida en la Capa Logica, porque depende de la fecha.
        CONSTRAINT CK_Vehiculos_Anio
            CHECK (Anio >= 1981)
    );
END
GO

-- Actualizacion para bases creadas cuando el VIN era opcional: pasa a ser obligatorio.
-- No inventa datos: si algun vehiculo no tiene VIN, el script se detiene para registrarlo antes.
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'dbo.Vehiculos') AND name = N'VIN' AND is_nullable = 1)
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Vehiculos WHERE VIN IS NULL)
        THROW 50001, N'Hay vehiculos sin VIN. Registra su VIN antes de ejecutar este script.', 1;

    -- El indice filtrado anterior no permite llaves foraneas; lo reemplaza UQ_Vehiculos_VIN.
    IF EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'UX_Vehiculos_VIN' AND object_id = OBJECT_ID(N'dbo.Vehiculos'))
        DROP INDEX UX_Vehiculos_VIN ON dbo.Vehiculos;

    ALTER TABLE dbo.Vehiculos DROP CONSTRAINT CK_Vehiculos_VIN;
    ALTER TABLE dbo.Vehiculos ALTER COLUMN VIN VARCHAR(17) NOT NULL;
    ALTER TABLE dbo.Vehiculos ADD CONSTRAINT CK_Vehiculos_VIN
        CHECK (LEN(VIN) = 17
            AND VIN COLLATE Latin1_General_BIN NOT LIKE '%[^A-HJ-NPR-Z0-9]%');
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints
               WHERE name = N'UQ_Vehiculos_VIN' AND parent_object_id = OBJECT_ID(N'dbo.Vehiculos'))
BEGIN
    ALTER TABLE dbo.Vehiculos ADD CONSTRAINT UQ_Vehiculos_VIN UNIQUE (VIN);
END
GO

-- Actualizacion: restriccion que usa la llave foranea de OrdenesReparacion (02_OrdenesReparacionNT.sql).
IF NOT EXISTS (SELECT 1 FROM sys.key_constraints
               WHERE name = N'UQ_Vehiculos_IdVehiculo_VIN' AND parent_object_id = OBJECT_ID(N'dbo.Vehiculos'))
BEGIN
    ALTER TABLE dbo.Vehiculos ADD CONSTRAINT UQ_Vehiculos_IdVehiculo_VIN UNIQUE (IdVehiculo, VIN);
END
GO

-- Actualizacion del anio minimo (antes 1900): ahora 1981, igual que la Capa Logica.
IF EXISTS (SELECT 1 FROM sys.check_constraints
           WHERE name = N'CK_Vehiculos_Anio' AND definition <> N'([Anio]>=(1981))')
BEGIN
    ALTER TABLE dbo.Vehiculos DROP CONSTRAINT CK_Vehiculos_Anio;
    ALTER TABLE dbo.Vehiculos ADD CONSTRAINT CK_Vehiculos_Anio CHECK (Anio >= 1981);
END
GO

-- Acelera la consulta de los vehiculos de un cliente.
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_Vehiculos_IdCliente' AND object_id = OBJECT_ID(N'dbo.Vehiculos'))
BEGIN
    CREATE INDEX IX_Vehiculos_IdCliente
        ON dbo.Vehiculos (IdCliente);
END
GO

-- Actualizacion para bases creadas antes de usar UTC: si el DEFAULT todavia es
-- SYSDATETIME() (hora local del servidor), se reemplaza por SYSUTCDATETIME().
-- No modifica los registros existentes; solo el valor de los nuevos.
IF EXISTS (SELECT 1 FROM sys.default_constraints
           WHERE name = N'DF_Clientes_FechaRegistro' AND definition <> N'(sysutcdatetime())')
BEGIN
    ALTER TABLE dbo.Clientes DROP CONSTRAINT DF_Clientes_FechaRegistro;
    ALTER TABLE dbo.Clientes ADD CONSTRAINT DF_Clientes_FechaRegistro
        DEFAULT SYSUTCDATETIME() FOR FechaRegistro;
END
GO

IF EXISTS (SELECT 1 FROM sys.default_constraints
           WHERE name = N'DF_Vehiculos_FechaRegistro' AND definition <> N'(sysutcdatetime())')
BEGIN
    ALTER TABLE dbo.Vehiculos DROP CONSTRAINT DF_Vehiculos_FechaRegistro;
    ALTER TABLE dbo.Vehiculos ADD CONSTRAINT DF_Vehiculos_FechaRegistro
        DEFAULT SYSUTCDATETIME() FOR FechaRegistro;
END
GO

PRINT N'Modulo 1: tablas Clientes y Vehiculos listas.';
GO
