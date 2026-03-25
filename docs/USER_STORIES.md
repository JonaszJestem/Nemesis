# Nemesis Mod - User Stories and User Flows

## 1. User Stories

### 1. Difficulty Director
- As a **host**, I want to configure dynamic difficulty scaling based on player count, game day, and session cycle, so that the game challenge adapts automatically to the group's progress.
- As a **player**, I want to see a HUD label showing the current difficulty multiplier, so that I understand why enemies feel harder or easier.

### 2. Role System
- As a **host**, I want to assign randomized roles (Scout, Tank, Medic, Scavenger) to all players at session start, so that each player has a unique gameplay identity with stat bonuses and trade-offs.
- As a **player**, I want to see my assigned role and its description on the HUD, so that I know what my strengths are and can play accordingly.

### 3. Damage Scale
- As a **host**, I want to adjust a global damage multiplier for all players, so that I can make combat easier or harder to suit the group's skill level.

### 4. More Mimics
- As a **host**, I want to increase the mimic spawn rate by a configurable multiplier, so that exploration feels more dangerous and encounters are more frequent.

### 5. Stamina (Infinite Stamina)
- As a **host**, I want to enable infinite stamina for all players, so that the group can sprint and perform actions without stamina constraints.

### 6. Jump (Better Jump)
- As a **host**, I want to enable an enhanced jump with configurable velocity, so that players can reach higher platforms and have more vertical mobility.

### 7. Fullbright
- As a **host**, I want to enable fullbright lighting with adjustable ambient intensity, so that all players can see clearly in dark areas without flashlights.

### 8. Loot Drop (Enemy Drop Loot)
- As a **host**, I want to configure enemies to drop loot on death with adjustable drop chance and max drops per kill, so that combat is more rewarding and incentivized.

### 9. More Voices
- As a **host**, I want to increase the maximum number of voice recordings allowed, so that players can record and use more speech events during gameplay.

### 10. Marker
- As a **host**, I want to enable infinite paintballs, permanent marks, and color cycling for the marker tool, so that players can mark locations and objects without resource limits.

### 11. Inventory Expansion
- As a **host**, I want to expand the player inventory by additional slots, so that players can carry more items without needing to return to base as often.

### 12. Auto Loot
- As a **host**, I want to enable automatic loot pickup within a configurable range, so that players can collect items without manually interacting with each one.

### 13. Fly
- As a **host**, I want to enable flight mode with adjustable speed, so that players can freely navigate the map for exploration or debugging purposes.

### 14. Proximity Radar
- As a **player**, I want to see a radar overlay showing nearby monsters, loot, and players, so that I have situational awareness of my surroundings without needing line of sight.

### 15. Persistent Progression
- As a **player**, I want to earn XP from kills, loot collection, item sales, and room clears that persists across sessions, so that I feel a sense of long-term progression and receive incremental stat bonuses.

### 16. FOV (Field of View)
- As a **player**, I want to adjust my field of view, so that I can set a comfortable viewing angle that suits my monitor and preferences.

### 17. Voice Fix
- As a **player**, I want to enable a voice fix module, so that known voice chat issues are resolved without affecting other players.

### 18. Health Indicators
- As a **player**, I want to see floating damage numbers and health bars on enemies, so that I can track combat effectiveness and remaining enemy health at a glance.

### 19. Tooltip
- As a **player**, I want to customize tooltip font size, so that item and object tooltips are readable at my preferred scale.

### 20. ESP (Extra-Sensory Perception)
- As a **player**, I want to see entity outlines or labels through walls with optional distance display, so that I can locate monsters, loot, and other players regardless of line of sight.

---

## 2. User Flows

### Flow 1: Host Configuring the Game

1. Host launches the game and loads the Nemesis mod (MelonLoader initializes `NemesisMod.OnInitializeMelon()`).
2. Mod loads saved config from disk via `ConfigManager.Load()`, populating the `SuiteConfig` with all 20 module configs.
3. All 20 modules are instantiated and `Initialize()` is called on each.
4. Host creates or joins a lobby.
5. Host presses **F10** to open the Admin Panel.
   - Cursor is unlocked and made visible.
   - Host status is checked via `PlayerAPI.GetLocalPlayer().IsHost`.
6. The panel renders all **20 tabs** (host view): Difficulty, Roles, Damage, Mimics, Stamina, Jump, Fullbright, LootDrop, Voices, Marker, Inventory, AutoLoot, Fly, Radar, Progression, FOV, VoiceFix, Health, Tooltip, ESP.
7. Host clicks a tab (e.g., "Difficulty") to view and adjust settings.
8. Host adjusts sliders and toggles within the tab's UI.
9. Host clicks **"Save Config"**:
   - `ConfigManager.Save(_config)` writes the full `SuiteConfig` to disk as JSON.
   - `NemesisMod.NotifyConfigChanged()` triggers `ConfigSync.ForcePush()`.
   - The 13 synced module configs are serialized into a `SyncPayload` JSON and written to Steam lobby data via `SteamLobbyHelper.SetLobbyData()`.
10. Host presses **F10** again or clicks **"Close [F10]"** to close the panel; cursor lock is restored.
11. Every 2 seconds, `ConfigSync.OnUpdate()` checks for config changes and pushes updates if the JSON has changed.

### Flow 2: Client Joining a Game

1. Client launches the game with Nemesis mod installed.
2. Mod loads the client's local config from disk (their personal saved settings).
3. Client joins the host's Steam lobby.
4. Every 2 seconds, `ConfigSync.OnUpdate()` runs on the client side:
   - Calls `PullConfigFromLobby()`.
   - Reads the `SyncPayload` JSON from Steam lobby data.
   - Applies the host's gameplay-affecting configs to the client's local `SuiteConfig` via `ConfigSyncLogic.Apply*Config()` methods.
   - The 14 synced modules (Difficulty, Roles, Damage, MoreMimics, Stamina, Jump, Fullbright, LootDrop, MoreVoices, Marker, InventoryExpansion, AutoLoot, Fly, Progression) now reflect the host's settings.
5. Client presses **F10** to open the Admin Panel.
   - Panel shows **(Client mode)** label.
   - Only **6 tabs** are available: Radar, FOV, VoiceFix, Health, Tooltip, ESP.
   - These are visual/audio-only settings that remain local to the client.
6. Client adjusts personal preferences (e.g., radar size, FOV, tooltip font size).
7. Client clicks **"Save Config"** to persist their local visual settings to disk.
   - `ForcePush()` is called but does nothing since `_isHost` is false.

### Flow 3: Role Assignment Flow

1. A session starts, triggering `ModuleEventBus.OnSessionStarted`.
2. `RoleSystemModule.OnSessionStarted()` fires:
   - Checks `_config.Enabled`; if false, exits.
   - Resets `_cachedLocalRole` to `Role.None`, `_roleRetryCount` to 0.
   - Calls `TryAssignRoles()` immediately.
3. **If the local player is the host:**
   a. `RoleAssigner.AssignRoles()` randomly assigns one of four roles (Scout, Tank, Medic, Scavenger) to each connected player.
   b. The host's own role is cached, and `RoleStatDelta` is computed with the configured multipliers (e.g., Scout: 1.15x speed; Tank: 1.25x HP, 0.95x speed; Medic: 1.20x interact; Scavenger: 1.30x loot).
   c. Role assignments are serialized as a `Dictionary<playerName, roleName>` JSON and pushed to Steam lobby data under `LobbyKeys.Roles`.
   d. If players are not yet loaded (role is `None`), `TryAssignRoles()` returns false.
4. **If players were not loaded (retry needed):**
   a. `OnUpdate()` increments `_roleRetryTimer` each frame.
   b. Every 1 second, it increments `_roleRetryCount` and retries `TryAssignRoles()`.
   c. This continues for up to **10 retries** (`MaxRoleRetries`).
   d. If all 10 retries fail, role assignment is abandoned for the session.
5. **If the local player is a client:**
   a. `PullLocalRole()` reads the role assignments JSON from lobby data.
   b. Looks up the local player's name in the dictionary.
   c. Parses the role enum, caches it, computes `RoleStatDelta`.
   d. Applies HP and speed multipliers to the player's stat manager using `CachedStats.ApplyMultipliers()`.
   e. If lobby data is not yet available, the retry mechanism (step 4) handles it.
6. Once a role is assigned, the HUD displays the role name (color-coded) and description at position (10, 10) on screen.

### Flow 4: XP Progression Flow

1. At session start, `PersistentProgressionModule.OnSessionStarted()`:
   - Creates or loads the local player's `PlayerProgression` record from `ProgressionStore`.
   - Captures base stats to avoid compounding multiplier drift.
   - Applies existing level bonuses (HP: +2% per level, Speed: +0.5% per level).
2. **Earning XP during gameplay:**
   - **Monster killed** (`OnMonsterKilled`): Awards `KillXP` (default 25). Scaled by difficulty multiplier if `ScaleWithDifficulty` is true. Increments `TotalKills`.
   - **Loot collected** (`OnLootCollected`): Awards `LootCollectedXP` (default 10). Increments `TotalLootCollected`.
   - **Item sold** (`OnItemSold`): Awards `SellXP` (default 15).
   - **Room cleared** (`OnRoomCleared`): Awards `RoomClearedXP` (default 50). Scaled by difficulty. Increments `TotalRoomsCleared`.
   - **Monster loot drop** (`OnMonsterLootDrop`): Awards `MonsterLootDropXP` (default 15). Scaled by difficulty.
   - **Session survived** (`AwardSessionSurvived`): Awards `SessionSurvivedXP` (default 100). Increments `TotalSessionsSurvived`.
3. **Leveling up:**
   - After each XP gain, `LevelTable.ComputeLevel()` recalculates the player's level using the formula: `BaseXPPerLevel` (100) with `XPScalingExponent` (1.5).
   - If the new level exceeds `_previousLevel`, a level-up is triggered.
   - Level boundaries are recached.
   - `ApplyLevelBonuses()` reapplies stat multipliers from `_baseStats` to avoid compounding.
   - Max level is capped at 50.
4. **XP bar HUD:**
   - Displays "Lv.{level}" and a progress bar at the bottom-left of the screen.
   - Bar shows current XP numerically and visually as a fill percentage toward the next level.
5. **Auto-save:**
   - Every 60 seconds (configurable), if `_dirty` is true, progression data is saved to disk via `ProgressionStore.Save()`.
   - Data is also saved on mod shutdown (`OnApplicationQuit`).

### Flow 5: Admin Panel Navigation

1. Player presses **F10** at any time during gameplay.
2. The Admin Panel window appears at position (50, 50) with default Medium size (550x600).
3. The panel header shows "Nemesis Admin Panel".
4. **Size presets:** Player clicks S (400x420), M (550x600), or L (750x800) buttons to resize the window. Font scale adjusts accordingly (0.85x, 1.0x, 1.2x).
5. **Tab navigation:**
   - Host sees 20 tabs rendered as horizontal buttons.
   - Client sees 7 tabs with a "(Client mode)" label.
   - Clicking a tab name highlights it (active style) and renders the corresponding tab content below.
   - If the active tab index exceeds the number of available tabs (e.g., switching from host to client view), it clamps to tab 0.
6. **Tab content:** Each tab renders its module-specific controls (toggles, sliders, value fields) using the `GUIStyles` system.
7. **Bottom buttons:**
   - **"Save Config"**: Saves all settings to disk and triggers a sync push (host only).
   - **"Close [F10]"**: Closes the panel and restores the previous cursor state.
8. The window is **draggable** via `GUI.DragWindow()`.
9. The window has a dark semi-transparent background (RGBA: 0.08, 0.08, 0.12, 0.96).
10. Pressing **F10** again also closes the panel and restores cursor lock.

---

## 3. Settings Propagation Matrix

| # | Module | Config Class | Host/Client | Synced via Lobby Data? | Default Enabled? | Affects Gameplay? | Affects Visuals Only? |
|---|--------|-------------|-------------|----------------------|-----------------|------------------|---------------------|
| 1 | Difficulty | `DifficultyConfig` | Host | Yes | Yes | Yes | No |
| 2 | Roles | `RoleConfig` | Host | Yes | Yes | Yes | No |
| 3 | Damage | `DamageScaleConfig` | Host | Yes | No | Yes | No |
| 4 | Mimics | `MoreMimicsConfig` | Host | Yes | No | Yes | No |
| 5 | Stamina | `StaminaConfig` | Host | Yes | No | Yes | No |
| 6 | Jump | `JumpConfig` | Host | Yes | No | Yes | No |
| 7 | Fullbright | `FullbrightConfig` | Host | Yes | No | Yes (removes darkness) | Partially (visual but gameplay-impacting) |
| 8 | LootDrop | `LootDropConfig` | Host | Yes | No | Yes | No |
| 9 | Voices | `MoreVoicesConfig` | Host | Yes | No | Yes | No |
| 10 | Marker | `MarkerConfig` | Host | Yes | No | Yes | No |
| 11 | Inventory | `InventoryExpansionConfig` | Host | Yes | No | Yes | No |
| 12 | AutoLoot | `AutoLootConfig` | Host | Yes | No | Yes | No |
| 13 | Fly | `FlyConfig` | Host | Yes | No | Yes | No |
| 14 | Radar | `RadarConfig` | Client | No | Yes | No | Yes |
| 15 | Progression | `ProgressionConfig` | Host | Yes | Yes | Yes (stat bonuses) | Partially (HUD + stats) |
| 16 | FOV | `FovConfig` | Client | No | No | No | Yes |
| 17 | VoiceFix | `VoiceFixConfig` | Client | No | No | No | Yes (audio) |
| 18 | Health | `HealthIndicatorsConfig` | Client | No | Yes | No | Yes |
| 19 | Tooltip | `TooltipConfig` | Client | No | Yes | No | Yes |
| 20 | ESP | `EspConfig` | Client | No | No | No | Yes |

**Notes:**
- 14 modules are synced from host to all clients via Steam lobby data (serialized as JSON via `HostConfig` every 2 seconds). Adding a new host config to `HostConfig.cs` automatically syncs it — no manual Apply methods needed.
- 6 modules are client-local and never synced; each player configures them independently.
- "Affects Gameplay" means the module changes game mechanics (stats, spawns, physics, economy). "Affects Visuals Only" means it only changes what the player sees or hears.
- Progression is host-synced because it affects gameplay through per-level stat bonuses (HP +2%/level, Speed +0.5%/level). XP data is stored locally per player.

---

## 4. Balance Considerations

### Dangerous Misconfiguration Scenarios

#### DamageMultiplier set to 0.1
- **Effect:** Players deal only 10% of normal damage. Enemies become extremely tanky. Basic enemies that normally take 2-3 hits could require 20-30 hits.
- **Risk:** Sessions become tediously long. Players may be unable to kill tougher enemies before being overwhelmed. Combined with high difficulty multipliers, this could make the game unwinnable.
- **Recommendation:** Clamp minimum to 0.25 for "hard mode"; never go below 0.5 for casual play.

#### SpawnRateMultiplier set to 5.0
- **Effect:** Five times the normal mimic spawn rate. Areas that normally have 2-3 mimics could have 10-15.
- **Risk:** Severe performance degradation from too many active entities. Players can be instantly swarmed and killed before reacting. Loot economy may break if combined with LootDrop (enemies dropping loot at 5x spawn rate = loot flooding).
- **Recommendation:** Cap at 3.0 for challenging play; 2.0 is the sweet spot for harder sessions without performance issues.

#### InfiniteStamina + Fly + Fullbright all enabled
- **Effect:** Players can fly freely at configurable speed, sprint indefinitely, and see everything regardless of lighting. This removes all core survival mechanics: exploration risk, resource management, darkness tension.
- **Risk:** Completely trivializes the game. No danger from navigation, no stamina management, no darkness-based scares. Essentially turns the game into a walking simulator with optional combat.
- **Recommendation:** This combination is appropriate only for debugging, content creation, or deliberate "sandbox mode." Not suitable for any form of competitive or cooperative challenge play.

### Preset Recommendations

#### "Fair Play" Preset (Balanced Enhancement)
| Module | Setting |
|--------|---------|
| Difficulty | Enabled, default weights |
| Roles | Enabled, default multipliers |
| Damage | Disabled (1.0x) |
| Mimics | Disabled (1.0x) |
| Stamina | Disabled |
| Jump | Disabled |
| Fullbright | Disabled |
| LootDrop | Disabled |
| Voices | Disabled |
| Marker | Disabled |
| Inventory | Disabled |
| AutoLoot | Disabled |
| Fly | Disabled |
| Radar | Enabled, range 30, monsters only |
| Progression | Enabled, default XP values |
| Health | Enabled |
| Tooltip | Enabled |

This preset adds roles and dynamic difficulty for variety while keeping the core survival loop intact. Radar is limited to nearby monsters for situational awareness without trivializing exploration.

#### "Fun Mode" Preset (Casual/Party)
| Module | Setting |
|--------|---------|
| Difficulty | Enabled, MinMultiplier 0.5, MaxMultiplier 1.5 |
| Roles | Enabled, boosted multipliers (Scout 1.3x speed, Tank 1.5x HP) |
| Damage | Enabled, 1.5x multiplier |
| Mimics | Enabled, 2.0x spawn rate |
| Stamina | Enabled (infinite) |
| Jump | Enabled, velocity 5.0 |
| Fullbright | Disabled |
| LootDrop | Enabled, 75% drop chance, 3 max drops |
| Voices | Enabled, 20 max recordings |
| Marker | Enabled, infinite paintballs |
| Inventory | Enabled, +4 slots |
| AutoLoot | Enabled, range 5.0 |
| Fly | Disabled |
| Radar | Enabled, range 50, all entities |
| Progression | Enabled, 2x XP values |
| Health | Enabled, damage numbers on |
| ESP | Enabled, range 30 |

This preset makes the game more action-oriented with more enemies, more loot, more power, and more information. Players feel powerful but still face danger from increased spawns. Fly and Fullbright remain off to preserve some exploration tension.

#### "Sandbox/Debug" Preset
| Module | Setting |
|--------|---------|
| All gameplay modules | Enabled with maximum values |
| Fly | Enabled, speed 20 |
| Fullbright | Enabled, intensity 1.5 |
| Stamina | Enabled |
| ESP | Enabled, max range 200 |
| Damage | 5.0x multiplier |

For mod development, map exploration, and content creation only.

### Additional Balance Interactions to Watch

- **Roles + Damage Scale:** A Scavenger with 1.3x loot multiplier plus LootDrop at 100% chance can flood the economy. Tank with 1.25x HP plus high damage multiplier becomes nearly invincible.
- **Difficulty scaling + Progression:** High difficulty multipliers increase XP gains (when `ScaleWithDifficulty` is true), which accelerates leveling, which grants stat bonuses, which makes high difficulty easier -- creating a feedback loop.
- **AutoLoot + LootDrop + MoreMimics:** Automatic pickup combined with high enemy loot drops and increased spawns can result in inventory filling instantly, especially if Inventory Expansion is not also enabled.
- **Fullbright + ESP:** Both remove information asymmetry. Together they eliminate all visual challenge. The game's horror elements rely heavily on darkness and uncertainty.

---

## 5. Known Limitations

### Role Assignment Retry (Up to 10 Attempts)
- **Issue:** When a session starts, players may not be fully loaded into the game yet. The `RoleSystemModule` calls `TryAssignRoles()` immediately, but if `PlayerAPI.GetAllPlayers()` returns null or the local player is not available, assignment fails.
- **Mitigation:** The module retries every 1 second for up to 10 attempts (`MaxRoleRetries = 10`, `RoleRetryInterval = 1f`).
- **Failure mode:** If all 10 retries are exhausted without successful assignment, roles are not assigned for the session. No error is shown to the user; `_cachedLocalRole` remains `Role.None` and the role HUD is simply not displayed.
- **Impact:** Players join without role bonuses. The session plays as vanilla. This is most likely to occur with very slow connections or when joining mid-session.

### Voice Recording Limit Changes Require Active SpeechEventArchive Instances
- **Issue:** The `MoreVoicesModule` modifies the `MaxRecordings` field on `SpeechEventArchive` instances via reflection. It uses `FindObjectsByType()` to locate active instances at runtime.
- **Mitigation:** If no instances are found, the module retries on the next `OnUpdate()` call (every frame) until instances appear and the setting is applied.
- **Failure mode:** If the `SpeechEventArchive` type or its `MaxRecordings` field cannot be found (e.g., game update changes the class), the module logs a warning and marks itself as applied to avoid spamming. The voice recording limit remains at the game's default.
- **Impact:** The setting only takes effect once the game has instantiated the relevant objects. Changes made in the admin panel while no instances exist will be applied retroactively when instances appear.

### InventoryExpansion Only Bypasses the Full Check, Does Not Add UI Slots
- **Issue:** The `InventoryExpansionModule` works by setting a static `IsEnabled` flag. It is consumed by Harmony patches that bypass the "inventory full" check, allowing players to pick up more items than the UI supports.
- **Limitation:** The module does not add new visual inventory slots to the UI. Items beyond the default slot count are held in the player's logical inventory but may not be visible or manageable through the standard inventory screen.
- **Impact:** Players can carry more items, but managing overflow items requires dropping or selling them without a UI representation. The `AdditionalSlots` config value (default 4) controls how many extra items are allowed past the full check, but the actual inventory UI remains unchanged.

### Jump Module Uses Transform Manipulation, Not Proper Physics
- **Issue:** The `JumpModule` applies upward movement by directly modifying `player.transform.position` (`+= Vector3.up * JumpVelocity * Time.deltaTime * 10f`) rather than applying a force or velocity to a Rigidbody or CharacterController.
- **Limitation:** This approach:
  - Does not interact properly with the game's physics system (no momentum, no arc).
  - May cause clipping through geometry if the jump displacement is large enough in a single frame.
  - Relies on the game's built-in gravity to pull the player back down, which may not produce a smooth parabolic arc.
  - Only triggers when the player is grounded (`player.grounded`), so no double-jumping or mid-air adjustments.
- **Impact:** Jump behavior feels "teleport-y" rather than smooth. High `JumpVelocity` values (e.g., 20+) may cause the player to clip through ceilings or land in unintended areas. The multiplier `* Time.deltaTime * 10f` makes the jump frame-rate dependent in its instantaneous displacement.

### Marker Mod: Only Infinite Paintballs Implemented, Color Cycling and Permanent Marks Are Config-Only
- **Issue:** The `MarkerModule` exposes three config flags: `InfinitePaintballs`, `PermanentMarks`, and `EnableColorCycling`. However, the module implementation only tracks static boolean properties for `IsEnabled` and `InfinitePaintballs`.
- **Limitation:** The `PermanentMarks` and `EnableColorCycling` config options exist in `MarkerConfig` and are synced to clients, but the module code does not contain logic to implement these features. They are placeholder configs for future development.
- **Impact:** Enabling `PermanentMarks` or `EnableColorCycling` in the admin panel has no gameplay effect. Only `InfinitePaintballs` (via the static property consumed by Harmony patches) is functional.

### Config Sync Timing
- **Issue:** Config sync runs on a 2-second polling interval (`SyncInterval = 2f`). Changes made by the host are not instantly reflected on clients.
- **Impact:** There is up to a 2-second delay between a host saving config changes and clients receiving them. During fast-paced gameplay, this lag could cause brief inconsistencies (e.g., a damage multiplier change not applying to a hit that occurs within the sync window).

### Host Detection Delay
- **Issue:** Both `NemesisMod` and `AdminPanel` check host status via `PlayerAPI.GetLocalPlayer().IsHost`. This check may fail early in the session before the local player object is available.
- **Mitigation:** `NemesisMod` rechecks every 5 seconds. `AdminPanel` rechecks each time the panel is opened.
- **Impact:** On the first panel open before the player is fully loaded, host/client status may be misidentified. The user can close and reopen the panel to trigger a recheck.

### Progression Data Is Purely Local
- **Issue:** `PersistentProgressionModule` stores XP and level data in a local file via `ProgressionStore`. There is no server-side validation or anti-cheat.
- **Impact:** Players can manually edit their progression file to set arbitrary levels and XP. The stat bonuses (HP, speed) will be applied accordingly. In a multiplayer context, one player could have significantly higher stats than others by editing their local save.

### Config Save Does Not Validate Ranges
- **Issue:** When the host saves config via the admin panel, values are written as-is without clamping or validation. There are no min/max bounds enforced at the config level.
- **Impact:** It is possible to set nonsensical values (e.g., `DamageMultiplier = -5`, `SpawnRateMultiplier = 1000`, `JumpVelocity = 999`). These values will be synced to clients and may cause crashes, physics glitches, or severe performance issues.
