
-- AutoManager DMS
-- Datos de ejemplo del Modulo 4 (Reportes)
-- Integrante: Fatima Palacios Ordonez (carne 0900-21-1810)
-- Ordenes de reparacion FICTICIAS sobre los vehiculos de ejemplo (90_DatosEjemploSC.sql),
-- para que los reportes muestren informacion en cualquier computadora.
-- No usa identificadores fijos: busca cada vehiculo por su VIN. Si el vehiculo no existe,
-- esa orden se omite. Se puede ejecutar varias veces sin duplicar nada.
-- Requiere 01_ClientesVehiculosSC.sql, 02_OrdenesReparacionNT.sql y 90_DatosEjemploSC.sql.
-- Las fechas estan en UTC (Guatemala = UTC - 6 horas).

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
GO

USE AutoManagerDMS;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Ordenes TABLE (
    Orden INT NOT NULL, VIN VARCHAR(17) NOT NULL, MotivoIngreso NVARCHAR(500) NOT NULL,
    Diagnostico NVARCHAR(1000) NULL, Estado NVARCHAR(20) NOT NULL, FechaApertura DATETIME2 NOT NULL,
    FechaCierre DATETIME2 NULL, ObservacionesCierre NVARCHAR(1000) NULL);

INSERT @Ordenes VALUES
    (1, '1HGCM82633A004352', N'Falla en el arranque', N'Batería descargada', N'Cerrada',
        '2026-09-28T15:00:00', '2026-09-30T22:00:00', N'Se reemplazó la batería.'),
    (2, '2T1BURHE4FC000115', N'Cambio de aceite y filtro', N'Mantenimiento de 10,000 km', N'Cerrada',
        '2026-10-05T14:00:00', '2026-10-05T20:30:00', N'Se cambió el aceite y el filtro de aceite.'),
    (3, '1HGCM82633A004352', N'Ruido en la suspensión delantera', N'Bujes desgastados; pendiente de repuestos', N'Abierta',
        '2026-10-12T15:30:00', NULL, NULL),
    (4, 'JTEBU5JR8K5612345', N'Cambio de pastillas de freno', N'Pastillas delanteras al 10 %', N'Cerrada',
        '2026-10-14T14:00:00', '2026-10-15T23:00:00', N'Se cambiaron las pastillas delanteras.'),
    (5, 'JM1BPACL0K1234567', N'Luz de check engine encendida', NULL, N'Abierta',
        '2026-10-20T13:00:00', NULL, NULL),
    (6, 'LALPCJ0F6M1000121', N'Revisión de cadena y frenos', NULL, N'Abierta',
        '2026-10-21T16:00:00', NULL, NULL);

-- La orden se enlaza al vehiculo por su VIN; la placa se copia como dato historico.
INSERT dbo.OrdenesReparacion (IdVehiculo, VIN, Placa, FechaApertura, MotivoIngreso, Diagnostico,
                              Estado, FechaCierre, ObservacionesCierre)
SELECT v.IdVehiculo, v.VIN, v.Placa, d.FechaApertura, d.MotivoIngreso, d.Diagnostico,
       d.Estado, d.FechaCierre, d.ObservacionesCierre
FROM @Ordenes AS d
INNER JOIN dbo.Vehiculos AS v ON v.VIN = d.VIN
WHERE NOT EXISTS (SELECT 1 FROM dbo.OrdenesReparacion AS o
                  WHERE o.VIN = d.VIN AND o.MotivoIngreso = d.MotivoIngreso AND o.FechaApertura = d.FechaApertura)
ORDER BY d.Orden;
PRINT CONCAT(N'Ordenes de ejemplo agregadas: ', @@ROWCOUNT);

COMMIT TRANSACTION;
GO
