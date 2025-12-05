# scripts/run-local.ps1
# Usage: from repo root: .\scripts\run-local.ps1

Write-Host "Building .NET projects..."
dotnet build

Write-Host "Starting docker-compose (DBs, RabbitMQ, services)..."
docker-compose up --build -d

# Allow time for DBs to be ready
Write-Host "Waiting for DBs to initialize (10s)..."
Start-Sleep -s 10

# Run EF migrations for PaymentsService and AccountsService
Write-Host "Applying EF migrations..."
Push-Location src/PaymentsService
dotnet tool restore
dotnet ef database update
Pop-Location

Push-Location src/AccountsService
dotnet tool restore
dotnet ef database update
Pop-Location

Write-Host "All up. Frontend: http://localhost:4200  RabbitMQ: http://localhost:15672 (guest/guest)"
