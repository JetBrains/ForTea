#!/usr/bin/env bash
../Frontend/gradlew :prepare --console=plain &&
dotnet build ../Backend/ForTea.Backend.sln &&
../Frontend/gradlew :buildPlugin --console=plain
