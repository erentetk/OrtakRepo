using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Sirket.Ortak;

var projeHedefi = Assembly.GetEntryAssembly()?
    .GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName ?? "?";

Console.WriteLine("===========================================");
Console.WriteLine("  UYGULAMA B   (yeni proje, .NET 10'a gecti)");
Console.WriteLine("===========================================");
Console.WriteLine($"Projenin hedefi  : {projeHedefi}");
Console.WriteLine($"Paket surumu     : {OrtakBilgi.PaketSurumu}");
Console.WriteLine($"Yuklenen derleme : lib/{OrtakBilgi.HedefCati}");
Console.WriteLine($"Calisan runtime  : {RuntimeInformation.FrameworkDescription}");
Console.WriteLine();

// Koleksiyon ifadesi (C# 12). net10.0 tarafinda modern sozdizimi
// kullanabiliyoruz; ayni kod net5.0 hedefinde derlenmezdi.
IslemSonucu[] sonuclar =
[
    new() { Islem = "100 + 250", Sonuc = Hesaplama.Topla(100, 250) },
    new() { Islem = "100 - 250", Sonuc = Hesaplama.Cikar(100, 250) },
    new() { Islem = "100 * 250", Sonuc = Hesaplama.Carp(100, 250) }
];

foreach (var s in sonuclar)
{
    Console.WriteLine(s);
}

