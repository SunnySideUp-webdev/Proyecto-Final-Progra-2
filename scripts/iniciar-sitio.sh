#!/bin/bash
# Inicia XSP4 (servidor web de Mono) para un proyecto Web Forms ya compilado.
# XSP4 busca los archivos en la carpeta actual e ignora --root, por eso este
# script entra primero a la carpeta del proyecto web.
# Uso: ./scripts/iniciar-sitio.sh <carpeta-del-proyecto-web> [puerto]
set -euo pipefail

MONO_COMMANDS="/Library/Frameworks/Mono.framework/Versions/Current/Commands"
case ":$PATH:" in
    *":$MONO_COMMANDS:"*) ;;
    *) PATH="$MONO_COMMANDS:$PATH" ;;
esac

if [ $# -lt 1 ]; then
    echo "Uso: $0 <carpeta-del-proyecto-web> [puerto]" >&2
    exit 1
fi

CARPETA="$1"
PUERTO="${2:-8080}"

if [ ! -f "$CARPETA/Web.config" ]; then
    echo "No existe $CARPETA/Web.config. Ejecuta primero ./scripts/configurar-entorno-local.sh" >&2
    exit 1
fi

cd "$CARPETA"
echo "Sitio: $(pwd)"
echo "URL:   http://127.0.0.1:$PUERTO/"
echo "Detener: Ctrl+C"

# XSP4 termina con código 0 cuando cambia Web.config o un .dll de bin (al recompilar);
# en ese caso se vuelve a iniciar.
# Cualquier otro código (por ejemplo, al pulsar Ctrl+C) detiene el script.
while true; do
    set +e
    xsp4 --address 127.0.0.1 --port "$PUERTO" --nonstop
    CODIGO=$?
    set -e
    if [ "$CODIGO" -ne 0 ]; then
        exit "$CODIGO"
    fi
    echo "Se detectó un cambio de configuración: reiniciando XSP4..."
done
