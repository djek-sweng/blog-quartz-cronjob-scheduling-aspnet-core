#!/bin/bash

export ASPNETCORE_ENVIRONMENT="Development"

dotnet ef migrations add ApplicationDbContext_"$1" \
  --context ApplicationDbContext \
  --output-dir ./../CronJobScheduling.DataStore/Migrations \
  --project ./../CronJobScheduling.DataStore
