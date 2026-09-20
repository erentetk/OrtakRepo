# Ortak Kütüphane Denemesi

Farklı solution'lardaki projelerin, ortak bir C# kütüphanesini **NuGet paketi olarak**
çekmesini simüle eden çalışan bir örnek. Kütüphaneyi istediğin zaman güncelleyip,
tüketici projelere istediğin zaman yeni sürümü aldırıyorsun — tıpkı nuget.org'dan
paket çeker gibi.

## Klasör yapısı

```
KütüphaneUygulamasıDeneme\
│
├── LocalFeed\                      <- YEREL NUGET FEED (sadece bir klasör)
│                                      paketler buraya düşer
├── 01-Kutuphane\                   <- ORTAK KÜTÜPHANE (kendi solution'ı)
│   ├── Sirket.Ortak.sln
│   ├── Directory.Build.props       <- TFM ve SÜRÜM burada, tek yerden
│   ├── nuget.config
│   ├── paketle.ps1                 <- pack + feed'e push
│   ├── surum-artir.ps1             <- sürümü değiştirir
│   └── src\Sirket.Ortak\
│       ├── Hesaplama.cs            <- Topla, Cikar
│       ├── IslemSonucu.cs          <- paylaşılan model
│       └── OrtakBilgi.cs           <- sürüm + yüklenen TFM bilgisi
│
├── 02-UygulamaA\                   <- ESKİ uygulama (net5.0), AYRI solution
│   ├── UygulamaA.sln
│   ├── nuget.config                <- LocalFeed'i kaynak olarak tanımlar
│   └── UygulamaA\
│
├── 03-UygulamaB\                   <- YENİ uygulama (net10.0), AYRI solution
│   ├── UygulamaB.sln
│   ├── nuget.config
│   └── UygulamaB\
│
└── hepsini-test-et.ps1             <- hepsini sırayla çalıştırır
```

Üç solution birbirinden tamamen bağımsız. Uygulamaların kütüphanenin kaynak koduyla
hiçbir bağı yok — sadece paketi tanıyorlar.

## Hızlı başlangıç

PowerShell'i bu klasörde aç ve çalıştır:

```powershell
.\hepsini-test-et.ps1
```

Beklenen çıktı:

```
  UYGULAMA A   (eski proje, henüz geçmedi)
Projenin hedefi  : .NETCoreApp,Version=v5.0
Paket surumu     : 1.0.0
Yuklenen derleme : lib/net5.0
Calisan runtime  : .NET 10.0.x

12 + 30      = 42
12 - 30      = -18

  UYGULAMA B   (yeni proje, .NET 10'a geçti)
Projenin hedefi  : .NETCoreApp,Version=v10.0
Paket surumu     : 1.0.0
Yuklenen derleme : lib/net10.0
Calisan runtime  : .NET 10.0.x

100 + 250    = 350
100 - 250    = -150
```

## Buradaki üç önemli detay

**1. İki uygulama AYNI paketten FARKLI dll aldı.**
`Yuklenen derleme` satırına bak: A `lib/net5.0`, B `lib/net10.0`. Paketin içinde
her iki derleme de var, NuGet tüketicinin hedefine bakıp doğru olanı kendisi seçiyor.
Sen hiçbir şey yapmıyorsun.

**2. Uygulama A, .NET 5 runtime kurulu olmadan çalıştı.**
`Calisan runtime` .NET 10 diyor ama proje net5.0 hedefliyor. Bunu sağlayan şey
`UygulamaA.csproj` içindeki `<RollForward>LatestMajor</RollForward>` satırı.
O satır olmasa "framework not found" hatası alırdın.

**3. Bağlantı `ProjectReference` değil, `PackageReference`.**
`UygulamaA.csproj` içinde şu var:

```xml
<PackageReference Include="Sirket.Ortak" Version="1.0.0" />
```

Kütüphanenin klasörünü bilmiyor bile. Paketi `..\LocalFeed`'den çekiyor; bu yolu
`nuget.config` söylüyor.

## Güncelleme döngüsünü kendin dene

Asıl mesele bu: kütüphaneyi güncelle, tüketicilere **istediğin zaman** aldır.

**Adım 1 — Kütüphaneye yeni fonksiyon ekle.**
`01-Kutuphane\src\Sirket.Ortak\Hesaplama.cs` dosyasındaki `Cikar` metodunun altına:

```csharp
/// <summary>Iki sayiyi carpar. (1.1.0 ile eklendi)</summary>
public static double Carp(double a, double b) => a * b;
```

**Adım 2 — Sürümü artır.** (Yeni özellik, eski kod bozulmuyor → minor)

```powershell
cd 01-Kutuphane
.\surum-artir.ps1 1.1.0
```

**Adım 3 — Paketle ve feed'e at.**

```powershell
.\paketle.ps1
```

Artık `LocalFeed` içinde iki paket var: `1.0.0` ve `1.1.0`.

**Adım 4 — SADECE Uygulama B'yi güncelle.**

```powershell
cd ..\03-UygulamaB
dotnet add UygulamaB\UygulamaB.csproj package Sirket.Ortak --version 1.1.0
```

`UygulamaB\Program.cs` içindeki diziye bir satır ekle:

```csharp
new() { Islem = "100 * 250", Sonuc = Hesaplama.Carp(100, 250) }
```

Çalıştır: `dotnet run --project UygulamaB\UygulamaB.csproj`

**Adım 5 — Uygulama A'ya hiç dokunma ve çalıştır.**

```powershell
dotnet run --project ..\02-UygulamaA\UygulamaA\UygulamaA.csproj
```

A hâlâ `1.0.0` diyor ve sorunsuz çalışıyor. İşte istediğin şey buydu: kütüphane
güncellendi ama her proje **kendi takvimine göre** geçiyor. Hiçbir şey kırılmıyor.

Visual Studio'da aynısını "Manage NuGet Packages → Updates" sekmesinden de
yapabilirsin; paket kaynağı olarak `LocalFeed`'i seç.

Hangi projede güncelleme var diye bakmak için:

```powershell
dotnet list package --outdated
```

## Bilmen gereken tuzak: NuGet cache

NuGet indirdiği paketi `%USERPROFILE%\.nuget\packages\sirket.ortak\<sürüm>\`
altına açar ve bir daha indirmez. Sürüm numarasını **değiştirmeden** yeniden
paketlersen, tüketici proje cache'teki eski dll'i kullanmaya devam eder ve sen
"ama ben düzelttim ya" diye saatlerce debug edersin.

Kural: **her pack'te sürümü artır.** `paketle.ps1` aynı sürümü feed'de görürse
zaten seni uyarıyor.

Yanlışlıkla aynı sürümü ezdiysen:

```powershell
dotnet nuget locals global-packages --clear
```

Geliştirme sırasında sık sık paketleyeceksen ön-sürüm eki kullan:
`.\surum-artir.ps1 1.2.0-dev.1`, `-dev.2` diye git; iş bitince `1.2.0`'ı bas.

## Taban TFM ve .NET 10 geçişi

`01-Kutuphane\Directory.Build.props` içindeki şu satır her şeyi belirliyor:

```xml
<TargetFrameworks>net5.0;net10.0</TargetFrameworks>
```

Kural: **taban TFM, en eski tüketicine göre belirlenir.** Bugün .NET 5 projelerin
olduğu için taban `net5.0`. Bir proje .NET 10'a geçtiğinde paket tarafında hiçbir
şey değişmiyor — o proje otomatik olarak `lib/net10.0` derlemesini almaya başlıyor.

Son .NET 5 projen de geçtiğinde yapacağın tek şey `net5.0;` kısmını silmek. Bu bir
breaking change'dir, paketin major sürümünü artır (`2.0.0`). Geride kalan bir .NET 5
projesi olursa `1.x`'te kalmaya devam eder, kırılmaz — feed'deki eski sürümleri
silmemenin sebebi tam olarak bu.

Geçiş sırasını planlarken **en çok ortak kod kullanan projeyi en sona bırak.** Önce
küçük ve bağımsız projeleri taşı, kütüphanenin iki hedefte de sağlam çalıştığını
gerçek kullanımda gör, sonra büyüklere gir.

Bir not: .NET 5 Mayıs 2022'den beri destek dışı, güvenlik yaması almıyor. Ürün
müşteri sunucularında çalıştığı için bu geçici bir köprü olarak görülmeli.

## Sonraki adım: GitLab Package Registry

Yerel klasör feed tek başına çalışırken iyi, ama ekip ve CI için GitLab'ın kendi
NuGet registry'sine geçersin. Değişmesi gereken tek şey `nuget.config`:

```xml
<add key="gitlab" value="https://gitlab.com/api/v4/projects/<PROJE_ID>/packages/nuget/index.json" />
```

Push komutu:

```powershell
dotnet nuget push .\nupkg\Sirket.Ortak.1.1.0.nupkg --source gitlab
```

Kimlik doğrulama için `api` scope'lu bir Personal Access Token ya da Deploy Token
gerekiyor. **Token'ı bu repodaki nuget.config'e yazıp commit'leme** — kullanıcı
seviyesindeki `%APPDATA%\NuGet\NuGet.Config` dosyasına koy, repodaki dosyada
sadece adres dursun.

Sonrası CI/CD: `.gitlab-ci.yml` içinde tag atınca otomatik pack + push yapan bir
job, elle sürüm artırma işini de bitirir.

## Bunu kendi projelerine uyarlarken

Kütüphaneye taşıdığın sınıflar artık **hiçbir uygulamaya ait değil**. Yani A
projesindeki ortak sınıfı kütüphaneye taşıdıktan sonra A da paketi çeken bir
tüketici oluyor — sahiplik bitiyor.

Tek paketle başla. Bölme zamanı şu: bir parça ağır bağımlılık getiriyorsa. Örneğin
SNMP iletişim kodunu ortak pakete koyarsan, sadece DTO'lara ihtiyacı olan bir proje
de SNMP kütüphanelerini sürüklemek zorunda kalır. O noktada `Sirket.Ortak.Core`
(bağımsız) ve `Sirket.Ortak.Snmp` diye ayırırsın.

Geliştirme sırasında kütüphaneyi ve tüketiciyi aynı anda değiştiriyorsan, pack →
push → update döngüsü yorucu olur. Geçici olarak kütüphane projesini o solution'a
ekleyip `ProjectReference` ile bağlaman çok daha hızlı; iş bitince `PackageReference`'a
dönersin. O geçici hâli commit'lememeye dikkat et.
