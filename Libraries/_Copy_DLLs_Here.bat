@ECHO OFF
COPY "%ProgramFiles(x86)%\Steam\steamapps\common\Dyson Sphere Program\BepInEx\core\0Harmony.dll" .
COPY "%ProgramFiles(x86)%\Steam\steamapps\common\Dyson Sphere Program\BepInEx\core\BepInEx.dll" .
COPY "%ProgramFiles(x86)%\Steam\steamapps\common\Dyson Sphere Program\DSPGAME_Data\Managed\UnityEngine.dll" .
COPY "%ProgramFiles(x86)%\Steam\steamapps\common\Dyson Sphere Program\DSPGAME_Data\Managed\UnityEngine.UI.dll" .
COPY "%ProgramFiles(x86)%\Steam\steamapps\common\Dyson Sphere Program\DSPGAME_Data\Managed\UnityEngine.CoreModule.dll" .

ECHO Assembly-CSharp.dll is obtained from https://nuget.bepinex.dev/packages/DysonSphereProgram.GameLibs
PAUSE
