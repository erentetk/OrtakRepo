using System;

namespace Sirket.Ortak
{
    /// <summary>
    /// Bir hesaplama isleminin sonucunu tasiyan ortak model.
    /// Iki uygulama da AYNI tipi kullanir; paylasilan DTO ornegi.
    /// </summary>
    public class IslemSonucu
    {
        /// <summary>Yapilan islemin metin gosterimi. Ornek: "12 + 30".</summary>
        public string Islem { get; set; } = string.Empty;

        /// <summary>Islemin sayisal sonucu.</summary>
        public double Sonuc { get; set; }

        /// <summary>Islemin yapildigi an.</summary>
        public DateTime Zaman { get; set; } = DateTime.Now;

        /// <summary>Okunabilir gosterim.</summary>
        public override string ToString() => $"{Islem,-12} = {Sonuc}";
    }
}
