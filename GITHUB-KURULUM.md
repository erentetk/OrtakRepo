# GitHub Packages Kurulumu

Yerel klasör feed'den GitHub Packages'a geçiş. Adımları sırayla takip et,
her adımın sonunda ne göreceğin yazıyor.

> Not: Şu an `LocalFeed` kurulumu çalışır durumda ve bozulmadı. GitHub'ı
> onun **yanına** ekliyoruz; istediğin zaman geri dönebilirsin.

---

## 1. Repoyu oluştur ve kodu at

GitHub'da yeni bir repo aç (adı `Sirket.Ortak` olabilir), **README ekleme**
seçeneğini işaretleme — klasörde zaten var.

Sonra bu klasörde PowerShell aç:

```powershell
git init
git add .
git commit -m "Ilk surum: ortak kutuphane ve iki tuketici uygulama"
git branch -M main
git remote add origin https://github.com/KULLANICI_ADIN/Sirket.Ortak.git
git push -u origin main
```

`.gitignore` zaten hazır; `bin`, `obj`, `nupkg` ve `LocalFeed` içindeki
paketler repoya gitmeyecek.

---

## 2. Personal Access Token üret

GitHub Packages **sadece classic PAT** kabul ediyor, yeni "fine-grained"
token'lar çalışmıyor. Dikkat et.

GitHub → sağ üstte profil → **Settings** → en altta **Developer settings** →
**Personal access tokens** → **Tokens (classic)** → **Generate new token (classic)**

Seçmen gereken scope'lar:

- `write:packages` — paket yayınlamak için
- `read:packages` — paket çekmek için
- `repo` — private repo kullanacaksan

Token'ı üretince **kopyala ve bir yere kaydet**; sayfadan çıkınca bir daha
göremezsin.

---

## 3. Kendi makinende kaynağı tanımla

```powershell
dotnet nuget add source `
  --username KULLANICI_ADIN `
  --password BURAYA_TOKEN `
  --store-password-in-clear-text `
  --name github `
  "https://nuget.pkg.github.com/KULLANICI_ADIN/index.json"
```

Bu komut token'ı **kullanıcı seviyesindeki** `%APPDATA%\NuGet\NuGet.Config`
dosyasına yazar, repodaki dosyaya değil. Yani token'ı yanlışlıkla commit'leme
riskin yok.

Kontrol et:

```powershell
dotnet nuget list source
```

Listede `github` görünmeli.

---

## 4. csproj'a repo adresini ekle

`01-Kutuphane\src\Sirket.Ortak\Sirket.Ortak.csproj` dosyasında hazır ama
yorumlanmış halde duruyor. Yorumu kaldır ve kullanıcı adını yaz:

```xml
<RepositoryUrl>https://github.com/KULLANICI_ADIN/Sirket.Ortak</RepositoryUrl>
```

Bu satır sayesinde paket, GitHub'da repo sayfana bağlı olarak görünür.

---

## 5. İlk yayını yap

Artık `paketle.ps1` çalıştırmana gerek yok. Sürüm numarası **tag'den** geliyor:

```powershell
git add .
git commit -m "RepositoryUrl eklendi"
git push

git tag v1.0.0
git push origin v1.0.0
```

Tag'i push ettiğin anda `.github\workflows\yayinla.yml` devreye giriyor.
GitHub'da repo → **Actions** sekmesinden canlı izleyebilirsin.

Yeşil tik geldiğinde: repo ana sayfasında sağ kolonda **Packages** başlığı
altında `Sirket.Ortak 1.0.0` belirir.

> Tag adı `v1.1.0` ise paket sürümü `1.1.0` olur — baştaki `v` otomatik
> kırpılıyor. `Directory.Build.props` içindeki `<Version>` değeri CI'da
> geçersiz kılınıyor, artık onu elle düzenlemeyeceksin.

---

## 6. Tüketici projede kullan

`02-UygulamaA\nuget.config` ve `03-UygulamaB\nuget.config` içinde `github`
satırı yorumlanmış halde hazır. Yorumu kaldır, kullanıcı adını yaz:

```xml
<add key="github" value="https://nuget.pkg.github.com/KULLANICI_ADIN/index.json" />
```

Sonra paketi çek:

```powershell
cd 03-UygulamaB
dotnet add UygulamaB\UygulamaB.csproj package Sirket.Ortak --version 1.0.0
dotnet run --project UygulamaB\UygulamaB.csproj
```

`LocalFeed` satırını silmene gerek yok; NuGet her iki kaynağa da bakar ve
paketi nerede bulursa oradan alır.

---

## 7. Bundan sonraki akışın

```powershell
# 1. kodu değiştir, Description / PackageReleaseNotes'u güncelle
# 2. commit + push
git add .
git commit -m "Carp fonksiyonu eklendi"
git push

# 3. tag at
git tag v1.1.0
git push origin v1.1.0

# 4. Actions işi bitirsin, sonra tüketicide:
dotnet add UygulamaB\UygulamaB.csproj package Sirket.Ortak --version 1.1.0
```

Hangi projede güncelleme var diye bakmak için:

```powershell
dotnet list package --outdated
```

---

## Sorun giderme

**Actions'ta push adımı 403 veriyor.**
Kişisel hesaplarda `GITHUB_TOKEN` bazen paket yazma için yetmiyor. Çözüm:
Settings → Secrets and variables → Actions → **New repository secret**,
adı `NUGET_PAT`, değeri 2. adımdaki token. Sonra `yayinla.yml` içinde
`${{ secrets.GITHUB_TOKEN }}` yerine `${{ secrets.NUGET_PAT }}` yaz.

**Restore 401 Unauthorized veriyor.**
Token'ın `read:packages` scope'u yok ya da süresi dolmuş. Yeni token üretip
3. adımı tekrarla. Eski kaynağı önce kaldır: `dotnet nuget remove source github`

**Paket public ama yine de token istiyor.**
Normal, GitHub'ın NuGet registry'si böyle çalışıyor — public paketlerde bile
kimlik doğrulama şart. Şirket içi kullanımda sorun değil; dışarıya açık
dağıtım gerekirse nuget.org'a basman gerekir.

**Paketi görüyorum ama eski sürüm geliyor.**
NuGet cache. Sürümü artırmadan aynı sürümü yeniden yayınladıysan olur.
`dotnet nuget locals global-packages --clear` çalıştır, ama asıl çözüm her
seferinde yeni tag atmak.

**Tag'i yanlış attım.**
```powershell
git tag -d v1.1.0
git push origin :refs/tags/v1.1.0
```
Paket zaten yayınlandıysa GitHub'da Packages sayfasından o sürümü silebilirsin,
ama alışkanlık haline getirme — bir sonraki numaraya geçmek daha temiz.

---

## Gerçek projelerine uyarlarken

Bu klasörde üç solution tek repoda duruyor çünkü bu bir deneme. Gerçek
hayatta **kütüphane kendi repo'sunda olmalı**: `Sirket.Ortak` ayrı bir repo,
A ve B kendi repolarında. O zaman `yayinla.yml` içindeki yolu sadeleştir:

```yaml
dotnet pack src/Sirket.Ortak/Sirket.Ortak.csproj \
```

Gerisi aynı kalır.

---

## İleride GitLab'e geçerken

Mantık birebir aynı, değişen üç şey var:

**1. Feed adresi** (`nuget.config`):
```xml
<add key="gitlab" value="https://gitlab.com/api/v4/projects/PROJE_ID/packages/nuget/index.json" />
```
`PROJE_ID`'yi GitLab'da proje ana sayfasında, isim altında görürsün.

**2. Token:** GitLab'da Personal Access Token ya da Deploy Token, `api`
scope'uyla. Classic/fine-grained ayrımı yok, GitHub'daki o dert burada yok.

**3. CI dosyası:** `.github/workflows/yayinla.yml` yerine `.gitlab-ci.yml`:

```yaml
yayinla:
  stage: deploy
  image: mcr.microsoft.com/dotnet/sdk:10.0
  rules:
    - if: $CI_COMMIT_TAG =~ /^v/
  script:
    - SURUM="${CI_COMMIT_TAG#v}"
    - dotnet pack src/Sirket.Ortak/Sirket.Ortak.csproj -c Release -o nupkg -p:Version=$SURUM
    - dotnet nuget add source "${CI_API_V4_URL}/projects/${CI_PROJECT_ID}/packages/nuget/index.json"
        --name gitlab --username gitlab-ci-token --password $CI_JOB_TOKEN --store-password-in-clear-text
    - dotnet nuget push "nupkg/*.nupkg" --source gitlab
```

GitLab tarafında bir avantaj var: `CI_JOB_TOKEN` her job'da otomatik geliyor,
ayrıca secret tanımlamana gerek kalmıyor. GitHub'daki 403 derdi de yok.

Bir de şunu düşün: şirket içi kendi GitLab kurulumunuz varsa paketler
şirket ağından hiç çıkmaz. Ürün müşteri sunucularında çalıştığı için bu
ciddi bir artı.
