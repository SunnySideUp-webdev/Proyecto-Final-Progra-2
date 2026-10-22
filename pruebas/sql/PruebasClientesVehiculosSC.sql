-- AutoManager DMS
-- Pruebas del Modulo 1: Clientes y vehiculos
-- Integrante: Sharon Consuegra
-- Comprueba que las tablas aceptan datos validos y rechazan duplicados,
-- formatos invalidos y referencias inexistentes.
-- Todo ocurre dentro de una transaccion que termina en ROLLBACK: no deja datos.
-- Usa valores exclusivos de prueba (DPI 9..., placas ZP9..., VIN 9ZZTEST...) para no
-- chocar con los datos que ya existan en la base.
-- Ejecutar con sqlcmd (opcion -I) o SSMS sobre AutoManagerDMS.

SET NOCOUNT ON;
SET XACT_ABORT OFF;
SET QUOTED_IDENTIFIER ON;
USE AutoManagerDMS;

DECLARE @Resultados TABLE (Orden INT IDENTITY(1,1), Caso NVARCHAR(120), Esperado INT, Obtenido INT);
DECLARE @Obtenido INT, @IdAna INT, @IdLuis INT;
DECLARE @ClientesAntes INT = (SELECT COUNT(*) FROM dbo.Clientes);
DECLARE @VehiculosAntes INT = (SELECT COUNT(*) FROM dbo.Vehiculos);

BEGIN TRANSACTION;

-- ===== Registros validos (esperado: 0 = sin error) =====
BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono)
    VALUES ('9100000000101', N'Ana', N'Pérez', '55551234');
    SET @IdAna = SCOPE_IDENTITY(); SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Cliente válido (campos obligatorios)', 0, @Obtenido);

BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono, Correo, Direccion)
    VALUES ('9200000000101', N'Luis', N'Gómez', '50212345678', N'luis@correo.com', N'Zona 1, Guatemala');
    SET @IdLuis = SCOPE_IDENTITY(); SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Cliente válido con correo, dirección y teléfono de 11 dígitos', 0, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdAna, 'ZP901AA', '9ZZTEST0000000001', N'Honda', N'Accord', 2003);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Vehículo válido con VIN', 0, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdAna, 'ZM902BB', '9ZZTEST0000000002', N'Honda', N'CB190', 2021);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Segundo vehículo válido del mismo cliente', 0, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP903CC', '9ZZTEST0000000003', N'Toyota', N'Hilux', 1981);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Vehículo de otro cliente con el primer año aceptado (1981)', 0, @Obtenido);

-- ===== Clientes rechazados =====
BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono)
    VALUES ('9100000000101', N'Otra', N'Persona', '55559999');
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'DPI duplicado', 2627, @Obtenido);

BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono)
    VALUES ('123456789010', N'Doce', N'Dígitos', '55559999');
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'DPI de 12 dígitos', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono)
    VALUES ('12345678901AB', N'Con', N'Letras', '55559999');
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'DPI con letras', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono)
    VALUES ('9300000000101', N'   ', N'Vacío', '55559999');
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Nombres solo con espacios', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono)
    VALUES ('9300000000101', N'Siete', N'Dígitos', '5555123');
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Teléfono de 7 dígitos', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono)
    VALUES ('9300000000101', N'Con', N'Guion', '5555-1234');
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Teléfono con guion (debe llegar normalizado)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos, Telefono, Correo)
    VALUES ('9300000000101', N'Correo', N'Inválido', '55559999', N'correo-sin-arroba.com');
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Correo sin @', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Clientes (DPI, Nombres, Apellidos)
    VALUES ('9300000000101', N'Sin', N'Teléfono');
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Teléfono omitido (obligatorio)', 515, @Obtenido);

-- ===== Vehiculos rechazados =====
BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP901AA', '9ZZTEST0000000011', N'Kia', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Placa duplicada', 2627, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'p-555xyz', '9ZZTEST0000000012', N'Kia', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Placa sin normalizar (minúsculas y guion)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'P12A', '9ZZTEST0000000013', N'Kia', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Placa de 4 caracteres', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP904DD', '9ZZTEST0000000001', N'Kia', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'VIN duplicado', 2627, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP904DE', N'Kia', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Vehículo sin VIN (obligatorio)', 515, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP904DD', '9ZZTEST000000000O', N'Kia', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'VIN con letra O (no permitida)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP904DD', '9ZZTEST000000001', N'Kia', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'VIN de 16 caracteres', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (999999, 'ZP905EE', '9ZZTEST0000000015', N'Kia', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Vehículo con cliente inexistente (llave foránea)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP906FF', '9ZZTEST0000000016', N'Ford', N'Escort', 1980);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Año 1980 (antes del VIN de 17 caracteres)', 547, @Obtenido);

BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP907GG', '9ZZTEST0000000017', N'', N'Rio', 2019);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Marca vacía', 547, @Obtenido);

-- El máximo de año depende de la fecha y lo valida la Capa Lógica (C#),
-- por eso la base acepta un año futuro si alguien inserta directo en SQL.
BEGIN TRY
    INSERT dbo.Vehiculos (IdCliente, Placa, VIN, Marca, Modelo, Anio)
    VALUES (@IdLuis, 'ZP908HH', '9ZZTEST0000000018', N'Kia', N'EV9', 2035);
    SET @Obtenido = 0;
END TRY BEGIN CATCH SET @Obtenido = ERROR_NUMBER(); END CATCH;
INSERT @Resultados VALUES (N'Año 2035: la base lo acepta (el máximo lo controla C#)', 0, @Obtenido);

-- ===== Consulta de los vehículos de un cliente =====
SET @Obtenido = (SELECT COUNT(*) FROM dbo.Vehiculos WHERE IdCliente = @IdAna);
INSERT @Resultados VALUES (N'Ana tiene exactamente 2 vehículos asociados', 2, @Obtenido);

-- ===== Relación por VIN (la usarán las órdenes de reparación) =====
SET @Obtenido = (SELECT COUNT(*) FROM sys.key_constraints
                 WHERE name = N'UQ_Vehiculos_VIN' AND parent_object_id = OBJECT_ID(N'dbo.Vehiculos'));
INSERT @Resultados VALUES (N'Vehiculos.VIN tiene la restricción única UQ_Vehiculos_VIN', 1, @Obtenido);

-- ===== Fechas en UTC (se revisa la definición: en un servidor UTC ambas horas coinciden) =====
SET @Obtenido = (SELECT COUNT(*) FROM sys.default_constraints
                 WHERE name IN (N'DF_Clientes_FechaRegistro', N'DF_Vehiculos_FechaRegistro')
                   AND definition = N'(sysutcdatetime())');
INSERT @Resultados VALUES (N'FechaRegistro usa SYSUTCDATETIME() en Clientes y Vehiculos', 2, @Obtenido);

-- Ninguna columna de la base debe usar la hora local del servidor como valor por defecto.
SET @Obtenido = (SELECT COUNT(*) FROM sys.default_constraints
                 WHERE definition LIKE N'%sysdatetime()%' AND definition NOT LIKE N'%sysutcdatetime()%'
                    OR definition LIKE N'%getdate()%');
INSERT @Resultados VALUES (N'Ningún DEFAULT usa SYSDATETIME() ni GETDATE()', 0, @Obtenido);

SET @Obtenido = (SELECT CONVERT(INT, value) FROM sys.database_scoped_configurations WHERE name = N'IDENTITY_CACHE');
INSERT @Resultados VALUES (N'IDENTITY_CACHE desactivado en AutoManagerDMS', 0, @Obtenido);

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

SELECT CASE WHEN (SELECT COUNT(*) FROM dbo.Clientes) = @ClientesAntes
             AND (SELECT COUNT(*) FROM dbo.Vehiculos) = @VehiculosAntes
            THEN 'Sí: no quedaron datos de prueba'
            ELSE 'NO: quedaron datos de prueba' END AS TablasIntactas;
