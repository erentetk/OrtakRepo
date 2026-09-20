using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Sirket.Ortak;

namespace UygulamaA
{
    internal static class Program
    {
        private static void Main()
        {
            var projeHedefi = Assembly.GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? "?";

            Console.WriteLine("===========================================");
            Console.WriteLine("  UYGULAMA A   (.Net5.0)  ");
            Console.WriteLine("===========================================");
            Console.WriteLine($"Projenin hedefi  : {projeHedefi}");
            Console.WriteLine($"Paket surumu     : {OrtakBilgi.PaketSurumu}");
            Console.WriteLine($"Yuklenen derleme : lib/{OrtakBilgi.HedefCati}");
            Console.WriteLine($"Calisan runtime  : {RuntimeInformation.FrameworkDescription}");
            Console.WriteLine();

            var toplam = new IslemSonucu { Islem = "12 + 30", Sonuc = Hesaplama.Topla(12, 30) };
            var fark = new IslemSonucu { Islem = "12 - 30", Sonuc = Hesaplama.Cikar(12, 30) };

            Console.WriteLine(toplam);
            Console.WriteLine(fark);
        }
    }
}
