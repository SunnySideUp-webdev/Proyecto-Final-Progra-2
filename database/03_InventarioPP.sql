-- AutoManager DMS
-- Modulo 3: Inventario de repuestos
-- Integrante: Pavel Alexander Palma Carrillo (carne 0900-24-4296)
-- Parte de Pavel: la tabla Repuesto, sus columnas y la regla de stock no negativo
-- (su estructura original de database/Inventario.sql).
-- Ajustes de integracion (Sharon Consuegra): sin CREATE DATABASE, ejecucion repetible,
-- restricciones con nombre, textos NVARCHAR (tildes y enes), reglas de textos y del anio,
-- y la actualizacion de una tabla creada con el script original.
-- Crea la tabla Repuesto o actualiza la que ya exista sin perder sus datos.
-- Requiere la base AutoManagerDMS (scripts/init.sql en Docker o 01_ClientesVehiculosSC.sql).

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

USE AutoManagerDMS;
GO

IF OBJECT_ID(N'dbo.Repuesto', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Repuesto (
        idrepuesto INT IDENTITY(1,1) NOT NULL,
        codigo VARCHAR(15) NOT NULL,
        nombre NVARCHAR(50) NOT NULL,
        descripcion NVARCHAR(100) NOT NULL,
        stock INT NOT NULL
            CONSTRAINT DF_Repuesto_Stock DEFAULT 0,
        -- Vehiculo con el que el repuesto es compatible.
        marcacarro NVARCHAR(50) NOT NULL,
        modelocarro NVARCHAR(50) NOT NULL,
        aniocarro INT NOT NULL,

        CONSTRAINT PK_Repuesto
            PRIMARY KEY (idrepuesto),

        CONSTRAINT UQ_Repuesto_Codigo
            UNIQUE (codigo),

        -- El stock nunca queda en negativo (regla original de Pavel: chk_stock_negativo).
        CONSTRAINT CK_Repuesto_Stock
            CHECK (stock >= 0),

        CONSTRAINT CK_Repuesto_Textos
            CHECK (LEN(codigo) > 0 AND LEN(nombre) > 0 AND LEN(descripcion) > 0
                   AND LEN(marcacarro) > 0 AND LEN(modelocarro) > 0),

        -- Mismo anio minimo que dbo.Vehiculos (CK_Vehiculos_Anio).
        CONSTRAINT CK_Repuesto_Anio
            CHECK (aniocarro >= 1981)
    );
END
GO

-- =====================================================================
-- Actualizacion de una tabla creada con el script original (database/Inventario.sql):
-- textos VARCHAR y restricciones sin nombre. Si encuentra datos que no cumplen una
-- regla nueva, se detiene con un mensaje y no cambia nada; corregidos los datos, se
-- vuelve a ejecutar.
-- =====================================================================

-- Textos en NVARCHAR, como el resto de la base (admite cualquier letra con tilde).
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'dbo.Repuesto')
             AND name IN (N'nombre', N'descripcion', N'marcacarro', N'modelocarro')
             AND TYPE_NAME(system_type_id) = N'varchar')
BEGIN
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;
    ALTER TABLE dbo.Repuesto ALTER COLUMN nombre NVARCHAR(50) NOT NULL;
    ALTER TABLE dbo.Repuesto ALTER COLUMN descripcion NVARCHAR(100) NOT NULL;
    ALTER TABLE dbo.Repuesto ALTER COLUMN marcacarro NVARCHAR(50) NOT NULL;
    ALTER TABLE dbo.Repuesto ALTER COLUMN modelocarro NVARCHAR(50) NOT NULL;
    COMMIT TRANSACTION;
    PRINT N'Columnas de texto de Repuesto pasadas a NVARCHAR.';
END
GO

-- Nombres de restricciones segun CONVENCIONES_BD.md (solo cambia el nombre).
DECLARE @Nombre SYSNAME;

SELECT @Nombre = name FROM sys.key_constraints
WHERE parent_object_id = OBJECT_ID(N'dbo.Repuesto') AND type = 'PK';
IF @Nombre IS NOT NULL AND @Nombre <> N'PK_Repuesto'
BEGIN
    SET @Nombre = N'dbo.' + QUOTENAME(@Nombre);
    EXEC sys.sp_rename @Nombre, N'PK_Repuesto', N'OBJECT';
END

SET @Nombre = NULL;
SELECT @Nombre = kc.name
FROM sys.key_constraints AS kc
INNER JOIN sys.index_columns AS ic ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
INNER JOIN sys.columns AS c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE kc.parent_object_id = OBJECT_ID(N'dbo.Repuesto') AND kc.type = 'UQ' AND c.name = N'codigo';
IF @Nombre IS NOT NULL AND @Nombre <> N'UQ_Repuesto_Codigo'
BEGIN
    SET @Nombre = N'dbo.' + QUOTENAME(@Nombre);
    EXEC sys.sp_rename @Nombre, N'UQ_Repuesto_Codigo', N'OBJECT';
END

SET @Nombre = NULL;
SELECT @Nombre = dc.name
FROM sys.default_constraints AS dc
INNER JOIN sys.columns AS c ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
WHERE dc.parent_object_id = OBJECT_ID(N'dbo.Repuesto') AND c.name = N'stock';
IF @Nombre IS NULL
    ALTER TABLE dbo.Repuesto ADD CONSTRAINT DF_Repuesto_Stock DEFAULT 0 FOR stock;
ELSE IF @Nombre <> N'DF_Repuesto_Stock'
BEGIN
    SET @Nombre = N'dbo.' + QUOTENAME(@Nombre);
    EXEC sys.sp_rename @Nombre, N'DF_Repuesto_Stock', N'OBJECT';
END

IF OBJECT_ID(N'dbo.chk_stock_negativo', N'C') IS NOT NULL
   AND OBJECT_ID(N'dbo.CK_Repuesto_Stock', N'C') IS NULL
    EXEC sys.sp_rename N'dbo.chk_stock_negativo', N'CK_Repuesto_Stock', N'OBJECT';
GO

-- Reglas nuevas. Antes de agregar cada una se revisa que los datos actuales la cumplan.
IF OBJECT_ID(N'dbo.CK_Repuesto_Stock', N'C') IS NULL
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Repuesto WHERE stock < 0)
        THROW 50030, N'Hay repuestos con stock negativo. Corrigelos y vuelve a ejecutar el script.', 1;
    ALTER TABLE dbo.Repuesto ADD CONSTRAINT CK_Repuesto_Stock CHECK (stock >= 0);
END
GO

IF OBJECT_ID(N'dbo.CK_Repuesto_Textos', N'C') IS NULL
BEGIN
    IF EXISTS (SELECT 1 FROM dbo.Repuesto
               WHERE LEN(codigo) = 0 OR LEN(nombre) = 0 OR LEN(descripcion) = 0
                  OR LEN(marcacarro) = 0 OR LEN(modelocarro) = 0)
        THROW 50031, N'Hay repuestos con codigo, nombre, descripcion, marca o modelo vacio. Corrigelos y vuelve a ejecutar el script.', 1;
    ALTER TABLE dbo.Repuesto ADD CONSTRAINT CK_Repuesto_Textos
        CHECK (LEN(codigo) > 0 AND LEN(nombre) > 0 AND LEN(descripcion) > 0
               AND LEN(marcacarro) > 0 AND LEN(modelocarro) > 0);
END
GO

IF OBJECT_ID(N'dbo.CK_Repuesto_Anio', N'C') IS NULL
BEGIN
    DECLARE @Mensaje NVARCHAR(400);
    DECLARE @AnioInvalido INT = (SELECT COUNT(*) FROM dbo.Repuesto WHERE aniocarro < 1981);
    IF @AnioInvalido > 0
    BEGIN
        SET @Mensaje = CONCAT(N'Hay ', @AnioInvalido, N' repuestos con un anio de vehiculo anterior a 1981. ',
                              N'Corrigelos y vuelve a ejecutar el script.');
        THROW 50032, @Mensaje, 1;
    END
    ALTER TABLE dbo.Repuesto ADD CONSTRAINT CK_Repuesto_Anio CHECK (aniocarro >= 1981);
END
GO

IF OBJECT_ID(N'dbo.UQ_Repuesto_Codigo', N'UQ') IS NOT NULL
   AND OBJECT_ID(N'dbo.CK_Repuesto_Stock', N'C') IS NOT NULL
   AND OBJECT_ID(N'dbo.CK_Repuesto_Textos', N'C') IS NOT NULL
   AND OBJECT_ID(N'dbo.CK_Repuesto_Anio', N'C') IS NOT NULL
    PRINT N'Modulo 3: tabla Repuesto lista.';
ELSE
    PRINT N'Modulo 3: la tabla Repuesto NO quedo completa. Revisa los mensajes anteriores.';
GO
