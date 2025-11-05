#!/usr/bin/env pwsh
# To run this script, in the developer powershell run the following script:
#   pwsh ./publish-build.ps1

# PowerShell script to build and publish PinChecker app to Docker Desktop with auto-versioning

$ErrorActionPreference = "Stop"

Write-Host "Starting PinChecker Docker Build and Publish Process" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green

# Get project information
$projectPath = "."
$dockerfilePath = "./Dockerfile"
$csprojPath = Join-Path $PSScriptRoot "PinChecker.csproj"

# Extract version from csproj
Write-Host "Reading version from project file..." -ForegroundColor Yellow
if (-not (Test-Path $csprojPath)) {
    Write-Host "PinChecker.csproj not found at: $csprojPath" -ForegroundColor Red
    exit 1
}

$csprojContent = Get-Content $csprojPath -Raw
$versionMatch = [regex]::Match($csprojContent, '<Version>([^<]+)</Version>')
if (-not $versionMatch.Success) {
    Write-Host "Could not find Version element in $csprojPath" -ForegroundColor Red
    exit 1
}

$baseVersion = $versionMatch.Groups[1].Value
Write-Host "Base version from csproj: $baseVersion" -ForegroundColor Green

# Check for existing images with this version to determine build number
Write-Host "Checking for existing builds..." -ForegroundColor Yellow
$existingImages = docker images --filter "reference=pin-checker" --format "{{.Tag}}" | Where-Object { $_ -like "$baseVersion.*" }

$buildNumber = 1
if ($existingImages) {
    $buildNumbers = $existingImages | ForEach-Object {
        if ($_ -match "$baseVersion\.(\d+)") {
            [int]$matches[1]
        }
    } | Sort-Object -Descending
    
    if ($buildNumbers) {
        $buildNumber = $buildNumbers[0] + 1
    }
}

$fullVersion = "$baseVersion.$buildNumber"
$imageName = "pin-checker:$fullVersion"

Write-Host "Build Configuration:" -ForegroundColor Cyan
Write-Host "  Project Path: $projectPath" -ForegroundColor White
Write-Host "  Dockerfile: $dockerfilePath" -ForegroundColor White
Write-Host "  Version Tag: $fullVersion" -ForegroundColor White
Write-Host "  Image Name: $imageName" -ForegroundColor White
Write-Host ""

# Verify Docker is running
Write-Host "Checking Docker availability..." -ForegroundColor Yellow
try {
    $dockerVersion = docker version --format "{{.Client.Version}}"
    Write-Host "Docker is running (Version: $dockerVersion)" -ForegroundColor Green
} catch {
    Write-Host "Docker is not running or not available. Please start Docker Desktop." -ForegroundColor Red
    exit 1
}

# Verify Dockerfile exists
if (-not (Test-Path $dockerfilePath)) {
    Write-Host "Dockerfile not found at: $dockerfilePath" -ForegroundColor Red
    exit 1
}

Write-Host "Dockerfile found" -ForegroundColor Green

# Clean up any existing containers and images with the same base version (but not the new one)
Write-Host "Cleaning up old containers and images for base version $baseVersion ..." -ForegroundColor Yellow

# Find all images with the same base version (including previous builds)
$oldImages = docker images --filter "reference=pin-checker" --format "{{.Repository}}:{{.Tag}}" | Where-Object { $_ -like "$($imageName.Split(':')[0]):$baseVersion.*" -and $_ -ne $imageName }

foreach ($oldImage in $oldImages) {
    Write-Host "  Processing old image: $oldImage" -ForegroundColor White
    # Find all containers using this old image
    $containers = docker ps -a --filter "ancestor=$oldImage" --format "{{.ID}}"
    if ($containers) {
        Write-Host "    Stopping and removing containers using $oldImage..." -ForegroundColor White
        docker stop $containers | Out-Null
        docker rm $containers | Out-Null
    }
    # Remove the old image
    Write-Host "    Removing image $oldImage..." -ForegroundColor White
    docker rmi $oldImage | Out-Null
}
Write-Host "Old containers and images cleaned up." -ForegroundColor Green

# Build Docker image with version tag
Write-Host "Building Docker image..." -ForegroundColor Yellow
$buildArgs = @("build", "-t", $imageName, "-f", $dockerfilePath, ".")

Write-Host "  Command: docker $($buildArgs -join ' ')" -ForegroundColor White
Write-Host ""

try {
    & docker @buildArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Docker build failed with exit code $LASTEXITCODE"
    }
    Write-Host "Docker image built successfully!" -ForegroundColor Green
} catch {
    Write-Host "Docker build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Verify the image was created
Write-Host "Verifying image creation..." -ForegroundColor Yellow
$imageExists = docker images --filter "reference=$imageName" --format "{{.Repository}}:{{.Tag}}"
if ($imageExists) {
    Write-Host "Image '$imageName' created successfully" -ForegroundColor Green
    
    # Get image details
    $imageInfo = docker images --filter "reference=$imageName" --format "table {{.Repository}}\t{{.Tag}}\t{{.Size}}\t{{.CreatedAt}}"
    Write-Host "Image Details:" -ForegroundColor Cyan
    Write-Host $imageInfo -ForegroundColor White
} else {
    Write-Host "Failed to create image '$imageName'" -ForegroundColor Red
    exit 1
}

# Show final status
Write-Host ""
Write-Host "Build and Publish Complete!" -ForegroundColor Green
Write-Host "=================================================" -ForegroundColor Green
Write-Host "Your PinChecker app is now available in Docker Desktop as:" -ForegroundColor White
Write-Host "  Image Name: $imageName" -ForegroundColor Cyan

# Start the container
Write-Host ""
Write-Host "Starting container..." -ForegroundColor Yellow
$containerName = "pin-checker-prod"
$dataVolume = "C:\Users\prage\OneDrive\Documents\MyStuff\MyApps\PinChecker"

# Stop and remove existing container with same name
$existingContainer = docker ps -a --filter "name=$containerName" --format "{{.ID}}"
if ($existingContainer) {
    Write-Host "  Stopping and removing existing container '$containerName'..." -ForegroundColor White
    docker stop $existingContainer | Out-Null
    docker rm $existingContainer | Out-Null
}

# Verify data directory exists
if (-not (Test-Path $dataVolume)) {
    Write-Host "Creating data directory: $dataVolume" -ForegroundColor Yellow
    New-Item -ItemType Directory -Path $dataVolume -Force | Out-Null
    Write-Host "Created data directory for volume mounting" -ForegroundColor Green
} else {
    Write-Host "Data directory exists: $dataVolume" -ForegroundColor Green
}

try {
    docker run -d --name $containerName -v "${dataVolume}:/app/data" $imageName
    Write-Host "Container '$containerName' started successfully!" -ForegroundColor Green
    Write-Host "Monitor logs with: docker logs -f $containerName" -ForegroundColor Cyan
    Write-Host "Data will be persisted to: $dataVolume" -ForegroundColor Cyan
} catch {
    Write-Host "Failed to start container: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "Script completed successfully!" -ForegroundColor Green