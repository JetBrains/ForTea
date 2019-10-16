pushd ..
  pushd Frontend
    .\gradlew :prepare
  popd
  pushd Backend
    msbuild ForTea.Backend.sln
  popd
  pushd Frontend
    .\gradlew :test
  popd
popd