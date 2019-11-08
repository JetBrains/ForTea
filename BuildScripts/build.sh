#!/usr/bin/env bash
{
  pushd ..
  {
    {
      pushd Frontend
      ./gradlew :prepare --console=plain
    } || {
      popd
      popd
      exit 1
    }
    popd
  } &&
  {
    {
      pushd Backend
      dotnet build ForTea.Backend.sln
    } || {
      popd
      popd
      exit 1
    }
    popd
  } &&
  {
    {
      pushd Frontend
      ./gradlew :buildPlugin --console=plain
    } || {
      popd
      popd
      exit 1
    }
    popd
  }
} ||
{
  popd
  exit 1
}

popd