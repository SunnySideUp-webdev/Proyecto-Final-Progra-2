#!/bin/bash
# Prepara la configuración LOCAL del taller mecánico (archivos ignorados por git):
#   1. Crea .env con contraseñas aleatorias si todavía no existe.
#   2. Crea cada archivo .config (Web.config, ConnectionStrings.config) a partir
#      de su plantilla .config.example.
# Las contraseñas nunca se muestran en pantalla.
# Uso: ./scripts/configurar-entorno-local.sh [--regenerar]
set -euo pipefail

RAIZ="$(cd "$(dirname "$0")/.." && pwd)"
ARCHIVO_ENV="$RAIZ/.env"
REGENERAR_CONFIG="no"
case "${1:-}" in
    --regenerar|--regenerar-webconfig) REGENERAR_CONFIG="si" ;;
esac

generar_contrasena() {
    # 24 caracteres con mayúsculas, minúsculas, números y símbolos que no
    # necesitan escaparse en .env, XML, cadenas de conexión ni en la shell.
    python3 -I -c '
import secrets, string
simbolos = "-_.@%+"
alfabeto = string.ascii_letters + string.digits + simbolos
while True:
    c = "".join(secrets.choice(alfabeto) for _ in range(24))
    if (c[0].isalnum() and any(x.islower() for x in c) and any(x.isupper() for x in c)
            and any(x.isdigit() for x in c) and sum(x in simbolos for x in c) >= 2):
        print(c, end="")
        break
'
}

leer_variable() {
    # Lee una variable de .env sin ejecutar el archivo.
    sed -n "s/^$1=//p" "$ARCHIVO_ENV" | tail -n 1
}

if [ -f "$ARCHIVO_ENV" ]; then
    echo "• .env ya existe: se conserva sin cambios."
else
    (
        umask 077
        {
            echo "# Configuración LOCAL del taller mecánico. NO se sube a GitHub (.gitignore)."
            echo "# Generado por scripts/configurar-entorno-local.sh el $(date '+%Y-%m-%d')."
            echo "MSSQL_SA_PASSWORD=$(generar_contrasena)"
            echo "TALLER_APP_PASSWORD=$(generar_contrasena)"
            echo "SQLSERVER_PORT=1434"
        } > "$ARCHIVO_ENV"
    )
    echo "• Creado .env con contraseñas aleatorias (no se muestran)."
fi

TALLER_APP_PASSWORD="$(leer_variable TALLER_APP_PASSWORD)"
if [ -z "$TALLER_APP_PASSWORD" ]; then
    echo "Error: falta TALLER_APP_PASSWORD en .env" >&2
    exit 1
fi
export TALLER_APP_PASSWORD

find "$RAIZ" -name '*.config.example' -not -path '*/bin/*' -not -path '*/obj/*' | sort | while IFS= read -r plantilla; do
    destino="${plantilla%.example}"
    relativo="${destino#"$RAIZ"/}"
    if [ -f "$destino" ] && [ "$REGENERAR_CONFIG" = "no" ]; then
        echo "• $relativo ya existe: se conserva (usa --regenerar para recrearlo)."
        continue
    fi
    (
        umask 077
        python3 -I -c '
import os, sys
contenido = open(sys.argv[1], encoding="utf-8").read()
contenido = contenido.replace("__TALLER_APP_PASSWORD__", os.environ["TALLER_APP_PASSWORD"])
open(sys.argv[2], "w", encoding="utf-8").write(contenido)
' "$plantilla" "$destino"
    )
    chmod 600 "$destino"
    echo "• Creado $relativo con la cadena de conexión local."
done
