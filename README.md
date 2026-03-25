# Nemesis

A mod suite for **MIMESIS** that adds dynamic difficulty, player roles, a proximity radar, and a leveling system. Built on [MimicAPI](https://github.com/NeoMimicry/MimicAPI).

## What's included

- **Difficulty Director** - The game gets harder as more players join and as you progress through days and sessions. Monsters get tougher, weather gets worse. Fully adjustable.
- **Role System** - Each player gets randomly assigned a role at session start: Scout (faster), Tank (more health), Medic (faster interactions), or Scavenger (better loot detection).
- **Proximity Radar** - A minimap in the corner of your screen showing nearby monsters (red), loot (yellow), and other players (green).
- **Persistent Progression** - Earn XP by killing monsters, collecting loot, and clearing rooms. Level up across sessions to get permanent stat bonuses.
- **Admin Panel** - Press **F10** to open the settings panel. Configure everything in-game between sessions.

## How to install

### Step 1: Install MelonLoader

1. Download **MelonLoader** from [https://melonwiki.xyz](https://melonwiki.xyz/#/?id=automated-installation)
2. Run the MelonLoader installer
3. Select your **MIMESIS** game folder (usually `C:\Program Files (x86)\Steam\steamapps\common\MIMESIS`)
4. Click **Install**
5. Launch the game once so MelonLoader sets itself up, then close it

### Step 2: Install MimicAPI

1. Download the latest **MimicAPI.dll** from [Thunderstore](https://thunderstore.io/c/mimesis/p/NeoMimicry/MimicAPI/) or the [MimicAPI GitHub releases](https://github.com/NeoMimicry/MimicAPI/releases)
2. Put `MimicAPI.dll` in your game's **Mods** folder:
   ```
   C:\Program Files (x86)\Steam\steamapps\common\MIMESIS\Mods\
   ```

### Step 3: Install Nemesis

1. Download the latest **Nemesis.dll** from the [Releases page](https://github.com/JonaszJestem/Nemesis/releases)
2. Put `Nemesis.dll` in the same **Mods** folder:
   ```
   C:\Program Files (x86)\Steam\steamapps\common\MIMESIS\Mods\
   ```
3. Launch the game!

Your Mods folder should look like this:
```
MIMESIS/
  Mods/
    MimicAPI.dll
    Nemesis.dll
```

## How to use

- **F10** opens the admin panel where you can tweak all settings
- Settings are saved automatically when you click **Save Config**
- Your XP and level are saved between sessions
- Players who don't have the mod installed can still play with you normally

## Settings you can change

Everything is configurable through the F10 panel:

| Setting | What it does |
|---------|-------------|
| Difficulty weights | How much player count, game day, and session cycle affect difficulty |
| Difficulty range | Set the min/max multiplier (from easy 0.5x to brutal 5x) |
| Role multipliers | How strong each role's bonus is (e.g. Scout speed, Tank HP) |
| Radar range | How far the radar detects entities (10-200m) |
| Radar filters | Toggle monsters, loot, and players on/off |
| XP rewards | How much XP you get for kills, loot, rooms, and survival |
| Level bonuses | How much HP and speed you gain per level |

Settings are saved to `MIMESIS\UserData\Nemesis\config.json`. Your progression data lives in `MIMESIS\UserData\Nemesis\progression.json`.

## For mod developers

If you want to build from source:

1. Clone this repo
2. Make sure MIMESIS is installed with MelonLoader
3. Update paths in `Directory.Build.props` if your game is installed somewhere other than `C:\Program Files (x86)\Steam\steamapps\common\MIMESIS`
4. Open `Nemesis/Nemesis.sln` in Visual Studio or Rider
5. Build — the DLL is automatically copied to your Mods folder

### Running tests

```bash
cd Nemesis/Tests
dotnet test
```

## Requirements

- MIMESIS (Steam)
- MelonLoader 0.6.1+
- MimicAPI 1.0.0+

## License

MIT
