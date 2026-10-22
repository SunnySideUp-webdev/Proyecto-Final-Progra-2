-- AutoManager DMS
-- Datos de ejemplo del Modulo 3 (Inventario)
-- Integrante: Pavel Alexander Palma Carrillo (carne 0900-24-4296)
-- Repuestos FICTICIOS compatibles con los vehiculos de ejemplo (90_DatosEjemploSC.sql).
-- Busca cada repuesto por su codigo: si ya existe, no lo vuelve a insertar ni cambia su
-- stock. Se puede ejecutar varias veces sin duplicar nada.
-- El stock indicado es el de ingreso; 93_DatosEjemploRepuestosOrdenSC.sql descuenta despues
-- las unidades usadas en las ordenes de ejemplo.
-- Requiere 03_InventarioPP.sql.

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET NOCOUNT ON;
GO

USE AutoManagerDMS;
GO

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Repuestos TABLE (
    Orden INT NOT NULL, codigo VARCHAR(15) NOT NULL, nombre NVARCHAR(50) NOT NULL,
    descripcion NVARCHAR(100) NOT NULL, stock INT NOT NULL, marcacarro NVARCHAR(50) NOT NULL,
    modelocarro NVARCHAR(50) NOT NULL, aniocarro INT NOT NULL);

INSERT @Repuestos VALUES
    (1, 'FIL-ACE-COR15', N'Filtro de aceite', N'Filtro de aceite para motor 1.8 L', 12, N'Toyota', N'Corolla', 2015),
    (2, 'ACE-5W30-COR15', N'Aceite de motor 5W-30', N'Aceite sintético, envase de un cuarto de galón', 40, N'Toyota', N'Corolla', 2015),
    (3, 'PAS-FRE-HLX20', N'Pastillas de freno delanteras', N'Juego de cuatro pastillas cerámicas', 6, N'Toyota', N'Hilux', 2020),
    (4, 'BAT-12V-ACC03', N'Batería de 12 V', N'Batería de 12 V y 550 A de arranque en frío', 3, N'Honda', N'Accord', 2003),
    (5, 'BUJ-SUS-ACC03', N'Bujes de suspensión delantera', N'Par de bujes de hule para la tijera inferior', 0, N'Honda', N'Accord', 2003),
    (6, 'KIT-ARR-CB190', N'Kit de arrastre', N'Cadena, piñón y corona', 2, N'Honda', N'CB190', 2021),
    (7, 'SEN-O2-MAZ19', N'Sensor de oxígeno', N'Sensor anterior al catalizador', 4, N'Mazda', N'3', 2019),
    (8, 'FIL-AIR-MAZ19', N'Filtro de aire', N'Filtro de aire para motor 2.0 L', 9, N'Mazda', N'3', 2019);

INSERT dbo.Repuesto (codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro)
SELECT d.codigo, d.nombre, d.descripcion, d.stock, d.marcacarro, d.modelocarro, d.aniocarro
FROM @Repuestos AS d
WHERE NOT EXISTS (SELECT 1 FROM dbo.Repuesto AS r WHERE r.codigo = d.codigo)
ORDER BY d.Orden;
PRINT CONCAT(N'Repuestos de ejemplo agregados: ', @@ROWCOUNT);

COMMIT TRANSACTION;
GO
