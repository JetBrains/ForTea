:<<"::CMDLITERAL"
@PUSHD "%~dp0..\Frontend"
@CALL "gradlew.bat" "%*"
@POPD  
@GOTO :EOF
::CMDLITERAL
pushd "../Frontend"
exec "gradlew" "$@"
popd
