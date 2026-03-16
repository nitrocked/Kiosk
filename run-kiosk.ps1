# Script to run Kiosk WebApi: Docker, Tests, and Run API
# Usage: .\run-kiosk.ps1

Write-Host "=== Starting Kiosk WebApi Script ===" -ForegroundColor Green

# Initial pause to ensure Docker Desktop is running
Write-Host "`nEnsure Docker Desktop is running, then press any key to continue..." -ForegroundColor Yellow
[Console]::ReadKey($true) | Out-Null

# 1. Start Docker (SQL Server)
Write-Host "`n1. Starting Docker (SQL Server)..." -ForegroundColor Yellow
try {
    docker-compose up -d
    Write-Host "Docker started successfully." -ForegroundColor Green
} catch {
    Write-Host "Error starting Docker: $_" -ForegroundColor Red
    exit 1
}

# Wait a bit for SQL Server to be ready
Write-Host "Waiting 10 seconds for SQL Server to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# 2. Run unit tests
Write-Host "`n2. Running unit tests..." -ForegroundColor Yellow

Write-Host "Running Domain tests..." -ForegroundColor Cyan
try {
    dotnet test Kiosk.Domain.Tests --no-build --verbosity minimal
    Write-Host "Domain tests passed." -ForegroundColor Green
} catch {
    Write-Host "Error in Domain tests: $_" -ForegroundColor Red
    exit 1
}
Write-Host "`nDomain tests completed, press any key to continue..." -ForegroundColor Cyan
[Console]::ReadKey($true) | Out-Null


Write-Host "Running API tests..." -ForegroundColor Cyan
try {
    dotnet test Kiosk.Api.Tests --no-build --verbosity minimal
    Write-Host "API tests passed." -ForegroundColor Green
} catch {
    Write-Host "Error in API tests: $_" -ForegroundColor Red
    exit 1
}
Write-Host "`nAPI tests completed, press any key to continue..." -ForegroundColor Cyan
[Console]::ReadKey($true) | Out-Null

# 3. Run Kiosk.Api
Write-Host "`n3. Running Kiosk.Api..." -ForegroundColor Yellow
Write-Host "Check output to ensure host listening address. The API will be probably available at https://localhost:5200" -ForegroundColor Cyan
Write-Host "Swagger UI will be available at https://localhost:5200/swagger/index.html" -ForegroundColor Green
Write-Host "`nHost will start after pressing any key..." -ForegroundColor Cyan
[Console]::ReadKey($true) | Out-Null

try {
    dotnet run --project Kiosk.Api/Kiosk.Api.csproj
} catch {
    Write-Host "Error running the API: $_" -ForegroundColor Red
    exit 1
}

Write-Host "`n=== Script completed ===" -ForegroundColor Green