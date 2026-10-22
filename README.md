# Proyecto-Final-Progra-2
Proyecto final de Programación 2

## AutoManager DMS

Sistema de gestión para un taller mecánico, desarrollado en **ASP.NET Web Forms (.NET Framework 4.8, C# 7.3)** con **SQL Server** y una arquitectura de **4 capas**.

| Módulo | Integrante | Rama |
|---|---|---|
| Gestión de Clientes y Vehículos | Sharon | `Sharon` |
| Órdenes de reparación | Neider | `Neider` |
| Inventario | Pavel | `Pavel` |
| Reportes e integración | Fátima | `Fatima` |

### Quién hizo qué

| Parte | Autor | Archivos |
|---|---|---|
| Módulo 1: Clientes y vehículos | Sharon | `Cls*ClienteSC`, `Cls*VehiculoSC` (entidades, datos y lógica), `CapaPresentacion/Clientes/`, `CapaPresentacion/Vehiculos/`, `Utilidades/ClsFormatoSC.cs`, `database/01_ClientesVehiculosSC.sql`, `database/90_DatosEjemploSC.sql`, `pruebas/` |
| Módulo 2: Órdenes de reparación | Neider | `ClsOrdenReparacionENT`, `ClsOrdenReparacionDANT`, `ClsOrdenReparacionLNT`, `CapaPresentacion/OrdenesReparacion/`, `database/02_OrdenesReparacionNT.sql` (su tabla, con ajustes de integración) |
| Módulo 3: Inventario | Pavel | `ClsRepuestoENPP`, `ClsRepuestoDAPP`, `ClsRepuestoLNPP`, `CapaPresentacion/Inventario/`, `database/03_InventarioPP.sql` (su tabla, con ajustes de integración), `database/92_DatosEjemploInventarioPP.sql`. Explicación completa en [docs/MODULO_INVENTARIO.md](docs/MODULO_INVENTARIO.md) |
| Módulo 4: Reportes | Fátima | `ClsReporteOrdenENFP`, `ClsResumenReportesENFP`, `ClsReporteDAFP`, `ClsReporteLNFP`, `CapaPresentacion/Reportes/`, `database/91_DatosEjemploReportesFP.sql`. Explicación completa en [docs/MODULO_REPORTES.md](docs/MODULO_REPORTES.md) |
| Repuestos utilizados en órdenes (une órdenes e inventario) | Sharon | `ClsRepuestoOrdenENSC`, `ClsRepuestoOrdenDASC`, `ClsRepuestoOrdenLNSC`, `CapaPresentacion/OrdenesReparacion/RepuestosOrdenSC.aspx`, `database/04_RepuestosOrdenSC.sql`, `database/93_DatosEjemploRepuestosOrdenSC.sql`, `pruebas/sql/PruebasInventarioRepuestosSC.sql` |
| Base común del sitio | Sharon | `Site.Master`, `Default.aspx`, `Error.html`, `Web.config`, `Content/autodms.css`, `Scripts/autodms.js`, `ClsConexionDA`, `ClsSqlDA`, `ClsDatoDuplicadoExcepcionDA`, `ClsValidacionExcepcionLN`, `ClsZonaHorariaLN`, `ClsRegistroErrores`, `scripts/`, `compose.yaml` |

En los archivos compartidos (`.csproj` y `Site.Master.cs`) las líneas de cada integrante están marcadas
con comentarios como «parte de Neider», «parte de Pavel» o «parte de Fátima» (o «Inicio / Fin de la parte
de Neider» cuando son varias). Los archivos de cada módulo llevan el encabezado de su autor, y los cambios
hechos en un módulo ajeno al integrarlo tienen el comentario «Ajuste de integración (Sharon)», como el
botón «Repuestos utilizados» del detalle de la orden.

## Estructura de la solución

```
TallerMecanico.sln
├── CapaEntidades/        Clases que viajan entre capas (Cls*ENSC, ClsOrdenReparacionENT, ClsRepuestoENPP, Cls*ENFP)
├── CapaDatos/            Acceso a SQL Server con consultas parametrizadas (ClsConexionDA, Cls*DASC, ClsOrdenReparacionDANT, ClsRepuestoDAPP, ClsReporteDAFP)
├── CapaLogica/           Reglas de negocio y validaciones (Cls*LNSC, ClsOrdenReparacionLNT, ClsRepuestoLNPP, ClsReporteLNFP, ClsZonaHorariaLN)
├── CapaPresentacion/     Sitio Web Forms: Site.Master, Default.aspx, Clientes/, Vehiculos/, OrdenesReparacion/, Inventario/, Reportes/, Error.html
├── database/             Scripts SQL numerados y CONVENCIONES_BD.md
├── docs/                 Guías de los módulos (MODULO_INVENTARIO.md y MODULO_REPORTES.md)
├── pruebas/              Pruebas de capas (consola), de SQL y de conexión
├── scripts/              Configuración local e inicio del sitio en macOS
└── compose.yaml          SQL Server 2022 Express en Docker
```

## Requisitos

| | macOS | Windows |
|---|---|---|
| Entorno de desarrollo | Mono MDK 6.12 (MSBuild y XSP4) | Visual Studio 2022 o 2019 con la carga **Desarrollo de ASP.NET y web** y el componente **.NET Framework 4.8 targeting pack** |
| Base de datos | Docker Desktop (SQL Server 2022 Express en contenedor) | SQL Server Express (`.\SQLEXPRESS`) o Docker Desktop |
| Otros | Python 3 (lo usa el script de configuración) | `sqlcmd` o SQL Server Management Studio (SSMS) |

## Instalación y ejecución en macOS

1. Instala Mono (en Apple Silicon funciona mediante Rosetta 2):
   ```bash
   brew install --cask mono-mdk
   export PATH="/Library/Frameworks/Mono.framework/Versions/Current/Commands:$PATH"
   ```
2. Crea la configuración local. El script genera `.env` con contraseñas aleatorias y los archivos `ConnectionStrings.config` / `Web.config` locales a partir de sus plantillas `.example`. Las contraseñas nunca se muestran y esos archivos están en `.gitignore`.
   ```bash
   ./scripts/configurar-entorno-local.sh
   ```
3. Inicia SQL Server. El servicio `database-init` crea las bases `AutoManagerDMS` y `TallerMecanicoDB`, el usuario `taller_app` (solo lectura y escritura de datos) y ejecuta en orden los scripts numerados de `database/`.
   ```bash
   docker compose up -d
   ```
   SQL Server queda disponible solo en este equipo, en `127.0.0.1,1434`.
4. Compila la solución:
   ```bash
   msbuild TallerMecanico.sln /p:Configuration=Debug
   ```
5. Inicia el sitio y abre <http://127.0.0.1:8080/>:
   ```bash
   ./scripts/iniciar-sitio.sh CapaPresentacion 8080
   ```
   XSP4 se reinicia solo cuando cambia `Web.config` o se vuelve a compilar. Se detiene con `Ctrl+C`.

## Instalación y ejecución en Windows

### Opción A: SQL Server Express

1. Crea la base de datos y ejecuta en orden todos los scripts numerados de `database/` (cada módulo agrega el suyo; usa `-I`, igual que Docker: activa QUOTED_IDENTIFIER, la configuración que recomienda SQL Server):
   ```powershell
   sqlcmd -S .\SQLEXPRESS -E -Q "CREATE DATABASE AutoManagerDMS"
   Get-ChildItem database\[0-9][0-9]_*.sql | Sort-Object Name | ForEach-Object { sqlcmd -S .\SQLEXPRESS -E -I -b -i $_.FullName }
   ```
   También se puede abrir cada script en SSMS y ejecutarlo en orden sobre `AutoManagerDMS`.
   Opcional, para evitar saltos grandes en los identificadores tras reiniciar el servidor:
   ```powershell
   sqlcmd -S .\SQLEXPRESS -E -d AutoManagerDMS -Q "ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF;"
   ```
2. Crea el archivo local de conexión copiando la plantilla:
   ```powershell
   Copy-Item CapaPresentacion\ConnectionStrings.config.example CapaPresentacion\ConnectionStrings.config
   ```
   y cambia el valor de `connectionString` por:
   ```
   Server=.\SQLEXPRESS;Database=AutoManagerDMS;Integrated Security=True;TrustServerCertificate=True;
   ```
3. Abre `TallerMecanico.sln` en Visual Studio, marca **CapaPresentacion** como proyecto de inicio y presiona **F5**. El sitio abre en IIS Express en <http://localhost:8080/> (si el puerto está ocupado, cámbialo en Propiedades del proyecto → Web).

### Opción B: Docker Desktop

1. Copia `.env.example` como `.env` y escribe dos contraseñas seguras (mínimo 8 caracteres con mayúsculas, minúsculas, números y símbolos).
2. Ejecuta `docker compose up -d` (crea las bases, el usuario `taller_app` y las tablas).
3. Copia `CapaPresentacion\ConnectionStrings.config.example` como `ConnectionStrings.config` y reemplaza `__TALLER_APP_PASSWORD__` por el valor de `TALLER_APP_PASSWORD` de tu `.env`.
4. Abre la solución y presiona **F5**, igual que en la opción A.

## Base de datos

- Todo el equipo usa una sola base: **AutoManagerDMS**. La base principal para la integración final es la de Neider (SQL Server Express); para actualizarla basta ejecutar en orden los scripts numerados. `TallerMecanicoDB` solo la usa la prueba de conexión `pruebas/PruebaConexionSQL`.
- Los scripts de `database/` se numeran (`01_`, `02_`, …) y se pueden ejecutar varias veces sin duplicar nada. Docker ejecuta solo los archivos que empiezan con dos dígitos.
- `90_DatosEjemploSC.sql` agrega clientes y vehículos **ficticios** de ejemplo. No usa identificadores fijos, así que se puede ejecutar sobre cualquier base, también sobre una que ya tenga datos. Nunca subas datos reales de personas al repositorio.
- `91_DatosEjemploReportesFP.sql` agrega órdenes de reparación **ficticias** sobre esos vehículos (buscándolos por VIN), para que los reportes muestren información.
- `03_InventarioPP.sql` crea la tabla de repuestos y `04_RepuestosOrdenSC.sql`, la de repuestos utilizados en cada orden. `92_DatosEjemploInventarioPP.sql` agrega repuestos **ficticios** y `93_DatosEjemploRepuestosOrdenSC.sql` registra su uso en las órdenes de ejemplo, descontando el stock.
- Las fechas se guardan en UTC (`SYSUTCDATETIME()`) y se muestran en hora de Guatemala con `CapaLogica/ClsZonaHorariaLN`.
- Las reglas para nuevas tablas (claves foráneas por `VIN`, nombres de restricciones, etc.) están en [database/CONVENCIONES_BD.md](database/CONVENCIONES_BD.md).

## Pruebas

- **Capas (consola):** compila `pruebas/PruebaCapas/PruebaCapas.csproj` y ejecuta `PruebaCapas.exe` (en macOS: `mono pruebas/PruebaCapas/bin/Debug/PruebaCapas.exe`). Necesita su propio `ConnectionStrings.config`, que en macOS crea el script de configuración y en Windows se copia desde la plantilla `.example` de esa carpeta.
- **SQL:** ejecuta `pruebas/sql/PruebasClientesVehiculosSC.sql` y `pruebas/sql/PruebasInventarioRepuestosSC.sql` sobre `AutoManagerDMS` con `sqlcmd -I` o SSMS. Todo ocurre dentro de una transacción que se deshace: no deja datos.
- **Conexión:** `pruebas/PruebaConexionSQL` es un sitio mínimo que solo comprueba la conexión a `TallerMecanicoDB`.

## Cómo integrar un módulo nuevo

- Crea la página principal en su carpeta de `CapaPresentacion` usando `MasterPageFile="~/Site.Master"`. El menú muestra un módulo automáticamente en cuanto su página existe (las rutas están en `Site.Master.cs`); los cuatro módulos ya están integrados.
- Obtén las conexiones con `CapaDatos.ClsConexionDA.fncObtenerConexion()` y usa siempre parámetros (`SqlCommand.Parameters`), nunca texto concatenado.
- Para relacionar con vehículos usa el `VIN`, que es obligatorio y único como la placa: llave foránea a `dbo.Vehiculos (VIN)` con `ON UPDATE CASCADE` (las órdenes usan `(IdVehiculo, VIN)`; ver `CONVENCIONES_BD.md`). `ClsVehiculoLNSC` ya permite obtener un vehículo, listar los de un cliente y buscar por placa o VIN.
- Agrega tus tablas en un script numerado nuevo (ya existen `01_` clientes y vehículos, `02_` órdenes, `03_` inventario y `04_` repuestos por orden; el siguiente sería `05_`).
- Convención de nombres del curso: clases `Cls<Entidad><EN|LN|DA><Iniciales>`, métodos `fnc…`, variables `l<Tipo>…` y parámetros `p<Tipo>…`.

## Seguridad y archivos locales

- **Nunca** subas `.env`, `**/ConnectionStrings.config` ni `pruebas/PruebaConexionSQL/Web.config`: contienen contraseñas y ya están en `.gitignore`. Solo se versionan sus plantillas `.example`.
- `CapaPresentacion/Web.config` sí se versiona porque no contiene credenciales (lee la conexión de `ConnectionStrings.config`).
- Ante un error no controlado o una dirección inexistente el sitio muestra `Error.html`, sin detalles técnicos. Para ver el detalle mientras desarrollas usa el depurador de Visual Studio (F5) o la consola de XSP4 en macOS; también puedes poner `customErrors mode="Off"` solo en tu copia local, sin subir ese cambio.

## Solución de problemas

| Problema | Solución |
|---|---|
| `docker compose up` indica que el puerto 1434 está ocupado | Cambia `SQLSERVER_PORT` en `.env` y el puerto en `ConnectionStrings.config`. |
| Todas las páginas dan 404 en macOS | Inicia el sitio con `./scripts/iniciar-sitio.sh`: XSP4 debe ejecutarse desde la carpeta del proyecto web. |
| `sqlcmd` muestra el error 1934 (QUOTED_IDENTIFIER) | Ejecuta el script con la opción `-I`. |
| Error de configuración: no existe `ConnectionStrings.config` | Créalo desde `ConnectionStrings.config.example` (paso 2 de tu sistema operativo). |
