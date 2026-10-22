-- AutoManager DMS
-- Pruebas de las tablas Repuesto (Modulo 3, Pavel) y RepuestosOrden (repuestos utilizados en ordenes)
-- Integrante: Sharon Consuegra
-- Comprueba que las tablas aceptan datos validos y rechazan stock negativo, codigos repetidos,
-- textos vacios, anios fuera de rango, cantidades no positivas y referencias inexistentes.
-- Todo ocurre dentro de una transaccion que termina en ROLLBACK: no deja datos.
-- Usa codigos exclusivos de prueba (ZQA-...) para no chocar con los datos que ya existan.
-- Ejecutar con sqlcmd (opcion -I) o SSMS sobre AutoManagerDMS, despues de 03 y 04.

SET NOCOUNT ON;
SET XACT_ABORT OFF;
SET QUOTED_IDENTIFIER ON;
USE AutoManagerDMS;

DECLARE @Resultados TABLE (Orden INT IDENTITY(1,1), Caso NVARCHAR(120), Esperado INT, Obtenido INT);
DECLARE @Obtenido INT, @IdRepuesto INT, @IdOrden INT;
DECLARE @RepuestosAntes INT = (SELECT COUNT(*) FROM dbo.Repuesto);
DECLARE @UsosAntes INT = (SELECT COUNT(*) FROM dbo.RepuestosOrden);

BEGIN TRANSACTION;

-- ===== Repuesto: registros validos (esperado: 0 = sin error) =====
BEGIN TRY
    INSERT dbo.Repuesto (codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro)
    VALUES ('ZQA-T-001', N'Bujía de iridio', N'Prueba: juego de 4', 3, N'Toyota', N'Yaris', 2018);
    SET @IdRepuesto = SCOPE_IDENTITY(); SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Repuesto válido', 0, @Obtenido);

SET @Obtenido = (SELECT COUNT(*) FROM dbo.Repuesto WHERE idrepuesto = @IdRepuesto AND nombre = N'Bujía de iridio');
INSERT @Resultados VALUES (N'El nombre conserva la tilde (columna NVARCHAR)', 1, @Obtenido);

BEGIN TRY
    INSERT dbo.Repuesto (codigo, nombre, descripcion, marcacarro, modelocarro, aniocarro)
    VALUES ('ZQA-T-002', N'Filtro', N'Prueba sin stock', N'Kia', N'Rio', 2020);
    SET @Obtenido = (SELECT stock FROM dbo.Repuesto WHERE codigo = 'ZQA-T-002');
END TRY BEGIN CATCH SET @Obtenido = -ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Sin stock indicado se registra con 0 (DF_Repuesto_Stock)', 0, @Obtenido);

-- ===== Repuesto: datos rechazados =====
BEGIN TRY
    INSERT dbo.Repuesto (codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro)
    VALUES ('ZQA-T-001', N'Otro', N'Código repetido', 1, N'Toyota', N'Yaris', 2018);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Código repetido (UQ_Repuesto_Codigo)', 2627, @Obtenido);

BEGIN TRY
    INSERT dbo.Repuesto (codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro)
    VALUES ('ZQA-T-003', N'Negativo', N'Stock negativo', -1, N'Toyota', N'Yaris', 2018);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Stock negativo al registrar (CK_Repuesto_Stock)', 547, @Obtenido);

BEGIN TRY
    UPDATE dbo.Repuesto SET stock = stock - 5 WHERE idrepuesto = @IdRepuesto;
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Descontar más unidades de las que hay (CK_Repuesto_Stock)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Repuesto (codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro)
    VALUES ('ZQA-T-004', N'Antiguo', N'Año 1980', 1, N'Toyota', N'Corona', 1980);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Año del vehículo 1980 (CK_Repuesto_Anio)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Repuesto (codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro)
    VALUES ('ZQA-T-005', N'', N'Nombre vacío', 1, N'Toyota', N'Yaris', 2018);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Nombre vacío (CK_Repuesto_Textos)', 547, @Obtenido);

-- ===== RepuestosOrden =====
SET @IdOrden = (SELECT TOP 1 IdOrden FROM dbo.OrdenesReparacion ORDER BY IdOrden);

BEGIN TRY
    INSERT dbo.RepuestosOrden (IdOrden, IdRepuesto, Cantidad) VALUES (@IdOrden, @IdRepuesto, 2);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Uso válido de un repuesto en una orden', 0, @Obtenido);

BEGIN TRY
    INSERT dbo.RepuestosOrden (IdOrden, IdRepuesto, Cantidad) VALUES (@IdOrden, @IdRepuesto, 0);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Cantidad 0 (CK_RepuestosOrden_Cantidad)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.RepuestosOrden (IdOrden, IdRepuesto, Cantidad) VALUES (-1, @IdRepuesto, 1);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Orden inexistente (FK_RepuestosOrden_OrdenesReparacion)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.RepuestosOrden (IdOrden, IdRepuesto, Cantidad) VALUES (@IdOrden, -1, 1);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Repuesto inexistente (FK_RepuestosOrden_Repuesto)', 547, @Obtenido);

BEGIN TRY
    DELETE FROM dbo.Repuesto WHERE idrepuesto = @IdRepuesto;
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'No se puede borrar un repuesto usado en una orden (conserva el historial)', 547, @Obtenido);

SET @Obtenido = (SELECT COUNT(*) FROM sys.default_constraints
                 WHERE name = N'DF_RepuestosOrden_FechaRegistro' AND definition = N'(sysutcdatetime())');
INSERT @Resultados VALUES (N'FechaRegistro usa SYSUTCDATETIME()', 1, @Obtenido);

SET @Obtenido = (SELECT COUNT(*) FROM sys.indexes
                 WHERE object_id = OBJECT_ID(N'dbo.RepuestosOrden')
                   AND name IN (N'IX_RepuestosOrden_IdOrden', N'IX_RepuestosOrden_IdRepuesto'));
INSERT @Resultados VALUES (N'Índices por orden y por repuesto', 2, @Obtenido);

ROLLBACK TRANSACTION;

-- ===== Resultados (la tabla variable conserva los datos después del ROLLBACK) =====
SELECT Orden,
       CASE WHEN Esperado = Obtenido THEN 'OK' ELSE 'FALLO' END AS Resultado,
       Caso,
       Esperado,
       Obtenido
FROM @Resultados
ORDER BY Orden;

SELECT SUM(CASE WHEN Esperado = Obtenido THEN 1 ELSE 0 END) AS Correctas,
       SUM(CASE WHEN Esperado <> Obtenido THEN 1 ELSE 0 END) AS Fallidas
FROM @Resultados;

SELECT CASE WHEN (SELECT COUNT(*) FROM dbo.Repuesto) = @RepuestosAntes
             AND (SELECT COUNT(*) FROM dbo.RepuestosOrden) = @UsosAntes
            THEN 'Sí: no quedaron datos de prueba'
            ELSE 'NO: quedaron datos de prueba' END AS TablasIntactas;
