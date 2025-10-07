# File: Rebuild-Docker.ps1
# Description: Cleans solution, rebuilds .NET projects, rebuilds Docker images, and restarts containers

# Stop and remove all containers
Write-Host "Stopping and removing existing Docker containers..."
docker-compose down

# Clean the .NET solution
Write-Host "Cleaning solution..."
dotnet clean YourSolution.sln

# Build the solution
Write-Host "Building solution..."
dotnet build YourSolution.sln -c Release

# Rebuild Docker images without cache
Write-Host "Rebuilding Docker images..."
docker-compose build --no-cache

# Start all containers
Write-Host "Starting containers..."
docker-compose up -d

Write-Host "✅ All services rebuilt and running fresh!"
