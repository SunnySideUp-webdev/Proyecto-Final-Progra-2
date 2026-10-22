USE AutoManagerDMS;
GO

IF OBJECT_ID('dbo.OrdenesReparacion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrdenesReparacion
    (
        IdOrden INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_OrdenesReparacion PRIMARY KEY,

        IdVehiculo INT NOT NULL,

        VIN VARCHAR(17) NULL,

        Placa VARCHAR(20) NULL,

        FechaApertura DATETIME2 NOT NULL
            CONSTRAINT DF_Ordenes_FechaApertura
            DEFAULT SYSDATETIME(),

        MotivoIngreso NVARCHAR(500) NOT NULL,

        Diagnostico NVARCHAR(1000) NULL,

        Estado NVARCHAR(20) NOT NULL
            CONSTRAINT DF_Ordenes_Estado
            DEFAULT N'Abierta',

        FechaCierre DATETIME2 NULL,

        ObservacionesCierre NVARCHAR(1000) NULL,

        CONSTRAINT CK_Ordenes_Estado
            CHECK (Estado IN (N'Abierta', N'Cerrada'))
    );
END;
GO
