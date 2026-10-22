# Módulo 4: Reportes

Este documento describe el módulo de reportes de AutoManager DMS: su funcionalidad, su implementación
en las cuatro capas, las decisiones técnicas adoptadas y el procedimiento de prueba.

---

## 1. Funcionalidad

El módulo presenta información del taller sin modificarla: únicamente ejecuta consultas sobre la base
de datos. La página Reportes se organiza en seis secciones, accesibles mediante pestañas:

| Sección | Contenido | Dirección |
|---|---|---|
| Resumen | Indicadores generales del taller: órdenes abiertas, órdenes cerradas en el mes, vehículos atendidos, tiempo promedio de reparación y orden abierta más antigua. | `Reportes.aspx` |
| Órdenes abiertas | Órdenes pendientes, de la más antigua a la más reciente, con los días que cada una lleva abierta. Las que alcanzan una semana o más se resaltan en rojo. | `Reportes.aspx?seccion=abiertas` |
| Órdenes cerradas | Órdenes cerradas dentro de un rango de fechas de cierre, con la duración de cada reparación. | `Reportes.aspx?seccion=cerradas&desde=2026-10-01&hasta=2026-10-22` |
| Historial por cliente | Todas las órdenes registradas para los vehículos de un cliente. | `Reportes.aspx?seccion=historial&idCliente=1` |
| Existencias | Stock de cada repuesto del inventario, su nivel (agotado, stock bajo o disponible) y las unidades usadas en órdenes. Primero los que tienen menos unidades. | `Reportes.aspx?seccion=existencias` |
| Repuestos utilizados | Unidades de cada repuesto utilizadas en órdenes dentro de un rango de fechas. Primero los más utilizados. | `Reportes.aspx?seccion=repuestos&desde=2026-10-01&hasta=2026-10-22` |

El botón Imprimir produce una versión del reporte sin menú, botones ni filtros. Esa versión incluye
todas las columnas, también las que en pantalla se ocultan por falta de espacio.

Fuera del alcance acordado por el equipo quedan los precios, los costos y cualquier monto en dinero.

---

## 2. Estructura de archivos

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Entidades | `CapaEntidades/ClsReporteOrdenENFP.cs` | Representa una fila de reporte: datos de la orden, de su vehículo y de su cliente, más los tiempos calculados (días abierta y minutos de reparación). |
| Entidades | `CapaEntidades/ClsResumenReportesENFP.cs` | Contiene los indicadores del resumen. |
| Entidades | `CapaEntidades/ClsExistenciaRepuestoENFP.cs` | Una fila del reporte de existencias: repuesto, stock, unidades usadas y nivel de stock. |
| Entidades | `CapaEntidades/ClsRepuestoUtilizadoENFP.cs` | Una fila del reporte de repuestos utilizados: unidades, órdenes, último uso y stock actual. |
| Datos | `CapaDatos/ClsReporteDAFP.cs` | Contiene las consultas SQL (solo `SELECT`, todas con parámetros). |
| Lógica | `CapaLogica/ClsReporteLNFP.cs` | Aplica las reglas del negocio: rangos de fechas según el calendario de Guatemala, validación de los filtros, cálculo de tiempos y nivel de stock. |
| Presentación | `CapaPresentacion/Reportes/Reportes.aspx` | Define la interfaz: pestañas, tarjetas y tablas. |
| Presentación | `CapaPresentacion/Reportes/Reportes.aspx.cs` | Interpreta los parámetros de la dirección (`?seccion=`, `?desde=`, etc.), invoca a la Capa Lógica y llena las tablas. |
| Estilos | `CapaPresentacion/Content/autodms.css` (secciones «Reportes» e «Impresión») | Estilos de las pestañas, las etiquetas de estado, los días resaltados y la versión de impresión. |
| Datos de ejemplo | `database/91_DatosEjemploReportesFP.sql` | Inserta seis órdenes ficticias para que los reportes muestren información en cualquier instalación. |

Componentes de otros módulos que el reporte reutiliza en lugar de duplicarlos:

- `ClsConexionDASC` (común): conexión a `AutoManagerDMS`.
- `ClsZonaHorariaLNSC` (común): conversión entre UTC y la hora de Guatemala.
- `ClsValidacionExcepcionLNSC` (común): mecanismo con el que la Capa Lógica informa de un dato inválido.
- `ClsClienteLNSC` (módulo 1): la búsqueda de clientes del historial es la misma del módulo de clientes.
- `ClsFormatoSC` (módulo 1): formato de placas, fechas y DPI, para que se presenten igual en todo el sitio.
- Las tablas `Clientes` y `Vehiculos` (módulo 1), `OrdenesReparacion` (módulo 2, Neider), `Repuesto`
  (módulo 3, Pavel) y `RepuestosOrden` (repuestos utilizados en órdenes, Sharon).
- `Site.Master`: el menú muestra la opción «Reportes» porque existe la página `~/Reportes/Reportes.aspx`.

---

## 3. Flujo de una solicitud entre capas

Ejemplo: solicitud de la sección Órdenes abiertas.

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

Cada capa se comunica solo con la capa inmediata inferior. La página no contiene SQL, y la Capa de
Datos no decide qué significa «este mes» ni cómo se presenta una fecha.

---

## 4. Detalle de las secciones

### 4.1 Consulta base (Capa de Datos)

Las tres listas parten de la misma consulta, definida en la constante `gStrConsultaBaseFP`:

```sql
SELECT o.IdOrden, o.IdVehiculo, v.Placa, v.VIN, v.Marca, v.Modelo,
       c.IdCliente, c.Nombres + N' ' + c.Apellidos AS Cliente,
       o.Estado, o.MotivoIngreso, o.FechaApertura, o.FechaCierre
FROM dbo.OrdenesReparacion AS o
INNER JOIN dbo.Vehiculos AS v ON v.IdVehiculo = o.IdVehiculo
INNER JOIN dbo.Clientes AS c ON c.IdCliente = v.IdCliente
```

- `INNER JOIN` con `Vehiculos`: cada orden pertenece a un vehículo, y la llave foránea de la orden
  garantiza que ese vehículo existe.
- `INNER JOIN` con `Clientes`: a partir del vehículo se obtiene su propietario.
- Cada método agrega su propia cláusula `WHERE` y su `ORDER BY`, de modo que el SQL no se repite.

### 4.2 Resumen

Los indicadores se obtienen con una sola consulta formada por cinco subconsultas
(`FncObtenerResumenFP` en la Capa de Datos):

| Indicador | SQL | Observación |
|---|---|---|
| Órdenes abiertas | `COUNT(*) … WHERE Estado = N'Abierta'` | |
| Cerradas este mes | `COUNT(*) … WHERE Estado = N'Cerrada' AND FechaCierre >= @Desde AND FechaCierre < @Hasta` | `@Desde` y `@Hasta` corresponden al inicio del mes actual y al del siguiente, en hora de Guatemala convertida a UTC. |
| Vehículos atendidos | `COUNT(DISTINCT IdVehiculo)` | Un vehículo con tres órdenes se cuenta una sola vez. |
| Tiempo promedio | `AVG(DATEDIFF(SECOND, FechaApertura, FechaCierre))`, expresado en minutos | Se calcula en segundos porque `DATEDIFF(MINUTE, …)` cuenta cambios de minuto del reloj: una reparación de 30 segundos entre las 17:46 y las 17:47 contaría como 1 minuto. |
| Orden abierta más antigua | `MIN(FechaApertura) … WHERE Estado = N'Abierta'` | La Capa Lógica calcula los días transcurridos. |

Si no existen órdenes cerradas, `AVG` devuelve `NULL` y la tarjeta muestra «Sin datos».

### 4.3 Órdenes abiertas

- Ordenamiento: de la más antigua a la más reciente (`ORDER BY o.FechaApertura`), porque las órdenes
  más antiguas son las que requieren atención primero.
- Días abierta (`FncCalcularDiasDesdeFP` en la Capa Lógica): se cuentan días de calendario de Guatemala.
  Una orden abierta el mismo día muestra «Hoy»; una abierta el día anterior muestra «1 día», aunque
  hayan transcurrido pocas horas.
- Las órdenes con 7 días o más se resaltan en rojo (constante `gIntDiasAlertaFP` de la página).
- Cada fila enlaza con el detalle de la orden (`DetalleOrdenNT.aspx`, módulo de Neider) y con el del
  cliente (`DetalleCliente.aspx`).

### 4.4 Órdenes cerradas

El filtro recibe dos fechas. Al pulsar Filtrar, la página redirige a `?seccion=cerradas&desde=…&hasta=…`
en lugar de consultar en el mismo envío. Así cada reporte tiene una dirección propia que puede
recargarse, compartirse o imprimirse con el mismo rango.

Validaciones (`SubLeerRangoFP` y `FncLeerFechaFP` en la Capa Lógica; las comparte la sección 4.7):

| Caso | Resultado |
|---|---|
| Sin fechas | No se considera un error: se usa el mes actual, desde el día 1 hasta la fecha de hoy. |
| Texto que no es una fecha | «La fecha inicial no es válida. Elígela en el calendario.» |
| Fecha anterior al año 2000 | «… debe ser del año 2000 en adelante.» |
| Fecha final anterior a la inicial | «La fecha final no puede ser anterior a la fecha inicial.» |
| Rango de más de 366 días | «El rango puede abarcar como máximo 366 días.» |

Los errores se notifican mediante `ClsValidacionExcepcionLNSC`, y la página los muestra debajo del campo
correspondiente.

Límite superior del rango: una consulta «hasta el 22 de octubre» debe incluir ese día completo. Por
eso la condición es `FechaCierre >= desde` y `FechaCierre < (22 de octubre + 1 día)`. Un intervalo que
incluye el inicio y excluye el final evita errores con las horas y los segundos.

Duración (`SubCalcularTiemposFP`): diferencia entre el cierre y la apertura, redondeada a minutos. La
página la presenta en formato legible mediante `FncFormatearDuracionFP`: «Menos de 1 min», «56 min»,
«6 h 30 min», «1 día 9 h».

Los campos de fecha son elementos `<input type="date">` de HTML: el navegador muestra un calendario y
envía la fecha con el formato `año-mes-día`. Se usan elementos HTML estándar en lugar de un `TextBox`
con `TextMode="Date"` porque ese control no se representa correctamente en Mono; de esta forma el
comportamiento es el mismo en Windows y en macOS.

### 4.5 Historial por cliente

1. El cliente se localiza con la misma búsqueda del módulo de clientes
   (`ClsClienteLNSC.FncBuscarClientesSC`): por DPI, nombres, apellidos o teléfono, sin distinguir
   tildes ni mayúsculas.
2. Al seleccionarlo, la dirección cambia a `?seccion=historial&idCliente=N`.
3. Se verifica que el cliente exista (`FncObtenerClienteSC`). Si el número no es válido o no
   corresponde a ningún cliente, la página responde con el código 404 y un mensaje descriptivo, igual
   que el detalle de clientes y de vehículos.
4. `FncListarOrdenesPorClienteFP` obtiene las órdenes de todos los vehículos del cliente
   (`WHERE v.IdCliente = @IdCliente`).
5. Cada orden muestra su estado y su tiempo: «Duró 6 h 30 min» si está cerrada, o «Abierta hace 1 día»
   si continúa abierta.

Esta sección complementa el historial por VIN del módulo de órdenes: aquel se consulta por vehículo,
y este, por cliente.

### 4.6 Existencias

Consulta (`FncListarExistenciasFP` en la Capa de Datos):

```sql
SELECT r.idrepuesto, r.codigo, r.nombre, r.marcacarro, r.modelocarro, r.aniocarro, r.stock,
       COALESCE(SUM(ro.Cantidad), 0) AS UnidadesUsadas
FROM dbo.Repuesto AS r
LEFT JOIN dbo.RepuestosOrden AS ro ON ro.IdRepuesto = r.idrepuesto
GROUP BY r.idrepuesto, r.codigo, r.nombre, r.marcacarro, r.modelocarro, r.aniocarro, r.stock
ORDER BY r.stock, r.nombre, r.codigo;
```

- `LEFT JOIN`: un repuesto que nunca se ha usado también aparece; `SUM` devuelve `NULL` para él y
  `COALESCE` lo convierte en 0.
- `GROUP BY`: una fila por repuesto, con la suma de todas sus unidades usadas.
- Orden: primero los que tienen menos unidades, que son los que requieren compra.

El nivel de stock es una regla del negocio y lo calcula la Capa Lógica (`FncObtenerNivelStockFP`):

| Unidades en bodega | Nivel |
|---|---|
| 0 | Agotado (rojo) |
| De 1 a 3 (constante `gIntStockBajoFP`) | Stock bajo (amarillo) |
| Más de 3 | Disponible (verde) |

El encabezado resume cuántos repuestos están agotados y cuántos tienen stock bajo. Cada fila enlaza al
inventario (módulo de Pavel) filtrado por el código del repuesto.

### 4.7 Repuestos utilizados

Consulta (`FncListarRepuestosUtilizadosFP` en la Capa de Datos):

```sql
SELECT r.idrepuesto, r.codigo, r.nombre,
       SUM(ro.Cantidad) AS Unidades,
       COUNT(DISTINCT ro.IdOrden) AS Ordenes,
       MAX(ro.FechaRegistro) AS UltimoUso,
       r.stock
FROM dbo.RepuestosOrden AS ro
INNER JOIN dbo.Repuesto AS r ON r.idrepuesto = ro.IdRepuesto
WHERE ro.FechaRegistro >= @Desde AND ro.FechaRegistro < @Hasta
GROUP BY r.idrepuesto, r.codigo, r.nombre, r.stock
ORDER BY Unidades DESC, r.nombre;
```

- `SUM(ro.Cantidad)`: unidades utilizadas del repuesto en el rango.
- `COUNT(DISTINCT ro.IdOrden)`: en cuántas órdenes distintas se usó.
- `MAX(ro.FechaRegistro)`: último uso dentro del rango.
- Se filtra por la fecha en que se registró el uso, con el mismo rango que las órdenes cerradas: incluye
  el día inicial y todo el día final.

Esta sección comparte con «Órdenes cerradas» el filtro de fechas y sus validaciones: la Capa Lógica usa
un solo método (`SubLeerRangoFP`) para ambas, de modo que las reglas y los mensajes son idénticos. El
botón «Filtrar» vuelve a la sección en la que está el usuario y el campo inicial cambia su etiqueta
(«Cerradas desde» o «Utilizados desde»).

---

## 5. Fechas y zona horaria

- La base de datos almacena todas las fechas en UTC (`SYSUTCDATETIME()`), tanto en Docker como en
  SQL Server Express.
- Guatemala está en UTC-6 durante todo el año, ya que no aplica horario de verano.
- Para mostrar una fecha, esta se convierte a la hora de Guatemala con `ClsFormatoSC.FncFormatearFechaSC`.
- Para filtrar («este mes», «del 1 al 22 de octubre»), la fecha de Guatemala se convierte a UTC con
  `ClsZonaHorariaLNSC.FncConvertirGuatemalaAUtcSC` antes de enviarse a SQL Server.

Ejemplo: el filtro «cerradas desde el 1 de octubre» en hora de Guatemala equivale a «desde el 1 de
octubre a las 06:00 UTC». Sin esta conversión, el reporte omitiría o incluiría por error las órdenes
cerradas entre la medianoche y las 6:00.

---

## 6. Seguridad y manejo de errores

| Riesgo | Medida |
|---|---|
| Inyección SQL | Todas las consultas usan parámetros (`@Desde`, `@Hasta`, `@IdCliente`); el texto del usuario nunca se concatena al SQL. |
| Modificación accidental de datos | El módulo contiene únicamente consultas `SELECT`. |
| Código malicioso en pantalla (XSS) | Todo texto que proviene de la base de datos o de la dirección se codifica con `FncCodificarFP` (`HttpUtility.HtmlEncode`) antes de mostrarse. |
| Datos inválidos en la dirección | Un valor desconocido en `?seccion=` muestra el resumen; las fechas inválidas generan un mensaje; un `?idCliente=` inválido responde con 404. |
| `ValidateRequest="false"` | Está desactivado en esta página, al igual que en las de clientes y vehículos, porque la Capa Lógica valida las entradas y toda salida se codifica. Así, un texto con `<` recibe un mensaje de validación en lugar de un error del servidor. |
| Base de datos no disponible | Cada sección captura el error, lo registra para su revisión por el equipo (`ClsRegistroErroresSC`) y muestra un mensaje general, sin detalles técnicos. |

---

## 7. Convenciones de nombres

El módulo aplica las convenciones del documento del equipo, con las iniciales FP:

| Elemento | Regla | Ejemplos |
|---|---|---|
| Clases | `Cls` + nombre + capa (`EN`, `DA`, `LN`) + iniciales | `ClsReporteOrdenENFP`, `ClsExistenciaRepuestoENFP`, `ClsReporteDAFP`, `ClsReporteLNFP` |
| Funciones (devuelven un valor) | `Fnc…FP` | `FncListarOrdenesAbiertasFP`, `FncFormatearDuracionFP` |
| Procedimientos (no devuelven valor) | `Sub…FP` | `SubCargarResumenFP`, `SubCalcularTiemposFP` |
| Variables locales | `l` + tipo + nombre + iniciales | `lStrSeccionFP`, `lIntIdClienteFP`, `lLstOrdenesFP` |
| Parámetros | `p` + tipo + nombre + iniciales | `pStrDesdeFP`, `pIntIdClienteFP` |
| Campos, constantes y propiedades de la clase | `g` + tipo + nombre + iniciales | `gObjReporteLNFP`, `gIntStockBajoFP`, `gIntIdOrdenFP` |
| Controles | prefijo del control + nombre + iniciales | `pnlResumenFP`, `rptAbiertasFP`, `btnFiltrarFP`, `litRangoFP` |

Prefijos de tipo utilizados: `Str` (texto), `Int` (entero), `Bln` (verdadero/falso), `Obj` (objeto),
`Dat` (fecha), `Lst` (lista) y `Dic` (diccionario).

---

## 8. Datos de ejemplo

El script `database/91_DatosEjemploReportesFP.sql` inserta seis órdenes ficticias sobre los vehículos
de ejemplo: tres cerradas (una en septiembre de 2026 y dos en octubre, para probar el filtro) y tres
abiertas (del 12, 20 y 21 de octubre de 2026, para comprobar el conteo de días y el resaltado en rojo).

- Las fechas son fijas. Con el paso del tiempo, el indicador «Cerradas este mes» puede quedar en 0 y
  los días de las órdenes abiertas siguen aumentando; ese es el comportamiento esperado. Las órdenes
  cerradas de ejemplo se consultan filtrando por septiembre u octubre de 2026.
- El script localiza cada vehículo por su VIN y no depende de identificadores fijos.
- Puede ejecutarse varias veces sin duplicar órdenes.
- Se ejecuta después de `01`, `02` y `90`. En macOS lo ejecuta Docker; en Windows, junto con los demás
  scripts numerados.

Las secciones de repuestos usan los datos de ejemplo de los otros módulos: ocho repuestos
(`92_DatosEjemploInventarioPP.sql`, uno agotado y dos con stock bajo) y cinco usos en las órdenes de
ejemplo (`93_DatosEjemploRepuestosOrdenSC.sql`): uno en septiembre y cuatro en octubre de 2026.

---

## 9. Procedimiento de prueba

1. Abrir la opción Reportes del menú.
2. Resumen: comparar los valores de las tarjetas con la información de las demás pestañas.
3. Órdenes abiertas: verificar que las órdenes con 7 días o más aparezcan en rojo (con los datos de
   ejemplo, al menos la del Honda Accord P 789GHI).
4. Órdenes cerradas: sin fechas, el reporte muestra el mes actual. Con el rango del 1 al 30 de
   septiembre de 2026 debe aparecer únicamente la orden del Honda Accord P 789GHI, con una duración de
   «2 días 7 h». Con una fecha final anterior a la inicial debe aparecer el mensaje debajo del campo.
5. Historial por cliente: buscar «perez» y seleccionar a Ana María Pérez López; deben mostrarse sus
   2 órdenes.
6. Existencias: el primer repuesto debe ser «Bujes de suspensión delantera» con el nivel «Agotado», y el
   encabezado debe indicar «1 agotado y 2 con stock bajo» (con los datos de ejemplo sin cambios).
7. Repuestos utilizados: con el rango del 1 al 31 de octubre de 2026 debe aparecer primero el aceite de
   motor (4 unidades) y un total de 7 unidades; con septiembre, solo la batería.
8. Pulsar Imprimir en cualquier sección.

Pruebas automatizadas: 38 comprobaciones de las cuatro primeras secciones. Los valores del resumen se
contrastaron con consultas SQL independientes. También se verificaron el orden de las listas, las
duraciones, las cinco validaciones de fechas, la búsqueda y el historial, la respuesta 404, la
codificación de textos con código HTML y los mensajes con la base de datos detenida. Las secciones de
repuestos forman parte de otras 67 comprobaciones junto con el inventario: las existencias se contrastaron
con una consulta SQL independiente y se verificaron los totales por rango, el filtro compartido y los
mensajes con la base de datos detenida.

---

## 10. Justificación de las decisiones técnicas

**¿Por qué los reportes no modifican datos?**
Un reporte solo presenta información; si escribiera en la base de datos podría alterar lo que reporta.
Todas las consultas son `SELECT`.

**¿Por qué se usa `INNER JOIN` y no `LEFT JOIN`?**
Las llaves foráneas garantizan que toda orden tiene un vehículo y que todo vehículo tiene un cliente.
Con `INNER JOIN` no se producen filas incompletas.

**¿Cómo se previene la inyección SQL?**
Con parámetros (`SqlCommand.Parameters`). El texto del usuario no forma parte de la instrucción SQL;
se envía como dato.

**¿Por qué la lógica de fechas no está en la página?**
Porque corresponde a reglas del negocio («este mes», «una semana o más»). Se ubica en la Capa Lógica
para que la Capa de Presentación solo muestre la información y la Capa de Datos solo consulte.

**¿Por qué las fechas se almacenan en UTC?**
Para que el resultado no dependa del servidor: SQL Server en Docker usa UTC y en Windows usa la hora
local. Al almacenar siempre en UTC, todas las instalaciones muestran las mismas horas, que se convierten
a la hora de Guatemala únicamente al presentarlas.

**¿Por qué el rango de fechas termina en el día siguiente?**
Para incluir completo el último día sin depender de horas ni segundos (sección 4.4).

**¿Por qué el promedio se calcula en segundos?**
Porque `DATEDIFF(MINUTE, …)` cuenta cambios de minuto del reloj, no minutos transcurridos (sección 4.2).

**¿Qué ocurre si se ingresa `<script>` en el buscador?**
Se busca como texto y se muestra codificado; el navegador no lo ejecuta.

**¿Por qué no hay reportes de montos?**
El alcance acordado por el equipo excluye precios, costos y facturación.

**¿Por qué el reporte de existencias usa `LEFT JOIN` y el de repuestos utilizados `INNER JOIN`?**
Las existencias deben mostrar todos los repuestos, incluso los que nunca se han usado; con `INNER JOIN`
desaparecerían. El reporte de repuestos utilizados, en cambio, solo debe mostrar los que tienen usos en el
rango.

**¿Por qué el nivel de stock se calcula en la Capa Lógica y no en SQL?**
Es una regla del negocio (qué se considera «stock bajo») que puede cambiar; vive en una sola constante
(`gIntStockBajoFP`) y la consulta solo entrega las cantidades.

**¿Por qué los dos filtros de fechas usan el mismo método?**
Para que las reglas y los mensajes sean idénticos en ambos reportes y cualquier corrección se haga en un
solo lugar (`SubLeerRangoFP`).

---

## 11. Pendientes

- Completar el nombre completo y el carné de la integrante en el encabezado de cada archivo del módulo
  (donde dice `PENDIENTE`) y en `database/91_DatosEjemploReportesFP.sql`.
