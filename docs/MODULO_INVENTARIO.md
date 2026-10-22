# Módulo 3: Inventario de repuestos

Este documento describe el módulo de inventario de AutoManager DMS: su funcionalidad, su implementación
en las cuatro capas, la correspondencia con la versión original del módulo, los ajustes realizados para
integrarlo y el procedimiento de prueba.

---

## 1. Funcionalidad

El módulo administra los repuestos del taller y sus unidades en bodega.

| Función | Descripción | Dirección |
|---|---|---|
| Consultar el inventario | Lista de repuestos, del registrado más recientemente al más antiguo, con el vehículo compatible y el stock. | `Inventario/Inventario.aspx` |
| Buscar | Por parte del nombre (sin distinguir tildes ni mayúsculas) o por el código completo. | `Inventario/Inventario.aspx?q=filtro` |
| Registrar un repuesto | Código, nombre, descripción, stock inicial y vehículo compatible (marca, modelo y año). | `Inventario/RegistroRepuesto.aspx` |
| Registrar una entrada a bodega | Suma unidades al stock de un repuesto. Se abre en una ventana sobre el listado. | `Inventario/Inventario.aspx?entrada=5` |

Las salidas de bodega no se registran en este módulo: el stock se descuenta automáticamente cuando un
repuesto se registra como utilizado en una orden de reparación (sección 7).

---

## 2. Estructura de archivos

| Capa | Archivo | Responsabilidad |
|---|---|---|
| Entidades | `CapaEntidades/ClsRepuestoENPP.cs` | Representa un repuesto y el vehículo con el que es compatible. |
| Datos | `CapaDatos/ClsRepuestoDAPP.cs` | Consultas SQL de `dbo.Repuesto`, todas con parámetros. |
| Lógica | `CapaLogica/ClsRepuestoLNPP.cs` | Normaliza y valida los datos, detecta códigos repetidos y valida las entradas de stock. |
| Presentación | `CapaPresentacion/Inventario/Inventario.aspx` (+ `.cs`) | Listado, búsqueda y ventana de entrada a bodega. |
| Presentación | `CapaPresentacion/Inventario/RegistroRepuesto.aspx` (+ `.cs`) | Formulario de registro de un repuesto. |
| Estilos | `CapaPresentacion/Content/autodms.css` (sección «Inventario») | Stock agotado en rojo y ventana de entrada. |
| Base de datos | `database/03_InventarioPP.sql` | Crea la tabla `dbo.Repuesto` o actualiza la que ya exista. |
| Datos de ejemplo | `database/92_DatosEjemploInventarioPP.sql` | Ocho repuestos ficticios compatibles con los vehículos de ejemplo. |

Componentes compartidos que el módulo reutiliza:

- `ClsConexionDA`: conexión a `AutoManagerDMS` a partir de `ConnectionStrings.config`.
- `ClsValidacionExcepcionLN`: mecanismo con el que la Capa Lógica informa los errores de cada campo.
- `ClsDatoDuplicadoExcepcionDA`: traduce el error de SQL Server por código repetido.
- `ClsSqlDA.fncEscaparPatronLike`: hace que `%`, `_` y `[` se busquen como texto.
- `ClsVehiculoLNSC` (módulo 1): regla del año de un vehículo (de 1981 al año siguiente al actual).
- `Site.Master`, `ClsRegistroErrores` y `autodms.css`: estructura, registro de errores y estilos comunes.

---

## 3. Correspondencia con la versión original

La versión original del módulo (rama `Pavel`) era una solución independiente. Al integrarla, cada parte
pasó a las capas comunes del proyecto. Esta tabla permite ubicar cada elemento original:

| Versión original | Versión integrada |
|---|---|
| `ClsCapaEntidades` | `ClsRepuestoENPP` |
| Propiedades `gstrcodrepuestoPP`, `gstrnomrepuestoPP`, `gintstockrepuestoPP`… | `lStrCodigoPP`, `lStrNombrePP`, `lIntStockPP`… (convención del proyecto para propiedades) |
| `Clsconexion.fncobtenerconexionPP` | `ClsConexionDA.fncObtenerConexion` (conexión compartida) |
| `Clsconexion.fncagregarepuestoPP` | `ClsRepuestoDAPP.FncAgregarRepuestoPP` |
| `Clsconexion.fncconsultarPP` | `ClsRepuestoDAPP.FncConsultarPP` |
| `Clsconexion.fncactualizarstockPP` | `ClsRepuestoDAPP.FncActualizarStockPP` |
| `ClslogicaNegocios.fncRegistroRepuestoPP` | `ClsRepuestoLNPP.FncRegistrarRepuestoPP` |
| `ClslogicaNegocios.fncconsultarbdPP` | `ClsRepuestoLNPP.FncConsultarRepuestosPP` |
| `ClslogicaNegocios.fncactualizastockPP` | `ClsRepuestoLNPP.FncRegistrarEntradaPP` |
| `Inventario.aspx`, `RegistroRepuesto.aspx` | Mismas páginas en `CapaPresentacion/Inventario/`, con la página maestra del sitio |
| `database/Inventario.sql` | `database/03_InventarioPP.sql` |

Se conservan las decisiones de diseño originales: la tabla `dbo.Repuesto` y sus columnas, la regla de stock
no negativo, la búsqueda por nombre o código exacto, la suma de unidades al stock, la ventana para actualizar
el stock y las dos secciones del formulario (información del repuesto y compatibilidad con el automóvil).

---

## 4. Ajustes de integración

| Situación en la versión original | Ajuste | Motivo |
|---|---|---|
| El `INSERT` se armaba con `string.Format` y el texto del usuario. | Todas las consultas usan parámetros. | Un apóstrofo (por ejemplo, en «D'Angelo») rompía la instrucción y permitía inyección SQL. |
| Cadena de conexión fija a `DESKTOP-FEI37DB\SQLEXPRESS`. | Se usa `ClsConexionDA`. | La conexión depende de la configuración de cada computadora, no del código. |
| La Capa Lógica devolvía textos «Éxito:»/«Error:» y las páginas buscaban `"exito"` y `"Exito"`. | Los errores se informan con `ClsValidacionExcepcionLN`; tras guardar, la página redirige con un aviso. | La comparación nunca coincidía con «Éxito» (tilde y mayúscula): el formulario no se limpiaba y la tabla no se actualizaba. |
| La ventana decía «Nuevo stock», pero el SQL sumaba la cantidad. | El campo se llama «Unidades que ingresan». | El texto ahora describe lo que hace la instrucción (`stock = stock + @Cantidad`). |
| `int.Parse` sobre el texto escrito. | Validación de números con mensajes por campo. | Escribir letras producía un error técnico en pantalla. |
| Los datos se mostraban sin codificar. | Todo texto se codifica con `HttpUtility.HtmlEncode`. | Evita que un dato guardado ejecute código en el navegador (XSS). |
| Validaciones solo de código, nombre y stock. | Se validan todos los campos: longitud, formato, stock y año. | La base rechazaba textos largos con un error técnico y aceptaba el año 0. |
| La lista decía «ordenados del más reciente», pero la consulta no ordenaba. | `ORDER BY idrepuesto DESC`. | El texto ahora es cierto. |
| La búsqueda distinguía tildes y trataba `%` como comodín. | `COLLATE Latin1_General_CI_AI` y escape de comodines. | «bateria» encuentra «Batería»; `%` se busca como texto. |
| Páginas con menú, estilos y Bootstrap propios. | Página maestra `Site.Master` y `autodms.css`. | Mismo diseño y menú en todo el sitio. |
| La ventana se abría con JavaScript que copiaba datos a campos ocultos. | La ventana la dibuja el servidor cuando la dirección trae `?entrada=`. | Funciona sin JavaScript propio y el repuesto se vuelve a comprobar en el servidor. |
| Script con `CREATE DATABASE`, sin `GO` y con restricciones sin nombre. | Script `03` repetible, con restricciones con nombre, que actualiza una tabla existente. | La base ya existe en todas las computadoras y el script debe poder ejecutarse varias veces. |
| Carpetas `obj/` y archivo `.csproj.user` en el repositorio. | No forman parte del proyecto integrado. | Son archivos generados por Visual Studio en cada computadora. |

---

## 5. Flujo de una solicitud entre capas

Ejemplo: registro de un repuesto.

```
Navegador ── POST RegistroRepuesto.aspx (botón Guardar) ──►  Presentación (RegistroRepuesto.aspx.cs)
                                                              │ arma un ClsRepuestoENPP con los textos
                                                              ▼
                                                              Lógica (ClsRepuestoLNPP.FncRegistrarRepuestoPP)
                                                              │ normaliza, valida cada campo y
                                                              │ consulta si el código ya existe
                                                              ▼
                                                              Datos (ClsRepuestoDAPP.FncAgregarRepuestoPP)
                                                              │ INSERT … OUTPUT INSERTED.idrepuesto (con parámetros)
                                                              ▼
                                                              SQL Server: PK, UQ_Repuesto_Codigo y CHECK
                     ◄── 302 a Inventario.aspx?registrado=<id> (aviso de éxito con los datos guardados)
```

Si un dato no cumple una regla, la Capa Lógica lanza `ClsValidacionExcepcionLN` con un mensaje por campo y
la página marca cada campo en rojo sin perder lo escrito.

---

## 6. Detalle de las funciones

### 6.1 Registro de un repuesto

La Capa Lógica normaliza los datos (código en mayúsculas y sin espacios; espacios sobrantes eliminados en los
demás textos) y aplica estas reglas:

| Campo | Regla | Ejemplo de mensaje |
|---|---|---|
| Código | Obligatorio, hasta 15 caracteres, letras y números separados por guiones, único. | «El código solo admite letras, números y guiones, por ejemplo FIL-ACE-001.» |
| Nombre | Obligatorio, hasta 50 caracteres. | «El nombre es obligatorio.» |
| Descripción | Obligatoria, hasta 100 caracteres. | «La descripción admite como máximo 100 caracteres.» |
| Stock inicial | Opcional (vacío equivale a 0); entero entre 0 y 99,999. | «El stock inicial debe ser un número entero entre 0 y 99,999.» |
| Marca y modelo | Obligatorios, hasta 50 caracteres (mismas reglas que un vehículo). | «La marca es obligatoria.» |
| Año | Obligatorio, 4 dígitos, de 1981 al año siguiente al actual. | «El año debe estar entre 1981 y 2027.» |

El código repetido se detecta antes del `INSERT`. Si dos usuarios registran el mismo código al mismo tiempo,
la restricción `UQ_Repuesto_Codigo` de SQL Server detiene el segundo registro y se muestra el mismo mensaje.

### 6.2 Consulta y búsqueda

```sql
SELECT TOP (@Maximo) idrepuesto, codigo, nombre, descripcion, stock, marcacarro, modelocarro, aniocarro
FROM dbo.Repuesto
WHERE @Patron IS NULL
   OR nombre COLLATE Latin1_General_CI_AI LIKE @Patron ESCAPE '\'
   OR codigo = @Codigo
ORDER BY CASE WHEN @Patron IS NULL THEN idrepuesto END DESC,
         CASE WHEN codigo = @Codigo THEN 0 ELSE 1 END,
         nombre;
```

- Sin texto de búsqueda (`@Patron` nulo) se listan los más recientes.
- `COLLATE Latin1_General_CI_AI` compara sin distinguir mayúsculas ni tildes.
- `ESCAPE '\'` junto con `ClsSqlDA.fncEscaparPatronLike` hace que los comodines se busquen como texto.
- El código exacto aparece primero; se muestran como máximo 100 repuestos.

### 6.3 Entrada a bodega

El botón «Registrar entrada» de cada fila abre la ventana con `?entrada=<id>`. Al guardar:

1. La Capa Lógica valida la cantidad: entero entre 1 y 9,999.
2. La Capa de Datos ejecuta `UPDATE dbo.Repuesto SET stock = stock + @Cantidad WHERE idrepuesto = @IdRepuesto`.
   La suma la hace SQL Server sobre el valor actual, de modo que dos entradas simultáneas no se pisan.
3. Si el repuesto ya no existe, no se actualiza ninguna fila y se muestra un aviso.
4. La página redirige con `?actualizado=<id>` y muestra el stock resultante leído de la base de datos.

Redirigir después de guardar evita que la operación se repita si el usuario recarga la página.

---

## 7. Relación con los demás módulos

- **Repuestos utilizados en órdenes** (parte de Sharon, `database/04_RepuestosOrdenSC.sql`): la tabla
  `dbo.RepuestosOrden` registra qué repuesto y cuántas unidades se usaron en cada orden de reparación. Al
  registrar un uso, el stock se descuenta en la misma transacción, solo si alcanza
  (`stock = stock - @Cantidad WHERE stock >= @Cantidad`). La restricción `CK_Repuesto_Stock` garantiza además
  que el stock nunca quede negativo.
- **Reportes** (Fátima): las secciones «Existencias» y «Repuestos utilizados» leen `dbo.Repuesto` y
  `dbo.RepuestosOrden`.
- **Menú**: la opción «Inventario» aparece porque existe la página `~/Inventario/Inventario.aspx`.

---

## 8. Base de datos

`database/03_InventarioPP.sql` crea la tabla con la estructura original y las reglas de integración:

| Columna | Tipo | Regla |
|---|---|---|
| `idrepuesto` | `INT IDENTITY` | `PK_Repuesto` |
| `codigo` | `VARCHAR(15)` | `UQ_Repuesto_Codigo` |
| `nombre` | `NVARCHAR(50)` | No vacío |
| `descripcion` | `NVARCHAR(100)` | No vacía |
| `stock` | `INT` | `DF_Repuesto_Stock` = 0 y `CK_Repuesto_Stock`: `stock >= 0` |
| `marcacarro`, `modelocarro` | `NVARCHAR(50)` | No vacíos (`CK_Repuesto_Textos`) |
| `aniocarro` | `INT` | `CK_Repuesto_Anio`: 1981 en adelante |

Si la tabla ya existe porque se creó con el script original, el script la actualiza sin perder datos: convierte
los textos a `NVARCHAR`, renombra las restricciones (por ejemplo, `chk_stock_negativo` pasa a
`CK_Repuesto_Stock`) y agrega las reglas nuevas. Si encuentra datos que no las cumplen (por ejemplo, un año
anterior a 1981), se detiene con un mensaje que indica qué corregir; después se vuelve a ejecutar.

---

## 9. Seguridad y manejo de errores

| Riesgo | Medida |
|---|---|
| Inyección SQL | Todas las consultas usan parámetros; el texto del usuario nunca se concatena al SQL. |
| Código malicioso en pantalla (XSS) | Todo texto se codifica antes de mostrarse; los nombres con `<` no superan la validación. |
| Registro doble al recargar | Después de guardar, la página redirige (patrón POST, redirección, GET). |
| Datos inválidos en la dirección | `?entrada=` con un repuesto inexistente muestra un aviso y no abre la ventana. |
| Base de datos no disponible | Se registra el error (`ClsRegistroErrores`) y se muestra un mensaje general sin detalles técnicos. |

---

## 10. Convenciones de nombres

Se siguen las del documento del equipo, con las iniciales **PP**:

| Elemento | Regla | Ejemplos |
|---|---|---|
| Clases | `Cls` + nombre + capa (`EN`, `DA`, `LN`) + iniciales | `ClsRepuestoENPP`, `ClsRepuestoDAPP`, `ClsRepuestoLNPP` |
| Funciones | `Fnc…PP` | `FncRegistrarRepuestoPP`, `FncConsultarPP` |
| Procedimientos | `Sub…PP` | `SubCargarRepuestosPP`, `SubNormalizarRepuestoPP` |
| Variables locales | `l` + tipo + nombre + iniciales | `lStrCriterioPP`, `lIntCantidadPP` |
| Parámetros | `p` + tipo + nombre + iniciales | `pIntIdRepuestoPP`, `pStrCantidadPP` |
| Campos de la clase | `g` + tipo + nombre + iniciales | `gObjRepuestoLNPP`, `gStrUrlVolverPP` |
| Controles | prefijo del control + nombre + iniciales | `txtCodigoPP`, `rptRepuestosPP`, `btnGuardarEntradaPP` |

---

## 11. Datos de ejemplo

`database/92_DatosEjemploInventarioPP.sql` inserta ocho repuestos ficticios compatibles con los vehículos de
ejemplo, entre ellos uno sin unidades (bujes de suspensión) y otros con pocas unidades, para que el reporte de
existencias muestre todos los niveles. Busca cada repuesto por su código y no lo duplica ni cambia su stock si
ya existe. Se ejecuta después de `03`; `93_DatosEjemploRepuestosOrdenSC.sql` descuenta luego las unidades
usadas en las órdenes de ejemplo.

---

## 12. Procedimiento de prueba

1. Abrir «Inventario» en el menú: deben aparecer los repuestos de ejemplo, el más reciente primero.
2. Buscar «bateria» (sin tilde): debe aparecer «Batería de 12 V».
3. Registrar un repuesto con el formulario vacío: deben aparecer los seis mensajes de los campos obligatorios.
4. Registrar un repuesto válido con un apóstrofo en el nombre: debe guardarse y aparecer primero en la lista.
5. Registrar otro con el mismo código: debe aparecer «Ya existe un repuesto registrado con este código.»
6. Pulsar «Registrar entrada» en un repuesto, escribir 0 (mensaje de error) y luego 5: el stock debe aumentar
   en 5 unidades.

Pruebas automatizadas realizadas: 67 comprobaciones del inventario, de los repuestos por orden y de los
reportes de repuestos, más 15 pruebas de las restricciones de las tablas en SQL Server
(`pruebas/sql/PruebasInventarioRepuestosSC.sql`).

---

## 13. Justificación de las decisiones técnicas

**¿Por qué el stock se suma en SQL y no en C#?**
Si dos usuarios registran entradas al mismo tiempo y cada uno calcula el total en C#, el segundo sobrescribe al
primero. Con `stock = stock + @Cantidad`, SQL Server suma sobre el valor vigente en ese momento.

**¿Por qué el código es único?**
Identifica físicamente al repuesto en bodega; dos repuestos con el mismo código harían ambiguas las entradas y
los usos en órdenes. Lo garantiza la restricción `UQ_Repuesto_Codigo`.

**¿Por qué se valida en la Capa Lógica si la base de datos también tiene restricciones?**
La Capa Lógica muestra un mensaje claro por campo; las restricciones de SQL Server son la última defensa si
algún dato llega por otra vía.

**¿Por qué las salidas no se registran en este módulo?**
Una salida de bodega corresponde a un repuesto utilizado en una orden. Registrarla desde la orden deja
constancia de en qué reparación se usó y permite el reporte de repuestos utilizados.

**¿Por qué no se puede borrar un repuesto?**
Un repuesto usado en una orden forma parte del historial de esa reparación; la llave foránea de
`dbo.RepuestosOrden` impide borrarlo.
