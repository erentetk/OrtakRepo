<#
    Kutuphaneyi paketler ve yerel feed'e (..\LocalFeed) atar.

    Kullanim:
        .\paketle.ps1
        .\paketle.ps1 -Zorla      # ayni surum varsa sormadan uzerine yaz
#>
param(
    [switch]$Zorla
)

$ErrorActionPreference = 'Stop'

$kok   = $PSScriptRoot
$proje = Join-Path $kok 'src\Sirket.Ortak\Sirket.Ortak.csproj'
$props = Join-Path $kok 'Directory.Build.props'
$feed  = Join-Path (Split-Path -Parent $kok) 'LocalFeed'
$cikti = Join-Path $kok 'nupkg'

# Surumu Directory.Build.props icinden oku (tek kaynak orada)
$xml = [xml](Get-Content $props -Raw)
$dugum = $xml.SelectSingleNode('//Version')
if ($null -eq $dugum) { throw "Directory.Build.props icinde <Version> bulunamadi." }
$surum = $dugum.InnerText.Trim()

Write-Host "Paketlenecek surum: $surum" -ForegroundColor Cyan

New-Item -ItemType Directory -Force -Path $feed  | Out-Null
New-Item -ItemType Directory -Force -Path $cikti | Out-Null

# Ayni surum feed'de zaten varsa uyar. Bu en sik yapilan hata.
$mevcut = Join-Path $feed "Sirket.Ortak.$surum.nupkg"
if (Test-Path $mevcut) {
    Write-Host ""
    Write-Host "UYARI: $surum surumu feed'de zaten var." -ForegroundColor Yellow
    Write-Host "       Uzerine yazsan bile tuketici projeler NuGet cache'indeki" -ForegroundColor Yellow
    Write-Host "       ESKI dll'i kullanmaya devam edebilir." -ForegroundColor Yellow
    Write-Host "       Dogrusu:  .\surum-artir.ps1 1.0.1   yapip tekrar paketlemek." -ForegroundColor Yellow
    Write-Host ""
    if (-not $Zorla) {
        $cevap = Read-Host "Yine de uzerine yazilsin mi? (e/h)"
        if ($cevap -ne 'e') { Write-Host "Iptal edildi." -ForegroundColor Red; return }
    }
    Remove-Item $mevcut -Force
}

Write-Host ""
Write-Host "==> dotnet pack" -ForegroundColor Cyan
dotnet pack $proje -c Release -o $cikti
if ($LASTEXITCODE -ne 0) { throw "pack basarisiz oldu." }

Write-Host ""
Write-Host "==> feed'e gonderiliyor: $feed" -ForegroundColor Cyan
dotnet nuget push (Join-Path $cikti "Sirket.Ortak.$surum.nupkg") -s $feed
if ($LASTEXITCODE -ne 0) { throw "push basarisiz oldu." }

Write-Host ""
Write-Host "Tamam. Feed'deki paketler:" -ForegroundColor Green
Get-ChildItem $feed -Filter *.nupkg | ForEach-Object { Write-Host "   $($_.Name)" }
Write-Host ""
Write-Host "Tuketici tarafta guncellemek icin:" -ForegroundColor Cyan
Write-Host "   dotnet add package Sirket.Ortak --version $surum"
