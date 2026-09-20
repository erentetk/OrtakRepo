<#
    Butun akisi bastan sona calistirir:
      1) kutuphaneyi paketler ve yerel feed'e atar
      2) Uygulama A'yi (eski, net5.0) calistirir
      3) Uygulama B'yi (yeni, net10.0) calistirir

    Kullanim:  .\hepsini-test-et.ps1
#>
$ErrorActionPreference = 'Stop'
$kok = $PSScriptRoot

Write-Host ""
Write-Host "########## 1) KUTUPHANEYI PAKETLE ##########" -ForegroundColor Magenta
& (Join-Path $kok '01-Kutuphane\paketle.ps1') -Zorla

Write-Host ""
Write-Host "########## 2) UYGULAMA A  (ayri solution) ##########" -ForegroundColor Magenta
dotnet run --project (Join-Path $kok '02-UygulamaA\UygulamaA\UygulamaA.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw "Uygulama A calismadi." }

Write-Host ""
Write-Host "########## 3) UYGULAMA B  (ayri solution) ##########" -ForegroundColor Magenta
dotnet run --project (Join-Path $kok '03-UygulamaB\UygulamaB\UygulamaB.csproj') -c Release
if ($LASTEXITCODE -ne 0) { throw "Uygulama B calismadi." }

Write-Host ""
Write-Host "Hepsi calisti." -ForegroundColor Green
Write-Host "Dikkat et: iki uygulama AYNI paketten farkli derleme aldi." -ForegroundColor Green
