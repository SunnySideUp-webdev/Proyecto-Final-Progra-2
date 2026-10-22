# Convenciones de la base de datos AutoManagerDMS

Acuerdos para que todos los módulos usen una sola base de datos de forma consistente.
Cada integrante mantiene sus propios scripts; este documento solo describe las reglas comunes.

## 1. Fechas: siempre en UTC

- Las columnas de fecha usan `DATETIME2` y su valor por defecto es **`SYSUTCDATETIME()`**.
- **No usar `SYSDATETIME()` ni `GETDATE()`**: devuelven la hora local *del servidor*.
  En Docker el servidor está en UTC, pero en SQL Server Express de Windows está en hora de
  Guatemala, así que la misma columna guardaría horas distintas según la computadora.
- La aplicación convierte a hora de Guatemala **solo al mostrar**, con
  `CapaLogica.ClsZonaHorariaLNSC.FncConvertirUtcAGuatemalaSC`. Para mostrar una fecha en una
  página se puede usar `ClsFormatoSC.FncFormatearFechaSC`, que ya hace esa conversión.
- Al leer una fecha con `SqlDataReader`, marcarla como UTC para evitar conversiones dobles:

  ```csharp
  DateTime.SpecifyKind(lector.GetDateTime(i), DateTimeKind.Utc)
  ```

- Para filtrar por fechas locales (por ejemplo, "órdenes de hoy"), convertir primero el
  inicio y el fin del día de Guatemala a UTC con `ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC`.

## 2. Relación con vehículos

Las tablas que se refieren a un vehículo usan su **VIN**. En `dbo.Vehiculos` el VIN es
obligatorio y único (restricción `UQ_Vehiculos_VIN`), igual que la placa:

```sql
VIN VARCHAR(17) NOT NULL,
CONSTRAINT FK_<Tabla>_Vehiculos
    FOREIGN KEY (VIN) REFERENCES dbo.Vehiculos (VIN) ON UPDATE CASCADE
```

`ON UPDATE CASCADE` hace que, si se corrige un VIN mal escrito en `dbo.Vehiculos`, las filas
relacionadas se actualicen solas. La placa y el propietario se obtienen con un `JOIN`
(`ON v.VIN = o.VIN`) a `dbo.Vehiculos` y `dbo.Clientes`.

Si una tabla guarda **también** `IdVehiculo` (como `OrdenesReparacion`), la llave foránea usa
los dos datos para que siempre sean del mismo vehículo (restricción `UQ_Vehiculos_IdVehiculo_VIN`):

```sql
CONSTRAINT FK_<Tabla>_Vehiculos
    FOREIGN KEY (IdVehiculo, VIN) REFERENCES dbo.Vehiculos (IdVehiculo, VIN) ON UPDATE CASCADE
```

Desde C# se pueden usar `ClsVehiculoLNSC.FncBuscarVehiculosSC(placaOVin)` (el VIN exacto
aparece primero) y `ClsVehiculoLNSC.FncObtenerVehiculoSC(idVehiculo)`.

## 3. Órdenes de reparación (`02_OrdenesReparacionNT.sql`)

La tabla `dbo.OrdenesReparacion` se crea con `database/02_OrdenesReparacionNT.sql`, a partir de
la estructura que usa el módulo de Neider (`IdVehiculo`, `VIN` y la `Placa` histórica), con:

- llave foránea `(IdVehiculo, VIN)` hacia `dbo.Vehiculos` con `ON UPDATE CASCADE`;
- `CK_OrdenesReparacion_Cierre`: una orden abierta no tiene fecha de cierre y una cerrada sí;
- `CK_OrdenesReparacion_Motivo`: el motivo de ingreso no puede estar vacío;
- `FechaApertura` en UTC e índice `IX_OrdenesReparacion_VIN` para el historial por VIN.

Si la tabla ya existe con la estructura anterior (VIN opcional, hora local, sin llave foránea),
el script la actualiza sin perder datos: completa el VIN desde `dbo.Vehiculos`, pasa las fechas
a UTC con la diferencia horaria del servidor y agrega las reglas. Si encuentra datos que no las
cumplen, se detiene con un mensaje que indica qué corregir; después se vuelve a ejecutar.

Al cerrar una orden, `FechaCierre` se llena con `SYSUTCDATETIME()` (o `DateTime.UtcNow` desde C#).

## 4. Inventario y repuestos utilizados (`03_InventarioPP.sql` y `04_RepuestosOrdenSC.sql`)

`dbo.Repuesto` (módulo de Pavel) conserva sus columnas originales en minúsculas (`idrepuesto`, `codigo`,
`stock`, `marcacarro`…), con estas reglas:

- `UQ_Repuesto_Codigo`: el código es único;
- `CK_Repuesto_Stock`: el stock nunca queda negativo (su regla original `chk_stock_negativo`, renombrada);
- `CK_Repuesto_Textos` y `CK_Repuesto_Anio`: textos no vacíos y año del vehículo compatible desde 1981;
- textos en `NVARCHAR`, como el resto de la base.

Si la tabla ya existe con el script original (`database/Inventario.sql` de su rama), `03` la actualiza sin
perder datos, igual que `02` con las órdenes.

`dbo.RepuestosOrden` registra los repuestos utilizados en cada orden: `IdOrden` (llave foránea a
`dbo.OrdenesReparacion`), `IdRepuesto` (llave foránea a `dbo.Repuesto`), `Cantidad` (`CK_RepuestosOrden_Cantidad`:
mayor que cero) y `FechaRegistro` en UTC. Las llaves foráneas impiden borrar un repuesto o una orden que ya
tenga usos registrados, para conservar el historial.

El stock se modifica siempre con una sola instrucción sobre el valor actual, nunca leyendo el valor en C# y
escribiéndolo después:

```sql
UPDATE dbo.Repuesto SET stock = stock + @Cantidad WHERE idrepuesto = @IdRepuesto;                     -- entrada
UPDATE dbo.Repuesto SET stock = stock - @Cantidad WHERE idrepuesto = @IdRepuesto AND stock >= @Cantidad; -- uso
```

El uso en una orden se registra en una transacción junto con el descuento del stock (ver
`ClsRepuestoOrdenDASC`), y solo mientras la orden está abierta.

## 5. Scripts

- Un script por módulo en `database/`, con número para fijar el orden de ejecución:
  `00_` crea la base, `01_ClientesVehiculosSC.sql`, `02_OrdenesReparacionNT.sql`, `03_InventarioPP.sql`,
  `04_RepuestosOrdenSC.sql`, etc.
  Los datos de ejemplo (siempre ficticios) van al final, en `90_` en adelante, y se insertan
  sin identificadores fijos (buscando por DPI, placa o VIN) para no chocar con datos existentes.
  Docker ejecuta automáticamente los archivos `database/[0-9][0-9]_*.sql` en orden.
- Cada script empieza con `USE AutoManagerDMS;` y debe poder ejecutarse varias veces
  (`IF OBJECT_ID(...) IS NULL`, `IF DB_ID(...) IS NULL`, `IF NOT EXISTS (...)`).
- Todas las restricciones llevan nombre (`PK_`, `FK_`, `UQ_`, `CK_`, `DF_`, `IX_`, `UX_`)
  para que sean iguales en todas las computadoras y se puedan modificar después.
- Incluir `SET QUOTED_IDENTIFIER ON;` al inicio (es la configuración que recomienda SQL Server
  y la obligatoria si algún módulo crea índices filtrados); con `sqlcmd`, usar la opción `-I`.

## 6. Identificadores

- `IDENTITY_CACHE` está desactivado en `AutoManagerDMS` para evitar saltos de 1000 cuando
  el servidor se detiene. Aun así, **los identificadores pueden tener huecos** (por ejemplo,
  si una transacción se deshace); no se debe suponer que son consecutivos.

## 7. Conexión desde C#

- Obtener las conexiones siempre con `CapaDatos.ClsConexionDASC.FncObtenerConexionSC()`.
- La cadena de conexión `AutoManagerDMS` está en `CapaPresentacion/ConnectionStrings.config`
  (local, no se sube a GitHub; plantilla: `ConnectionStrings.config.example`).
- Usar siempre parámetros (`SqlCommand.Parameters`), nunca concatenar datos del usuario.
