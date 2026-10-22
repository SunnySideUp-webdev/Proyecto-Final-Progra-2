
-- AutoManager DMS
-- Datos de ejemplo del Modulo 1 (Clientes y vehiculos)
-- Integrante: Sharon Consuegra
-- Clientes y vehiculos FICTICIOS para demostraciones y capturas de pantalla.
-- No usa identificadores fijos: SQL Server asigna los suyos, asi que se puede ejecutar
-- en una base que ya tenga otros datos (por ejemplo, la base del equipo).
-- Se puede ejecutar varias veces: solo inserta lo que todavia no existe
-- (clientes por DPI; vehiculos por VIN y placa).
-- Requiere 01_ClientesVehiculosSC.sql. Las fechas de registro estan en UTC.

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
GO

USE AutoManagerDMS;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Clientes TABLE (
    Orden INT NOT NULL, DPI CHAR(13) NOT NULL, Nombres NVARCHAR(80) NOT NULL, Apellidos NVARCHAR(80) NOT NULL,
    Telefono VARCHAR(15) NOT NULL, Correo NVARCHAR(254) NULL, Direccion NVARCHAR(200) NULL, FechaRegistro DATETIME2 NOT NULL);

INSERT @Clientes VALUES
    (1, '1234567890101', N'Ana María', N'Pérez López', '55551234', N'ana.perez@correo.com', N'Zona 10, Ciudad de Guatemala', '2026-10-20T02:53:15.1877457'),
    (2, '2987654320109', N'José Luis', N'García Ramírez', '50241234567', NULL, NULL, '2026-10-20T02:53:15.2168057'),
    (3, '3141592650101', N'María José', N'D''Angelo Castillo', '30123456', NULL, N'Calle 5 & Av. Reforma, Zona 9', '2026-10-20T02:53:15.2336868'),
    (4, '4567890120101', N'Carlos Eduardo', N'Méndez Ruiz', '55123456', N'carlos.mendez@correo.com', N'Calzada Roosevelt 12-34, Zona 11', '2026-10-20T03:29:42.4321581');

DECLARE @Vehiculos TABLE (
    Orden INT NOT NULL, DPIPropietario CHAR(13) NOT NULL, Placa VARCHAR(10) NOT NULL, VIN VARCHAR(17) NOT NULL,
    Marca NVARCHAR(50) NOT NULL, Modelo NVARCHAR(50) NOT NULL, Anio SMALLINT NOT NULL, FechaRegistro DATETIME2 NOT NULL);

INSERT @Vehiculos VALUES
    (1, '1234567890101', 'P246BCD', '2T1BURHE4FC000115', N'Toyota', N'Corolla', 2015, '2026-10-20T03:15:14.8049135'),
    (2, '1234567890101', 'M456DEF', 'LALPCJ0F6M1000121', N'Honda', N'CB190', 2021, '2026-10-20T03:15:14.8309550'),
    (3, '2987654320109', 'P789GHI', '1HGCM82633A004352', N'Honda', N'Accord', 2003, '2026-10-20T03:15:14.8487469'),
    (4, '3141592650101', 'C321JKL', 'JTEBU5JR8K5612345', N'Toyota', N'Hilux', 2020, '2026-10-20T03:15:14.8927287'),
    (5, '4567890120101', 'P654MNO', 'JM1BPACL0K1234567', N'Mazda', N'3', 2019, '2026-10-20T03:30:27.5742725');

-- Clientes que todavia no existen (por DPI).
INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono, Correo, Direccion, FechaRegistro)
SELECT d.DPI, d.Nombres, d.Apellidos, d.Telefono, d.Correo, d.Direccion, d.FechaRegistro
FROM @Clientes AS d
WHERE NOT EXISTS (SELECT 1 FROM dbo.Clientes AS c WHERE c.DPI = d.DPI)
ORDER BY d.Orden;
PRINT CONCAT(N'Clientes de ejemplo agregados: ', @@ROWCOUNT);

-- Avisos: placa o VIN que ya pertenecen a OTRO vehiculo (no se insertan).
DECLARE @Aviso NVARCHAR(400);
SELECT @Aviso = STRING_AGG(CONCAT(d.Placa, N' / ', d.VIN), N', ')
FROM @Vehiculos AS d
WHERE EXISTS (SELECT 1 FROM dbo.Vehiculos AS v
              WHERE (v.Placa = d.Placa AND v.VIN <> d.VIN) OR (v.VIN = d.VIN AND v.Placa <> d.Placa));
IF @Aviso IS NOT NULL
    PRINT N'AVISO: no se agregaron estos vehiculos de ejemplo porque su placa o VIN ya es de otro vehiculo: ' + @Aviso;

-- Vehiculos que todavia no existen (por VIN y por placa), enlazados a su cliente por DPI.
INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio, FechaRegistro)
SELECT c.IdCliente, d.Placa, d.VIN, d.Marca, d.Modelo, d.Anio, d.FechaRegistro
FROM @Vehiculos AS d
INNER JOIN dbo.Clientes AS c ON c.DPI = d.DPIPropietario
WHERE NOT EXISTS (SELECT 1 FROM dbo.Vehiculos AS v WHERE v.VIN = d.VIN OR v.Placa = d.Placa)
ORDER BY d.Orden;
PRINT CONCAT(N'Vehiculos de ejemplo agregados: ', @@ROWCOUNT);

COMMIT TRANSACTION;
GO
