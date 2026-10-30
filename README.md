# Fast Crucible Pour

For Vintage Story 1.22.7. Transfers up to 100 metal units per pour step instead of 2 (50x the transfer rate). The normal initial pouring animation delay remains.

## Install

Copy FastCruciblePour-1.0.0.zip into `%APPDATA%\VintagestoryData\Mods`, then restart the game. Leave the ZIP compressed. Multiplayer requires the mod on the server and each client.

## Behavior

Changes the vanilla crucible pouring method with the game's bundled Harmony library. Preserves mold capacity, remaining metal, temperature, animations, and empty-crucible handling. Other mods that replace the same pouring method may conflict.

## Build

Run `powershell -ExecutionPolicy Bypass -File .\build.ps1`. The build uses the installed game's API and bundled Harmony, the .NET 10.0.7 runtime assemblies, and the C# compiler from SDK 9.0.302. Adjust paths in build.ps1 for another machine.

To remove, delete the mod ZIP and restart the game.

## Source repository

[FastCruciblePour](https://github.com/tannerjriddle97/FastCruciblePour) contains the addon source, authoring files and available test sources. No open-source license has been selected for this source. Game/mod dependencies, local profiles, saves, recovery provenance and generated packages are excluded. Historical audit/probe scripts require a separate deliberate runtime-testing decision; publication does not run them. Interactive acceptance limits remain as documented.
