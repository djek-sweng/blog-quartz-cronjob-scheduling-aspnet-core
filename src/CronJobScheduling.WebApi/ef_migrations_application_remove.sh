#!/bin/bash

export ASPNETCORE_ENVIRONMENT="Development"

dotnet ef migrations remove \
  --context ApplicationDbContext \
  --project ./../CronJobScheduling.DataStore
