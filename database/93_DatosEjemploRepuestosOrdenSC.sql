-- AutoManager DMS
-- Datos de ejemplo: repuestos utilizados en las ordenes de ejemplo
-- Integrante: Sharon Yasmin Consuegra Borrayo (carne 0900-25-22148)
-- Registra que repuestos (92_DatosEjemploInventarioPP.sql) se usaron en las ordenes
-- FICTICIAS de 91_DatosEjemploReportesFP.sql y descuenta esas unidades del stock, igual que
-- lo hace la aplicacion. Cada orden se busca por VIN, motivo y fecha de apertura, y cada
-- repuesto por su codigo; si alguno no existe, ese uso se omite.
-- Se puede ejecutar varias veces: un uso ya registrado no se repite ni descuenta otra vez.
-- Requiere 04_RepuestosOrdenSC.sql, 91_DatosEjemploReportesFP.sql y 92_DatosEjemploInventarioPP.sql.
-- Las fechas estan en UTC (Guatemala = UTC - 6 horas).

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
GO

USE AutoManagerDMS;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Usos TABLE (
    Orden INT NOT NULL, VIN VARCHAR(17) NOT NULL, MotivoIngreso NVARCHAR(500) NOT NULL,
    FechaApertura DATETIME2 NOT NULL, Codigo VARCHAR(15) NOT NULL, Cantidad INT NOT NULL,
    FechaRegistro DATETIME2 NOT NULL);

INSERT @Usos VALUES
    (1, '1HGCM82633A004352', N'Falla en el arranque', '2026-09-28T15:00:00', 'BAT-12V-ACC03', 1, '2026-09-29T17:00:00'),
    (2, '2T1BURHE4FC000115', N'Cambio de aceite y filtro', '2026-10-05T14:00:00', 'FIL-ACE-COR15', 1, '2026-10-05T16:00:00'),
    (3, '2T1BURHE4FC000115', N'Cambio de aceite y filtro', '2026-10-05T14:00:00', 'ACE-5W30-COR15', 4, '2026-10-05T16:05:00'),
    (4, 'JTEBU5JR8K5612345', N'Cambio de pastillas de freno', '2026-10-14T14:00:00', 'PAS-FRE-HLX20', 1, '2026-10-15T18:00:00'),
    (5, 'LALPCJ0F6M1000121', N'Revisión de cadena y frenos', '2026-10-21T16:00:00', 'KIT-ARR-CB190', 1, '2026-10-21T20:00:00');

DECLARE @Registrados TABLE (IdRepuesto INT NOT NULL, Cantidad INT NOT NULL);

INSERT dbo.RepuestosOrden (IdOrden, IdRepuesto, Cantidad, FechaRegistro)
OUTPUT INSERTED.IdRepuesto, INSERTED.Cantidad INTO @Registrados
SELECT o.IdOrden, r.idrepuesto, u.Cantidad, u.FechaRegistro
FROM @Usos AS u
INNER JOIN dbo.OrdenesReparacion AS o
    ON o.VIN = u.VIN AND o.MotivoIngreso = u.MotivoIngreso AND o.FechaApertura = u.FechaApertura
INNER JOIN dbo.Repuesto AS r ON r.codigo = u.Codigo
WHERE NOT EXISTS (SELECT 1 FROM dbo.RepuestosOrden AS x
                  WHERE x.IdOrden = o.IdOrden AND x.IdRepuesto = r.idrepuesto AND x.FechaRegistro = u.FechaRegistro)
ORDER BY u.Orden;
PRINT CONCAT(N'Usos de repuestos de ejemplo agregados: ', @@ROWCOUNT);

-- Descuenta solo lo que se acaba de registrar. Si algun repuesto no alcanza, CK_Repuesto_Stock
-- detiene el script y la transaccion se deshace completa.
UPDATE r SET r.stock = r.stock - n.Total
FROM dbo.Repuesto AS r
INNER JOIN (SELECT IdRepuesto, SUM(Cantidad) AS Total FROM @Registrados GROUP BY IdRepuesto) AS n
    ON n.IdRepuesto = r.idrepuesto;

COMMIT TRANSACTION;
GO
