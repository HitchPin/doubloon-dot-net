#!/bin/bash

set -euo pipefail

source ./.env || true
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
  -assemblyfilters:-InternalReservedAttributeContent \
  -classfilters:-System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute,-System.Diagnostics.CodeAnalysis.MemberNotNullAttribute,-System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute,-System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute,-System.Diagnostics.CodeAnalysis.NotNullWhenAttribute,-System.Runtime.CompilerServices.CallerArgumentExpressionAttribute,-System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute,-System.Runtime.CompilerServices.RefSafetyRulesAttribute,-System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute
  #-license:$REPORT_GENERATOR