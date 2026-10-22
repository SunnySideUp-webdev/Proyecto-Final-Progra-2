/* =============================================================
   AutoManager DMS - Comportamiento compartido de la interfaz.
   No reemplaza ninguna validación: todas las reglas se aplican
   en el servidor (Capa Lógica).
   ============================================================= */
(function () {
    "use strict";

    var ATRIBUTO_PROCESANDO = "data-adms-procesando";
    var ATRIBUTO_TEXTO_ORIGINAL = "data-adms-texto-original";

    // Evita envíos dobles: el botón pulsado se deshabilita y muestra un texto de
    // progreso. Se hace con setTimeout para que el navegador incluya primero el
    // botón en los datos enviados y ASP.NET pueda ejecutar su evento Click.
    document.addEventListener("submit", function (evento) {
        var boton = evento.submitter;
        if (!boton || !boton.hasAttribute(ATRIBUTO_PROCESANDO)) {
            return;
        }

        window.setTimeout(function () {
            if (!boton.hasAttribute(ATRIBUTO_TEXTO_ORIGINAL)) {
                boton.setAttribute(ATRIBUTO_TEXTO_ORIGINAL, boton.value);
            }
            boton.value = boton.getAttribute(ATRIBUTO_PROCESANDO);
            boton.disabled = true;
        }, 0);
    });

    // Si el usuario vuelve con el botón "Atrás", el navegador puede restaurar la
    // página con el botón deshabilitado; aquí se deja como estaba.
    window.addEventListener("pageshow", function () {
        var botones = document.querySelectorAll("[" + ATRIBUTO_TEXTO_ORIGINAL + "]");
        for (var i = 0; i < botones.length; i++) {
            botones[i].value = botones[i].getAttribute(ATRIBUTO_TEXTO_ORIGINAL);
            botones[i].disabled = false;
        }
    });
})();
