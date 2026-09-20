using System.Reflection;

namespace Sirket.Ortak
{
    /// <summary>
    /// Tuketen uygulamaya paketin hangi surumunun ve hangi TFM
    /// derlemesinin verildigini bildirir.
    /// Coklu hedeflemenin gercekten calistigini GOZLE gormek icin var.
    /// </summary>
    public static class OrtakBilgi
    {
        /// <summary>
        /// Paketin surumu. Directory.Build.props icindeki &lt;Version&gt;
        /// degerinden otomatik okunur, elle guncellemek gerekmez.
        /// </summary>
        public static string PaketSurumu
        {
            get
            {
                var asm = typeof(OrtakBilgi).Assembly;
                var s = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                        ?? asm.GetName().Version?.ToString()
                        ?? "bilinmiyor";

                // SDK surumun sonuna "+<git-hash>" ekleyebiliyor, onu kirpiyoruz.
                var i = s.IndexOf('+');
                return i >= 0 ? s.Substring(0, i) : s;
            }
        }

        /// <summary>
        /// Tuketen uygulamaya paketin hangi TFM derlemesinin
        /// verildigini dondurur (lib/net5.0 mi, lib/net10.0 mi).
        /// </summary>
        public static string HedefCati =>
#if NET10_0_OR_GREATER
            "net10.0";
#elif NET8_0
            "net8.0";
#elif NET5_0
            "net5.0";
#else
            "bilinmeyen";
#endif
    }
}
