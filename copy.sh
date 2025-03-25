#!/bin/sh

export WINEPREFIX="$HOME/.wine64"
export RAINWORLD_ROOT="$WINEPREFIX/drive_c/GOG Games/Rain World"

mkdir -p lib
cp "$RAINWORLD_ROOT/BepInEx/utils/PUBLIC-Assembly-CSharp.dll" lib/
cp "$RAINWORLD_ROOT/RainWorld_Data/Managed/UnityEngine.dll" lib/
cp "$RAINWORLD_ROOT/RainWorld_Data/Managed/UnityEngine.CoreModule.dll" lib/
cp "$RAINWORLD_ROOT/RainWorld_Data/Managed/UnityEngine.InputLegacyModule.dll" lib/
cp "$RAINWORLD_ROOT/BepInEx/core/BepInEx.dll" lib/
cp "$RAINWORLD_ROOT/BepInEx/plugins/HOOKS-Assembly-CSharp.dll" lib/
cp "$RAINWORLD_ROOT/BepInEx/core/MonoMod.RuntimeDetour.dll" lib/
cp "$RAINWORLD_ROOT/BepInEx/core/MonoMod.Utils.dll" lib/

cp "$RAINWORLD_ROOT/RainWorld_Data/StreamingAssets/mods/Rain_Meadow/plugins/Rain Meadow.dll" lib/
cp "$RAINWORLD_ROOT/BepInEx/core/Mono.Cil.dll" lib/
cp "$RAINWORLD_ROOT/BepInEx/core/Mono.Cecil.dll" lib/
