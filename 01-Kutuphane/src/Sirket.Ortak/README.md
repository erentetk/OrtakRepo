# Sirket.Ortak

Projeler arasi ortak kullanilan siniflar.

## Icerik

- `Hesaplama` : temel aritmetik islemler (`Topla`, `Cikar`)
- `IslemSonucu` : islem sonucunu tasiyan ortak model
- `OrtakBilgi` : paket surumu ve yuklenen TFM derlemesi bilgisi

## Hedef catilar

`net5.0` ve `net10.0`. Tuketen projenin hedefine gore NuGet dogru
derlemeyi kendisi secer.

## Kurulum

```
dotnet add package Sirket.Ortak
```

Paket yerel feed'den geliyorsa solution kokundeki `nuget.config`
dosyasinda `LocalFeed` kaynagi tanimli olmalidir.
