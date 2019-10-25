pushd ..
  pushd Backend
    .\build.ps1 pack
  popd
popd
echo "`n---- R# plugin build finished. Binaries are at ForTea\Backend\output\Debug ----`n"