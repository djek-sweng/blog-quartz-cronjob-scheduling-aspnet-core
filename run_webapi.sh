#!/bin/sh

# set variables
CSPROJ="./src/CronJobScheduling.WebApi/CronJobScheduling.WebApi.csproj"

# run .NET application
dotnet run --project "$CSPROJ" -c Release
