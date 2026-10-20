
-- AutoManager DMS
-- Modulos 4 y 6: Ordenes de reparacion
-- Integrante: Neider Toma
-- Script inicial para nuevas bases de datos

USE AutoManagerDMS;
GO

CREATE TABLE dbo.OrdenesReparacion (
    IdOrden INT IDENTITY(1,1) PRIMARY KEY,
    VIN VARCHAR(17) NOT NULL,
    FechaApertura DATETIME2 NOT NULL
        DEFAULT SYSDATETIME(),
    MotivoIngreso NVARCHAR(500) NOT NULL,
    Diagnostico NVARCHAR(1000) NULL,
    Estado NVARCHAR(20) NOT NULL
        DEFAULT 'Abierta',
    FechaCierre DATETIME2 NULL,
    ObservacionesCierre NVARCHAR(1000) NULL,

    CONSTRAINT CK_OrdenesReparacion_Estado
        CHECK (Estado IN ('Abierta', 'Cerrada')),

    CONSTRAINT CK_OrdenesReparacion_VIN
        CHECK (LEN(VIN) = 17)
);
GO

-- Pendiente: agregar la llave foranea
-- hacia Vehiculos(VIN) al integrar el modulo 1.
