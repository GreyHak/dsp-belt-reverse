# Belt Reverse Direction for Dyson Sphere Program

**Belt Reverse Direction** is a mod for the Unity game Dyson Sphere Program developed by Youthcat Studio and published by Gamera Game.  The game is available on [here](https://store.steampowered.com/app/1366540/Dyson_Sphere_Program/).

To reverse the direction of a conveyor belt, click on any segment of the conveyor belt, and click the reverse button.
![Reverse Button image](https://raw.githubusercontent.com/GreyHak/dsp-belt-reverse/master/ReverseButton.jpg)
If you like this mod, please click the thumbs up at the [top of the page](https://dsp.thunderstore.io/package/GreyHak/DSP_Belt_Reverse_Direction/) (next to the Total rating).  That would be a nice thank you for me, and help other people to find a mod you enjoy.

If you have issues with this mod, please report them on [GitHub](https://github.com/GreyHak/dsp-belt-reverse/issues).  You can also contact me at GreyHak#2995 on the [DSP Modding](https://discord.gg/XxhyTNte) Discord #tech-support channel.

## Installation
This mod uses the BepInEx mod plugin framework.  So BepInEx must be installed to use this mod.  Find details for installing BepInEx [in their user guide](https://bepinex.github.io/bepinex_docs/master/articles/user_guide/installation/index.html#installing-bepinex-1).  This mod was tested with BepInEx x64 5.4.11.0 and Dyson Sphere Program 0.8.22.9331 on Windows 10.

To manually install this mod, add the `DSPBeltReverseDirection.dll` to your `%PROGRAMFILES(X86)%\Steam\steamapps\common\Dyson Sphere Program\BepInEx\plugins\` folder.

This mod can also be installed using ebkr's [r2modman](https://dsp.thunderstore.io/package/ebkr/r2modman/) mod manager by clicking "Install with Mod Manager" on the [DSP Modding](https://dsp.thunderstore.io/package/GreyHak/DSP_Star_Sector_Resource_Spreadsheet_Generator/) site.

## Open Source
The source code for this mod is available for download, review and forking on GitHub [here](https://github.com/GreyHak/dsp-belt-reverse) under the BSD 3 clause license.

## Change Log
### v1.1.3
 - Will now run with Dyson Sphere Program 0.8.22.9331 update.  Rebuild only.
### v1.1.2
 - Fixed a bug with inserters disconnecting after game load.
### v1.1.1
 - Updated button position to deconflict with new Memo Icon game feature.
### v1.1.0
 - Reversing a belts direction will no longer send half the items on the belt to Icarus' inventory.  The mod now attempts to maintain the position of cargo on the belt.
 - Bug fixed in cleanup of the old cargo path.
 - Many debug messages downgraded from Info to Debug.  So BepInEx's default log level setting will be far less cluttered.
### v1.0.1
 - Rebuild required for the recent Dyson Sphere Program [0.6.17.5932 update](https://store.steampowered.com/news/app/1366540/view/4178750236020840924).
### v1.0.0
 - Secondary belt inputs will no longer break a primary line when reversed.
 - Thank you to iskabot and [Appun](https://dsp.thunderstore.io/package/appuns/) and Nordblum on Discord for beta testing.
### v0.1.0-beta
 - Initial release.
