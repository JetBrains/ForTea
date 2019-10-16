pushd ..
  pushd Frontend
    .\gradlew :prepare
  popd
  pushd Backend
    msbuild ForTea.Backend.sln
  popd
  pushd Frontend
    .\gradlew :buildPlugin
  popd
popd
echo "`n---- Rider plugin build finished. Binaries are at ForTea\Frontend\build\distributions ----`n" 