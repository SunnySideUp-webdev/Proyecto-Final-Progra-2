# Módulo 4: Reportes

Guía del módulo de reportes de AutoManager DMS: qué hace, cómo está construido en las
cuatro capas y por qué se tomó cada decisión. Sirve para estudiarlo, probarlo y explicarlo.

> Antes de presentarlo, completa tu nombre completo y tu carné en el encabezado de cada
> archivo del módulo (donde dice `PENDIENTE`) y en `database/91_DatosEjemploReportesFP.sql`.

---

## 1. Qué hace el módulo

Muestra información del taller **sin modificar nada**: solo consulta la base de datos.
Tiene cuatro secciones, que se eligen con las pestañas de la página **Reportes**:

| Sección | Qué responde | Dirección |
|---|---|---|
| **Resumen** | ¿Cómo va el taller? Órdenes abiertas, cerradas este mes, vehículos atendidos, tiempo promedio de reparación y la orden abierta más antigua. | `Reportes.aspx` |
| **Órdenes abiertas** | ¿Qué está pendiente y desde cuándo? Las órdenes abiertas de la más antigua a la más nueva, con los días que llevan abiertas (en rojo si llevan una semana o más). | `Reportes.aspx?seccion=abiertas` |
| **Órdenes cerradas** | ¿Qué se terminó en un período y cuánto tardó? Filtro por fechas de cierre y duración de cada reparación. | `Reportes.aspx?seccion=cerradas&desde=2026-10-01&hasta=2026-10-22` |
| **Historial por cliente** | ¿Qué reparaciones ha tenido un cliente? Todas las órdenes de todos sus vehículos. | `Reportes.aspx?seccion=historial&idCliente=1` |

Además, el botón **Imprimir** imprime solo el reporte (sin menú, botones ni filtros) y con todas
las columnas, aunque en pantalla algunas se oculten por falta de espacio.

**Qué no hace (fuera del alcance acordado):** no calcula precios, costos ni totales en dinero.
Los reportes de **existencias de repuestos** y **repuestos utilizados** se agregarán cuando el
módulo de inventario (Pavel) esté integrado; hoy no se muestran para no presentar datos inventados.

---

## 2. Dónde está cada archivo

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Entidades | `CapaEntidades/ClsReporteOrdenENFP.cs` | Una fila de reporte: datos de la orden + su vehículo + su cliente, y los tiempos calculados (días abierta, minutos de reparación). |
| Entidades | `CapaEntidades/ClsResumenReportesENFP.cs` | Los indicadores del resumen. |
| Datos | `CapaDatos/ClsReporteDAFP.cs` | Las consultas SQL (solo `SELECT`, todas con parámetros). |
| Lógica | `CapaLogica/ClsReporteLNFP.cs` | Reglas: rangos de fechas con el calendario de Guatemala, validación de los filtros y cálculo de tiempos. |
| Presentación | `CapaPresentacion/Reportes/Reportes.aspx` | Lo que se ve: pestañas, tarjetas y tablas. |
| Presentación | `CapaPresentacion/Reportes/Reportes.aspx.cs` | Lee la dirección (`?seccion=`, `?desde=`…), llama a la Capa Lógica y llena las tablas. |
| Estilos | `CapaPresentacion/Content/autodms.css` (sección «Reportes» e «Impresión») | Pestañas, etiquetas de estado, días resaltados y la versión para imprimir. |
| Datos de ejemplo | `database/91_DatosEjemploReportesFP.sql` | Seis órdenes ficticias para que los reportes tengan información en cualquier computadora. |

**Lo que el módulo reutiliza de los demás** (no lo duplica):

- `ClsConexionDA` (común): la conexión a `AutoManagerDMS`.
- `ClsZonaHorariaLN` (común): conversión entre UTC y la hora de Guatemala.
- `ClsValidacionExcepcionLN` (común): cómo la Capa Lógica avisa de un dato inválido.
- `ClsClienteLNSC` (módulo 1): la búsqueda de clientes del historial es la misma del módulo de clientes.
- `ClsFormatoSC` (módulo 1): formato de placas, fechas y DPI, para que se vean igual en todo el sitio.
- Las tablas `Clientes`, `Vehiculos` (módulo 1) y `OrdenesReparacion` (módulo 2, Neider).
- `Site.Master`: el menú muestra «Reportes» automáticamente porque existe `~/Reportes/Reportes.aspx`.

---

## 3. Cómo viaja un reporte por las capas

Ejemplo: alguien abre **Órdenes abiertas**.

```
Navegador ── GET Reportes.aspx?seccion=abiertas ──►  Presentación (Reportes.aspx.cs)
                                                     │ Page_Load lee ?seccion= → "abiertas"
                                                     │ SubCargarAbiertasFP()
                                                     ▼
                                                     Lógica (ClsReporteLNFP)
                                                     │ FncListarOrdenesAbiertasFP()
                                                     ▼
                                                     Datos (ClsReporteDAFP)
                                                     │ FncListarOrdenesAbiertasFP()
                                                     │ SELECT … JOIN … WHERE Estado = N'Abierta'
                                                     ▼
                                                     SQL Server (AutoManagerDMS)
                                                     │ devuelve las filas
                                                     ▼
                     Datos: convierte cada fila en un ClsReporteOrdenENFP (fechas marcadas como UTC)
                     Lógica: SubCalcularTiemposFP() calcula los días que lleva abierta cada una
                     Presentación: el Repeater dibuja la tabla; todo texto pasa por FncCodificarFP()
```

Cada capa solo conoce a la de abajo: la página nunca escribe SQL y la Capa de Datos nunca decide
qué es «este mes» ni cómo se ve una fecha.

---

## 4. Cada sección en detalle

### 4.1 Consulta base (Capa de Datos)

Las tres listas parten de la misma consulta, guardada en la constante `lStrConsultaBaseFP`:

```sql
SELECT o.IdOrden, o.IdVehiculo, v.Placa, v.VIN, v.Marca, v.Modelo,
       c.IdCliente, c.Nombres + N' ' + c.Apellidos AS Cliente,
       o.Estado, o.MotivoIngreso, o.FechaApertura, o.FechaCierre
FROM dbo.OrdenesReparacion AS o
INNER JOIN dbo.Vehiculos AS v ON v.IdVehiculo = o.IdVehiculo
INNER JOIN dbo.Clientes AS c ON c.IdCliente = v.IdCliente
```

- **`INNER JOIN` con `Vehiculos`**: cada orden pertenece a un vehículo (la llave foránea de la
  orden garantiza que el vehículo existe).
- **`INNER JOIN` con `Clientes`**: del vehículo se obtiene su dueño.
- Cada método agrega su propio `WHERE` y `ORDER BY`; así el SQL no se repite.

### 4.2 Resumen

Una sola consulta con cinco subconsultas (`FncObtenerResumenFP` en la Capa de Datos):

| Indicador | SQL | Nota |
|---|---|---|
| Órdenes abiertas | `COUNT(*) … WHERE Estado = N'Abierta'` | |
| Cerradas este mes | `COUNT(*) … WHERE Estado = N'Cerrada' AND FechaCierre >= @Desde AND FechaCierre < @Hasta` | `@Desde` y `@Hasta` son el inicio de este mes y del siguiente, **en hora de Guatemala convertida a UTC**. |
| Vehículos atendidos | `COUNT(DISTINCT IdVehiculo)` | Un vehículo con tres órdenes cuenta una vez. |
| Tiempo promedio | `AVG(DATEDIFF(SECOND, FechaApertura, FechaCierre))`, en minutos | Se mide en **segundos**: `DATEDIFF(MINUTE, …)` cuenta cambios de minuto del reloj, y una reparación de 30 segundos entre 17:46 y 17:47 contaría como 1 minuto. |
| Orden abierta más antigua | `MIN(FechaApertura) … WHERE Estado = N'Abierta'` | La Capa Lógica calcula cuántos días lleva. |

Si no hay órdenes cerradas, `AVG` devuelve `NULL` y la tarjeta dice «Sin datos».

### 4.3 Órdenes abiertas

- Orden: de la más antigua a la más reciente (`ORDER BY o.FechaApertura`), porque las más
  antiguas son las que necesitan atención.
- **Días abierta** (`FncCalcularDiasDesdeFP` en la Capa Lógica): se cuentan **días de calendario de
  Guatemala**. Una orden abierta hoy lleva «Hoy»; una abierta ayer, «1 día», aunque hayan pasado pocas horas.
- Las que llevan **7 días o más** se pintan en rojo (constante `lIntDiasAlertaFP` de la página).
- Cada fila enlaza a la orden (`DetalleOrdenNT.aspx`, módulo de Neider) y al cliente (`DetalleCliente.aspx`).

### 4.4 Órdenes cerradas

El filtro tiene dos fechas. Al pulsar **Filtrar**, la página no consulta directamente: redirige a
`?seccion=cerradas&desde=…&hasta=…`. Así el reporte tiene una dirección propia que se puede recargar,
compartir o imprimir con el mismo rango.

**Validaciones** (`FncListarOrdenesCerradasFP` y `FncLeerFechaFP` en la Capa Lógica):

| Caso | Mensaje |
|---|---|
| Sin fechas | No es error: se usa el mes actual (del día 1 a hoy). |
| Texto que no es una fecha | «La fecha inicial no es válida. Elígela en el calendario.» |
| Fecha anterior al año 2000 | «… debe ser del año 2000 en adelante.» |
| Fecha final antes de la inicial | «La fecha final no puede ser anterior a la fecha inicial.» |
| Rango de más de 366 días | «El rango puede abarcar como máximo 366 días.» |

Los errores se devuelven con `ClsValidacionExcepcionLN` y la página los muestra debajo de cada campo.

**¿Por qué se busca hasta el día siguiente?** Si el usuario pide «hasta el 22 de octubre», quiere
incluir todo ese día. Por eso la consulta usa `FechaCierre >= desde` y `FechaCierre < (22 de octubre + 1 día)`.
Un rango así (que incluye el inicio y excluye el final) evita problemas con las horas y los segundos.

**Duración** (`SubCalcularTiemposFP`): cierre menos apertura, redondeado a minutos. La página la escribe de
forma legible con `FncFormatearDuracionFP`: «Menos de 1 min», «56 min», «6 h 30 min», «1 día 9 h».

Los campos de fecha son `<input type="date">` de HTML: el navegador muestra un calendario y envía la fecha
como `año-mes-día`. Se usan inputs normales (y no un `TextBox` con `TextMode="Date"`) porque ese control no
se dibuja bien en Mono; así funciona igual en Windows y en macOS.

### 4.5 Historial por cliente

1. El usuario busca un cliente. Se usa la **misma búsqueda del módulo de clientes** (`ClsClienteLNSC.fncBuscarClientesSC`):
   por DPI, nombres, apellidos o teléfono, sin importar tildes ni mayúsculas.
2. Al elegirlo, la dirección cambia a `?seccion=historial&idCliente=N`.
3. Se verifica que el cliente exista (`fncObtenerClienteSC`). Si el número no es válido o no existe,
   la página responde **404** con un mensaje claro (igual que el detalle de clientes y vehículos).
4. `FncListarOrdenesPorClienteFP` trae las órdenes de **todos sus vehículos** (`WHERE v.IdCliente = @IdCliente`).
5. Cada orden muestra su estado y su tiempo: «Duró 6 h 30 min» si está cerrada o «Abierta hace 1 día» si sigue abierta.

Complementa al historial por VIN del módulo de órdenes: ese es por vehículo; este, por cliente.

---

## 5. Fechas y zona horaria

- La base guarda todas las fechas en **UTC** (`SYSUTCDATETIME()`), igual en Docker que en SQL Server Express.
- Guatemala está en **UTC−6** todo el año (no tiene horario de verano).
- Al **mostrar** una fecha se convierte a hora de Guatemala con `ClsFormatoSC.fncFormatearFechaSC`.
- Al **filtrar** («este mes», «del 1 al 22 de octubre») se toma la fecha de Guatemala y se convierte a UTC
  con `ClsZonaHorariaLN.fncConvertirGuatemalaAUtc` antes de enviarla a SQL.

Ejemplo: «cerradas desde el 1 de octubre» en Guatemala = desde el **1 de octubre a las 06:00 UTC**.
Si se usara la fecha sin convertir, se perderían o sobrarían órdenes cerradas entre medianoche y las 6 a. m.

---

## 6. Seguridad y errores

| Riesgo | Cómo se evita |
|---|---|
| Inyección SQL | Todas las consultas usan parámetros (`@Desde`, `@Hasta`, `@IdCliente`); nunca se concatena texto del usuario. |
| Modificar datos por error | El módulo solo tiene consultas `SELECT`. |
| Código malicioso en pantalla (XSS) | Todo texto que viene de la base o de la dirección se codifica con `FncCodificarFP` (`HttpUtility.HtmlEncode`) antes de mostrarse. |
| Datos inválidos en la dirección | `?seccion=` desconocido muestra el resumen; fechas inválidas muestran un mensaje; `?idCliente=` inválido responde 404. |
| `ValidateRequest="false"` | Está desactivado en esta página (como en las de clientes y vehículos) porque la Capa Lógica valida y todo se codifica; así un texto con `<` recibe un mensaje claro y no un error del servidor. |
| Base de datos caída | Cada sección captura el error, lo registra para el equipo (`ClsRegistroErrores`) y muestra un mensaje general, sin detalles técnicos. |

---

## 7. Convenciones de nombres

Se siguen las del documento del equipo, con las iniciales **FP**:

| Elemento | Regla | Ejemplos |
|---|---|---|
| Clases | `Cls` + nombre + capa (`EN`, `DA`, `LN`) + iniciales | `ClsReporteOrdenENFP`, `ClsReporteDAFP`, `ClsReporteLNFP` |
| Funciones (devuelven un valor) | `Fnc…FP` | `FncListarOrdenesAbiertasFP`, `FncFormatearDuracionFP` |
| Procedimientos (no devuelven nada) | `Sub…FP` | `SubCargarResumenFP`, `SubCalcularTiemposFP` |
| Variables locales | `l` + tipo + nombre + iniciales | `lStrSeccionFP`, `lIntIdClienteFP`, `lLstOrdenesFP` |
| Parámetros | `p` + tipo + nombre + iniciales | `pStrDesdeFP`, `pIntIdClienteFP` |
| Campos de la clase | `g` + tipo + nombre + iniciales | `gObjReporteLNFP`, `gStrDesdeFP` |
| Controles | prefijo del control + nombre + iniciales | `pnlResumenFP`, `rptAbiertasFP`, `btnFiltrarFP`, `litRangoFP` |

Tipos usados: `Str` (texto), `Int` (entero), `Bln` (verdadero/falso), `Obj` (objeto), `Dat` (fecha),
`Lst` (lista), `Dic` (diccionario).

---

## 8. Datos de ejemplo

`database/91_DatosEjemploReportesFP.sql` agrega seis órdenes ficticias sobre los vehículos de ejemplo:
tres cerradas (una en septiembre de 2026 y dos en octubre, para probar el filtro) y tres abiertas
(abiertas el 12, 20 y 21 de octubre de 2026, para ver los días y el resaltado en rojo).

- Las fechas son fijas. Por eso, con el paso del tiempo, «Cerradas este mes» puede quedar en 0 y los días de
  las abiertas siguen aumentando. Es lo correcto: para ver las cerradas de ejemplo, filtra por septiembre u octubre de 2026.
- Busca cada vehículo por su **VIN**; no usa números de identificación fijos.
- Se puede ejecutar varias veces: no duplica órdenes.
- Va después de `01`, `02` y `90`. En macOS lo ejecuta Docker; en Windows, junto con los demás scripts numerados.

---

## 9. Cómo probarlo

1. Abre **Reportes** en el menú.
2. **Resumen**: compara las tarjetas con lo que ves en las otras pestañas.
3. **Órdenes abiertas**: las que llevan 7 días o más aparecen en rojo (con los datos de ejemplo, al menos
   la del Honda Accord P 789GHI).
4. **Órdenes cerradas**: sin fechas, muestra el mes actual. Prueba del 1 al 30 de septiembre de 2026: debe
   aparecer solo la orden del Honda Accord P 789GHI, que duró «2 días 7 h». Luego pon una fecha final anterior
   a la inicial: debe aparecer el mensaje debajo del campo.
5. **Historial por cliente**: busca «perez» y elige a Ana María Pérez López; deben aparecer sus 2 órdenes.
6. Pulsa **Imprimir** en cualquier sección.

Pruebas automáticas realizadas al construirlo: **38 comprobaciones**. Los números del resumen se compararon
con consultas SQL independientes. También se probaron el orden de las listas, las duraciones, las cinco
validaciones de fechas, la búsqueda y el historial, el 404, la codificación de textos peligrosos y los
mensajes con la base de datos detenida.

---

## 10. Preguntas que te pueden hacer

**¿Por qué los reportes no guardan nada?**
Porque un reporte solo presenta información; si escribiera datos podría alterar lo que reporta. Todas las consultas son `SELECT`.

**¿Por qué usaste `INNER JOIN` y no `LEFT JOIN`?**
Toda orden tiene vehículo y todo vehículo tiene cliente: lo garantizan las llaves foráneas. Con `INNER JOIN` no hay filas incompletas.

**¿Cómo evitas la inyección SQL?**
Con parámetros (`SqlCommand.Parameters`). El texto del usuario nunca forma parte del SQL; viaja como dato.

**¿Por qué la lógica de fechas no está en la página?**
Porque es una regla del negocio («este mes», «una semana o más»). Va en la Capa Lógica para que la página solo muestre y la Capa de Datos solo consulte.

**¿Por qué las fechas se guardan en UTC?**
Para que el resultado no dependa del servidor: SQL Server en Docker está en UTC y en Windows en hora local. Guardando siempre UTC, todos ven las mismas horas, y se convierten a Guatemala solo al mostrarlas.

**¿Por qué el rango de fechas termina en «día siguiente»?**
Para incluir el último día completo sin depender de horas ni segundos (ver la sección 4.4).

**¿Por qué el promedio se calcula en segundos?**
Porque `DATEDIFF(MINUTE, …)` cuenta cambios de minuto del reloj, no minutos reales (ver la sección 4.2).

**¿Qué pasa si escribo `<script>` en el buscador?**
Se busca como texto y se muestra codificado; el navegador no lo ejecuta.

**¿Por qué no hay reportes de dinero?**
El alcance acordado por el equipo excluye precios, costos y facturación.

**¿Y los repuestos?**
Dependen del módulo de inventario. Cuando esté integrado se agregan dos secciones (existencias y repuestos utilizados) con el mismo patrón.

---

## 11. Pendiente

- Completar tu nombre y carné en los encabezados (`PENDIENTE`).
- Reportes de inventario: existencias de repuestos y repuestos utilizados por orden, cuando el módulo de
  Pavel esté integrado (necesitan sus tablas y la relación entre repuestos y órdenes).
