-- AutoManager DMS
-- Repuestos utilizados en las ordenes de reparacion
-- Integrante: Sharon Yasmin Consuegra Borrayo (carne 0900-25-22148)
-- Relaciona cada orden de reparacion (02) con los repuestos del inventario (03) y la
-- cantidad que se uso. La aplicacion descuenta el stock en la misma transaccion en la
-- que registra el uso, y lo devuelve si el uso se quita mientras la orden sigue abierta.
-- Requiere 02_OrdenesReparacionNT.sql y 03_InventarioPP.sql. Se puede ejecutar varias veces.

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

USE AutoManagerDMS;
GO

IF OBJECT_ID(N'dbo.OrdenesReparacion', N'U') IS NULL OR OBJECT_ID(N'dbo.Repuesto', N'U') IS NULL
    THROW 50040, N'Ejecuta primero 02_OrdenesReparacionNT.sql y 03_InventarioPP.sql.', 1;
GO

IF OBJECT_ID(N'dbo.RepuestosOrden', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.RepuestosOrden (
        IdRepuestoOrden INT IDENTITY(1,1) NOT NULL,
        IdOrden INT NOT NULL,
        IdRepuesto INT NOT NULL,
        Cantidad INT NOT NULL,
        FechaRegistro DATETIME2 NOT NULL
            CONSTRAINT DF_RepuestosOrden_FechaRegistro DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_RepuestosOrden
            PRIMARY KEY (IdRepuestoOrden),

        CONSTRAINT FK_RepuestosOrden_OrdenesReparacion
            FOREIGN KEY (IdOrden) REFERENCES dbo.OrdenesReparacion (IdOrden),

        CONSTRAINT FK_RepuestosOrden_Repuesto
            FOREIGN KEY (IdRepuesto) REFERENCES dbo.Repuesto (idrepuesto),

        CONSTRAINT CK_RepuestosOrden_Cantidad
            CHECK (Cantidad > 0)
    );
END
GO

-- Aceleran la lista de repuestos de una orden y los reportes por repuesto.
IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_RepuestosOrden_IdOrden' AND object_id = OBJECT_ID(N'dbo.RepuestosOrden'))
    CREATE INDEX IX_RepuestosOrden_IdOrden ON dbo.RepuestosOrden (IdOrden);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_RepuestosOrden_IdRepuesto' AND object_id = OBJECT_ID(N'dbo.RepuestosOrden'))
    CREATE INDEX IX_RepuestosOrden_IdRepuesto ON dbo.RepuestosOrden (IdRepuesto);
GO

PRINT N'Repuestos utilizados en ordenes: tabla RepuestosOrden lista.';
GO
