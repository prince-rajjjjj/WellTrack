# WellTrack — Start all services
# Run from project root: .\run-all.ps1

$root = $PSScriptRoot

$services = @(
    @{ Name = "Gateway        :5000"; Path = "WellTrack.Gateway" },
    @{ Name = "Identity       :5001"; Path = "WellTrack.Services.Identity" },
    @{ Name = "Wellness       :5002"; Path = "WellTrack.Services.Wellness" },
    @{ Name = "Participation  :5003"; Path = "WellTrack.Services.Participation" },
    @{ Name = "Engagement     :5004"; Path = "WellTrack.Services.Engagement" },
    @{ Name = "Compliance     :5005"; Path = "WellTrack.Services.Compliance" },
    @{ Name = "Reporting      :5006"; Path = "WellTrack.Services.Reporting" },
    @{ Name = "Notification   :5007"; Path = "WellTrack.Services.Notification" }
)

foreach ($svc in $services) {
    $fullPath = Join-Path $root $svc.Path
    Start-Process powershell -ArgumentList "-NoExit", "-Command", `
        "cd '$fullPath'; Write-Host '--- $($svc.Name) ---' -ForegroundColor Cyan; dotnet run"
    Start-Sleep -Milliseconds 500
}

Write-Host ""
Write-Host "All 8 services starting..." -ForegroundColor Green
Write-Host "Gateway  -> http://localhost:5000" -ForegroundColor Yellow
