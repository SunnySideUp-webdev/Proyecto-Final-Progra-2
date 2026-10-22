
-- AutoManager DMS
-- Modulo 2: Ordenes de reparacion
-- Integrante: Neider Ernesto Tomas Raxcaco (carne 0900-16-12945)
-- Parte de Neider: la tabla OrdenesReparacion y sus columnas (su estructura original).
-- Ajustes de integracion (Sharon Consuegra): llave foranea (IdVehiculo, VIN), reglas de
-- cierre y motivo, fechas en UTC, indice por VIN y la actualizacion de tablas existentes.
-- Crea la tabla OrdenesReparacion (misma estructura que usa el codigo del modulo) o
-- actualiza la que ya exista sin perder sus datos. Se puede ejecutar varias veces.
-- Requiere 01_ClientesVehiculosSC.sql (tabla Vehiculos).
-- Fechas: se guardan en UTC (SYSUTCDATETIME); la aplicacion las convierte a hora de
-- Guatemala solo al mostrarlas.

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

USE AutoManagerDMS;
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints
               WHERE name = N'UQ_Vehiculos_IdVehiculo_VIN' AND parent_object_id = OBJECT_ID(N'dbo.Vehiculos'))
    THROW 50010, N'Ejecuta primero 01_ClientesVehiculosSC.sql: falta la tabla Vehiculos actualizada.', 1;
GO

IF OBJECT_ID(N'dbo.OrdenesReparacion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrdenesReparacion (
        IdOrden INT IDENTITY(1,1) NOT NULL,
        IdVehiculo INT NOT NULL,
        VIN VARCHAR(17) NOT NULL,
        -- Placa que tenia el vehiculo cuando se abrio la orden (dato historico).
        Placa VARCHAR(20) NULL,
        FechaApertura DATETIME2 NOT NULL
            CONSTRAINT DF_OrdenesReparacion_FechaApertura DEFAULT SYSUTCDATETIME(),
        MotivoIngreso NVARCHAR(500) NOT NULL,
        Diagnostico NVARCHAR(1000) NULL,
        Estado NVARCHAR(20) NOT NULL
            CONSTRAINT DF_OrdenesReparacion_Estado DEFAULT N'Abierta',
        FechaCierre DATETIME2 NULL,
        ObservacionesCierre NVARCHAR(1000) NULL,

        CONSTRAINT PK_OrdenesReparacion
            PRIMARY KEY (IdOrden),

        -- IdVehiculo y VIN deben ser del mismo vehiculo. Si se corrige el VIN
        -- en Vehiculos, las ordenes se actualizan solas.
        CONSTRAINT FK_OrdenesReparacion_Vehiculos
            FOREIGN KEY (IdVehiculo, VIN) REFERENCES dbo.Vehiculos (IdVehiculo, VIN)
            ON UPDATE CASCADE,

        CONSTRAINT CK_OrdenesReparacion_Estado
            CHECK (Estado IN (N'Abierta', N'Cerrada')),

        -- Una orden abierta no tiene fecha de cierre y una cerrada si.
        CONSTRAINT CK_OrdenesReparacion_Cierre
            CHECK ((Estado = N'Abierta' AND FechaCierre IS NULL)
                OR (Estado = N'Cerrada' AND FechaCierre IS NOT NULL AND FechaCierre >= FechaApertura)),

        CONSTRAINT CK_OrdenesReparacion_Motivo
            CHECK (LEN(MotivoIngreso) > 0)
    );
END
GO

-- =====================================================================
-- Actualizacion de una tabla creada con la version anterior del script
-- (VIN opcional, fecha con hora local y sin llave foranea). Cada bloque se
-- revisa por si solo: si encuentra datos inconsistentes se detiene con un
-- mensaje y no cambia nada; corregidos los datos, se vuelve a ejecutar.
-- =====================================================================

IF COL_LENGTH(N'dbo.OrdenesReparacion', N'IdVehiculo') IS NULL
    THROW 50011, N'OrdenesReparacion no tiene la columna IdVehiculo: es una estructura anterior que este script no actualiza.', 1;
GO

-- El VIN pasa a ser obligatorio: las ordenes sin VIN toman el de su vehiculo.
IF COLUMNPROPERTY(OBJECT_ID(N'dbo.OrdenesReparacion'), N'VIN', N'AllowsNull') = 1
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    UPDATE o SET o.VIN = v.VIN
    FROM dbo.OrdenesReparacion AS o
    INNER JOIN dbo.Vehiculos AS v ON v.IdVehiculo = o.IdVehiculo
    WHERE o.VIN IS NULL;

    IF EXISTS (SELECT 1 FROM dbo.OrdenesReparacion WHERE VIN IS NULL)
        THROW 50012, N'Hay ordenes sin VIN cuyo vehiculo no existe. Corrigelas y vuelve a ejecutar el script.', 1;

    ALTER TABLE dbo.OrdenesReparacion ALTER COLUMN VIN VARCHAR(17) NOT NULL;
    COMMIT TRANSACTION;
END
GO

-- Fechas en UTC. Las fechas ya guardadas con la hora local del servidor se pasan a UTC
-- con la diferencia horaria actual del servidor (Guatemala no aplica horario de verano;
-- en un servidor que ya esta en UTC no cambian).
DECLARE @NombreDefault SYSNAME, @Definicion NVARCHAR(4000), @Sql NVARCHAR(400);
SELECT @NombreDefault = dc.name, @Definicion = dc.definition
FROM sys.default_constraints AS dc
INNER JOIN sys.columns AS c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.OrdenesReparacion') AND c.name = N'FechaApertura';

IF @Definicion IS NULL OR @Definicion <> N'(sysutcdatetime())'
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    DECLARE @MinutosServidor INT = DATEPART(TZOFFSET, SYSDATETIMEOFFSET());
    IF (@Definicion LIKE N'%sysdatetime()%' OR @Definicion LIKE N'%getdate()%') AND @MinutosServidor <> 0
    BEGIN
        UPDATE dbo.OrdenesReparacion
        SET FechaApertura = DATEADD(MINUTE, -@MinutosServidor, FechaApertura),
            FechaCierre = DATEADD(MINUTE, -@MinutosServidor, FechaCierre);
        PRINT CONCAT(N'Fechas existentes pasadas a UTC (', @@ROWCOUNT, N' ordenes).');
    END

    IF @NombreDefault IS NOT NULL
    BEGIN
        SET @Sql = N'ALTER TABLE dbo.OrdenesReparacion DROP CONSTRAINT ' + QUOTENAME(@NombreDefault) + N';';
        EXEC sys.sp_executesql @Sql;
    END
    ALTER TABLE dbo.OrdenesReparacion ADD CONSTRAINT DF_OrdenesReparacion_FechaApertura
        DEFAULT SYSUTCDATETIME() FOR FechaApertura;
    COMMIT TRANSACTION;
END
GO

-- Nombres de restricciones segun CONVENCIONES_BD.md (solo cambia el nombre).
DECLARE @Nombre SYSNAME;

SELECT @Nombre = name FROM sys.key_constraints
WHERE parent_object_id = OBJECT_ID(N'dbo.OrdenesReparacion') AND type = 'PK';
IF @Nombre IS NOT NULL AND @Nombre <> N'PK_OrdenesReparacion'
BEGIN
    SET @Nombre = N'dbo.' + QUOTENAME(@Nombre);
    EXEC sys.sp_rename @Nombre, N'PK_OrdenesReparacion', N'OBJECT';
END

SET @Nombre = NULL;
SELECT @Nombre = dc.name
FROM sys.default_constraints AS dc
INNER JOIN sys.columns AS c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.OrdenesReparacion') AND c.name = N'Estado';
IF @Nombre IS NULL
    ALTER TABLE dbo.OrdenesReparacion ADD CONSTRAINT DF_OrdenesReparacion_Estado DEFAULT N'Abierta' FOR Estado;
ELSE IF @Nombre <> N'DF_OrdenesReparacion_Estado'
BEGIN
    SET @Nombre = N'dbo.' + QUOTENAME(@Nombre);
    EXEC sys.sp_rename @Nombre, N'DF_OrdenesReparacion_Estado', N'OBJECT';
END

IF OBJECT_ID(N'dbo.CK_Ordenes_Estado', N'C') IS NOT NULL
   AND OBJECT_ID(N'dbo.CK_OrdenesReparacion_Estado', N'C') IS NULL
    EXEC sys.sp_rename N'dbo.CK_Ordenes_Estado', N'CK_OrdenesReparacion_Estado', N'OBJECT';
GO

-- Reglas nuevas. Antes de agregar cada una se revisa que los datos actuales la cumplan.
IF OBJECT_ID(N'dbo.CK_OrdenesReparacion_Estado', N'C') IS NULL
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.OrdenesReparacion WHERE Estado NOT IN (N'Abierta', N'Cerrada'))
        THROW 50013, N'Hay ordenes con un estado distinto de Abierta o Cerrada. Corrigelas y vuelve a ejecutar el script.', 1;
    ALTER TABLE dbo.OrdenesReparacion ADD CONSTRAINT CK_OrdenesReparacion_Estado
        CHECK (Estado IN (N'Abierta', N'Cerrada'));
END
GO

IF OBJECT_ID(N'dbo.CK_OrdenesReparacion_Cierre', N'C') IS NULL
BEGIN
    DECLARE @Mensaje NVARCHAR(400);
    DECLARE @Incoherentes INT = (SELECT COUNT(*) FROM dbo.OrdenesReparacion
        WHERE NOT ((Estado = N'Abierta' AND FechaCierre IS NULL)
                OR (Estado = N'Cerrada' AND FechaCierre IS NOT NULL AND FechaCierre >= FechaApertura)));
    IF @Incoherentes > 0
    BEGIN
        SET @Mensaje = CONCAT(N'Hay ', @Incoherentes, N' ordenes cuyo estado no coincide con la fecha de cierre. ',
                              N'Corrigelas y vuelve a ejecutar el script.');
        THROW 50014, @Mensaje, 1;
    END
    ALTER TABLE dbo.OrdenesReparacion ADD CONSTRAINT CK_OrdenesReparacion_Cierre
        CHECK ((Estado = N'Abierta' AND FechaCierre IS NULL)
            OR (Estado = N'Cerrada' AND FechaCierre IS NOT NULL AND FechaCierre >= FechaApertura));
END
GO

IF OBJECT_ID(N'dbo.CK_OrdenesReparacion_Motivo', N'C') IS NULL
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.OrdenesReparacion WHERE LEN(MotivoIngreso) = 0)
        THROW 50015, N'Hay ordenes con el motivo de ingreso vacio. Corrigelas y vuelve a ejecutar el script.', 1;
    ALTER TABLE dbo.OrdenesReparacion ADD CONSTRAINT CK_OrdenesReparacion_Motivo
        CHECK (LEN(MotivoIngreso) > 0);
END
GO

IF OBJECT_ID(N'dbo.FK_OrdenesReparacion_Vehiculos', N'F') IS NULL
BEGIN
    DECLARE @Mensaje NVARCHAR(400);
    DECLARE @SinVehiculo INT = (SELECT COUNT(*) FROM dbo.OrdenesReparacion AS o
        WHERE NOT EXISTS (SELECT 1 FROM dbo.Vehiculos AS v
                          WHERE v.IdVehiculo = o.IdVehiculo AND v.VIN = o.VIN));
    IF @SinVehiculo > 0
    BEGIN
        SET @Mensaje = CONCAT(N'Hay ', @SinVehiculo, N' ordenes cuyo IdVehiculo y VIN no corresponden a un mismo ',
                              N'vehiculo registrado. Corrigelas y vuelve a ejecutar el script.');
        THROW 50016, @Mensaje, 1;
    END
    ALTER TABLE dbo.OrdenesReparacion ADD CONSTRAINT FK_OrdenesReparacion_Vehiculos
        FOREIGN KEY (IdVehiculo, VIN) REFERENCES dbo.Vehiculos (IdVehiculo, VIN)
        ON UPDATE CASCADE;
END
GO

-- Acelera el historial de reparaciones por VIN.
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_OrdenesReparacion_VIN' AND object_id = OBJECT_ID(N'dbo.OrdenesReparacion'))
BEGIN
    CREATE INDEX IX_OrdenesReparacion_VIN ON dbo.OrdenesReparacion (VIN);
END
GO

IF OBJECT_ID(N'dbo.FK_OrdenesReparacion_Vehiculos', N'F') IS NOT NULL
   AND COLUMNPROPERTY(OBJECT_ID(N'dbo.OrdenesReparacion'), N'VIN', N'AllowsNull') = 0
   AND OBJECT_ID(N'dbo.CK_OrdenesReparacion_Cierre', N'C') IS NOT NULL
   AND OBJECT_ID(N'dbo.CK_OrdenesReparacion_Motivo', N'C') IS NOT NULL
    PRINT N'Modulo 2: tabla OrdenesReparacion lista.';
ELSE
    PRINT N'Modulo 2: la tabla OrdenesReparacion NO quedo completa. Revisa los mensajes anteriores.';
GO
