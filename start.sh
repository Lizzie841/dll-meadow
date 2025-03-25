#!/bin/sh

export WINEDLLOVERRIDES="winhttp=n,b"
export WINEPREFIX="$HOME/.wine64"
export RAINWORLD_ROOT="$WINEPREFIX/drive_c/GOG Games/Rain World"

echo '>> BUILD'
dotnet build >& log.txt || exit
echo '>> COPY'
cp -r Mod/* "$RAINWORLD_ROOT/RainWorld_Data/StreamingAssets/mods/DllMeadow/"
cp bin/Debug/net48/* "$RAINWORLD_ROOT/RainWorld_Data/StreamingAssets/mods/DllMeadow/plugins"
echo '>> RUN'
cd "$RAINWORLD_ROOT"
wine RainWorld.exe >& log.txt || exit
echo '>> EXIT'
