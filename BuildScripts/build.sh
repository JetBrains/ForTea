#!/usr/bin/env bash
{
  pushd ..
  {
    {
      pushd Frontend
      ./gradlew :prepare --console=plain
    } ||
    {
      popd
    }
  } &&
  {
    {
      pushd Backend
      dotnet build ForTea.Backend.sln
    } ||
    {
      popd
    }
  } &&
  {
    {
      pushd Frontend
      ./gradlew :buildPlugin --console=plain
    } ||
    {
      popd
    }
  }
} ||
{
  popd
}
