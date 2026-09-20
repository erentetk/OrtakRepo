<#
    Directory.Build.props icindeki <Version> degerini gunceller.

    Kullanim:
        .\surum-artir.ps1 1.0.1
        .\surum-artir.ps1 1.1.0-dev.1

    SemVer hatirlatmasi:
        1.0.X  -> sadece hata duzeltme, API ayni
        1.X.0  -> yeni ozellik, eski kod bozulmadan derleniyor
        X.0.0  -> BREAKING CHANGE (metot imzasi degisti, sinif silindi,
                  taban TFM yukseldi)
#>
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidatePattern('^\d+\.\d+\.\d+(-[0-9A-Za-z\.\-]+)?$')]
    [string]$YeniSurum
)

$ErrorActionPreference = 'Stop'

$props  = Join-Path $PSScriptRoot 'Directory.Build.props'
$icerik = Get-Content $props -Raw

if ($icerik -notmatch '<Version>(.*?)</Version>') {
    throw "Directory.Build.props icinde <Version> bulunamadi."
}
$eski = $Matches[1]

if ($eski -eq $YeniSurum) {
    Write-Host "Surum zaten $YeniSurum." -ForegroundColor Yellow
    return
}

$icerik = $icerik -replace '<Version>.*?</Version>', "<Version>$YeniSurum</Version>"
Set-Content -Path $props -Value $icerik -NoNewline -Encoding UTF8

Write-Host "Surum guncellendi:  $eski  ->  $YeniSurum" -ForegroundColor Green
Write-Host "Simdi paketle:      .\paketle.ps1" -ForegroundColor Cyan
