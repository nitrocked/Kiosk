#!/bin/bash

# Script to run Kiosk WebApi: Docker, Tests, and Run API
# Usage: ./run-kiosk.sh

echo -e "\e[32m=== Starting Kiosk WebApi Script ===\e[0m"

# Initial pause to ensure Docker Desktop is running
echo -e "\e[33m\nEnsure Docker Desktop is running, then press any key to continue...\e[0m"
read -n 1 -s -r

# 1. Start Docker (SQL Server)
echo -e "\e[33m\n1. Starting Docker (SQL Server)...\e[0m"
docker-compose up -d
if [ $? -ne 0 ]; then
    echo -e "\e[31mError starting Docker\e[0m"
    exit 1
fi
echo -e "\e[32mDocker started successfully.\e[0m"

# Wait a bit for SQL Server to be ready
echo -e "\e[33mWaiting 10 seconds for SQL Server to start...\e[0m"
sleep 10

# 2. Run unit tests
echo -e "\e[33m\n2. Running unit tests...\e[0m"

echo -e "\e[36mRunning Domain tests...\e[0m"
dotnet test Kiosk.Domain.Tests --verbosity minimal
if [ $? -ne 0 ]; then
    echo -e "\e[31mError in Domain tests\e[0m"
    exit 1
fi
echo -e "\e[32mDomain tests passed.\e[0m"
echo -e "\e[33m\nPress any key to continue...\e[0m"
read -n 1 -s -r

echo -e "\e[36mRunning API tests...\e[0m"
dotnet test Kiosk.Api.Tests --verbosity minimal
if [ $? -ne 0 ]; then
    echo -e "\e[31mError in API tests\e[0m"
    exit 1
fi
echo -e "\e[32mAPI tests passed.\e[0m"
echo -e "\e[33m\nPress any key to continue...\e[0m"
read -n 1 -s -r

# 3. Run Kiosk.Api
echo -e "\e[33m\n3. Running Kiosk.Api...\e[0m"
echo -e "\e[36mCheck output to ensure host listening address. The API will be probably available at https://localhost:5200\e[0m"
echo -e "\e[36mSwagger UI will be available at https://localhost:5200/swagger/index.html\e[0m"
echo -e "\e[36mAuth login credentials: admin / password\e[0m"

echo -e "\e[33m\nHost will start after pressing any key...\e[0m"
read -n 1 -s -r

dotnet run --project Kiosk.Api/Kiosk.Api.csproj
if [ $? -ne 0 ]; then
    echo -e "\e[31mError running the API\e[0m"
    exit 1
fi

echo -e "\e[32m\n=== Script completed ===\e[0m"