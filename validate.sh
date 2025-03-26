#!/bin/bash

set -euo pipefail

source ./.env
dotnet clean

cd src
dotnet restore
dotnet build -c Release
dotnet publish -c Release

cd ../test
dotnet run -- --coverage --coverage-output-format xml --coverage-output $(pwd)/coverage.xml
dotnet reportgenerator \
  -reports:"./coverage.xml" \
  -targetdir:"coveragereport" \
  -reporttypes:Html \
  -license:$REPORT_GENERATOR_LICENSE