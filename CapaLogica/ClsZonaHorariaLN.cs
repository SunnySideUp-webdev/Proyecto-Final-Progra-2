//==============================================================
// Nombre: Sharon Yasmin Consuegra Borrayo
// Carné: 0900-25-22148
// Tema: AutoManager DMS - Componente compartido - Lógica de Negocio
//==============================================================
using System;

namespace CapaLogica
{
    /// <summary>
    /// Convierte fechas entre UTC (como se guardan en la base) y la hora de Guatemala
    /// (como se muestran y como se interpretan "hoy" y "este mes"). Es el único lugar
    /// donde se hacen conversiones de zona horaria, para evitar conversiones dobles.
    /// </summary>
    public static class ClsZonaHorariaLN
    {
        /// <summary>
        /// Identificadores de la zona según la plataforma: IANA en Mono/macOS/Linux y
        /// Windows en .NET Framework 4.8. Si ninguno existe se usa UTC-6 fijo, porque
        /// Guatemala no aplica horario de verano.
        /// </summary>
        private static readonly string[] lArrIdentificadores = { "America/Guatemala", "Central America Standard Time" };

        private static readonly TimeZoneInfo lObjZonaGuatemala = fncCargarZona();

        /// <summary>Obtiene la zona horaria utilizada por la aplicación.</summary>
        public static TimeZoneInfo lObjZona
        {
            get { return lObjZonaGuatemala; }
        }

        /// <summary>Convierte una fecha guardada en UTC a la hora de Guatemala.</summary>
        /// <param name="pDatUtc">Fecha en UTC (Kind Utc o Unspecified, como llega de SQL Server).</param>
        /// <returns>La misma fecha expresada en hora de Guatemala.</returns>
        /// <exception cref="ArgumentException">La fecha ya es local: convertirla sería una doble conversión.</exception>
        public static DateTime fncConvertirUtcAGuatemala(DateTime pDatUtc)
        {
            if (pDatUtc.Kind == DateTimeKind.Local)
            {
                throw new ArgumentException(
                    "Se esperaba una fecha en UTC; convertir una hora local produciría una doble conversión.",
                    nameof(pDatUtc));
            }
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(pDatUtc, DateTimeKind.Utc), lObjZonaGuatemala);
        }

        /// <summary>Convierte una fecha y hora de Guatemala a UTC, por ejemplo para filtrar en SQL.</summary>
        /// <param name="pDatGuatemala">Fecha en hora de Guatemala (Kind Unspecified).</param>
        /// <returns>La misma fecha expresada en UTC.</returns>
        /// <exception cref="ArgumentException">La fecha ya está en UTC.</exception>
        public static DateTime fncConvertirGuatemalaAUtc(DateTime pDatGuatemala)
        {
            if (pDatGuatemala.Kind == DateTimeKind.Utc)
            {
                throw new ArgumentException(
                    "La fecha ya está en UTC; convertirla otra vez produciría una doble conversión.",
                    nameof(pDatGuatemala));
            }
            return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(pDatGuatemala, DateTimeKind.Unspecified), lObjZonaGuatemala);
        }

        /// <summary>Obtiene la fecha y hora actuales en Guatemala, sin depender de la zona del servidor.</summary>
        /// <returns>Fecha y hora actuales de Guatemala.</returns>
        public static DateTime fncObtenerAhoraGuatemala()
        {
            return fncConvertirUtcAGuatemala(DateTime.UtcNow);
        }

        /// <summary>Obtiene la fecha de hoy en Guatemala.</summary>
        /// <returns>Fecha de hoy (sin hora) en Guatemala.</returns>
        public static DateTime fncObtenerHoyGuatemala()
        {
            return fncObtenerAhoraGuatemala().Date;
        }

        /// <summary>Busca la zona por sus identificadores conocidos o crea una equivalente.</summary>
        private static TimeZoneInfo fncCargarZona()
        {
            foreach (string lStrIdentificador in lArrIdentificadores)
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(lStrIdentificador);
                }
                catch (TimeZoneNotFoundException)
                {
                }
                catch (InvalidTimeZoneException)
                {
                }
            }

            return TimeZoneInfo.CreateCustomTimeZone(
                "America/Guatemala", TimeSpan.FromHours(-6), "Guatemala (UTC-6)", "Guatemala (UTC-6)");
        }
    }
}
