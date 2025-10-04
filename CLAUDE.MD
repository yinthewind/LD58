# Claude Documentation

This file serves as an index for project documentation to help AI assistants understand the codebase.

## Package Documentation

### Tarodev 2D Controller

**Location:** [docs/tarodev-2d-controller.md](docs/tarodev-2d-controller.md)

Complete guide to the Tarodev 2D platformer controller package, including:
- Architecture overview (PlayerController, ScriptableStats, PlayerAnimator)
- Advanced jump mechanics (coyote time, jump buffering, variable height)
- Configuration parameters and customization guide
- Usage instructions and troubleshooting

**Quick Reference:**
- **Prefab:** `Assets/Tarodev 2D Controller/Prefabs/Player Controller.prefab`
- **Stats Asset:** `Assets/Tarodev 2D Controller/Stat Presets/Player Controller.asset`
- **Demo Scene:** `Assets/Tarodev 2D Controller/Demo/Scene.unity`
- **Default Controls:** WASD/Arrows to move, Space/C to jump

### Inventory & Collection System

**Location:** [docs/creating-collectibles.md](docs/creating-collectibles.md)

Guide for creating collectible items that fly to inventory when player gets close:
- Complete collection system (Collectible, ItemCollector, CollectionTween)
- Flying animation with arc and scale effects
- ItemData ScriptableObject setup
- Step-by-step prefab creation
- Automatic slot detection and stacking

**Quick Reference:**
- **Core Scripts:** `Assets/Scripts/Inventory/`
- **ItemData Template:** Create → Inventory → Item Data
- **Main Scene:** `Assets/Scripts/Inventory/InventoryScene.unity`
- **How it works:** Player touch → Item flies to inventory → Auto-stacks

### Camera Follow System

**Location:** [docs/camera-system.md](docs/camera-system.md)

Deadzone-based camera controller that keeps the player centered with smooth following:
- Deadzone box (camera stays still when player near center)
- Smooth lerp-based camera movement
- Velocity-based lookahead (optional)
- World bounds clamping (optional)
- ScriptableObject presets (Standard, Tight, Loose, Lookahead)
- Visual debug gizmos

**Quick Reference:**
- **Core Scripts:** `Assets/Scripts/Camera/`
- **Presets:** `Assets/Settings/CameraPresets/` (Standard Follow, Tight Follow, Loose Follow, Lookahead Follow)
- **Setup:** Add FollowCamera to Main Camera → Assign player as target → Choose preset
- **How it works:** Player in deadzone → Camera still | Player exits deadzone → Camera smoothly follows

---

## Project Structure

```
LudumDare58/
├── Assets/
│   ├── Tarodev 2D Controller/    # 2D platformer controller package
│   ├── Scripts/
│   │   ├── Camera/               # Camera follow system
│   │   │   ├── FollowCamera.cs       # Main camera controller
│   │   │   └── CameraStatsData.cs    # Camera configuration presets
│   │   ├── Inventory/            # Inventory system and collectibles
│   │   │   ├── InventoryScene.unity  # Main inventory scene
│   │   │   ├── InventorySystem.cs    # Backend inventory logic
│   │   │   ├── InventoryDisplay.cs   # Visual inventory grid
│   │   │   ├── Collectible.cs        # Collectible item component
│   │   │   └── ItemCollector.cs      # Player collection handler
│   │   └── InventoryScene/       # Scene initialization scripts
│   ├── Items/                    # ItemData assets (ScriptableObjects)
│   ├── Scenes/                   # Unity scenes
│   ├── Settings/                 # URP and pipeline settings
│   │   └── CameraPresets/        # Camera preset assets
│   └── ...
├── docs/                         # Project documentation
│   ├── tarodev-2d-controller.md
│   ├── creating-collectibles.md
│   └── camera-system.md
├── ProjectSettings/              # Unity project configuration
└── CLAUDE.MD                     # This file
```

---

## Unity Project Settings

**Input System:** Both (legacy Input Manager + new Input System)
- Changed to support Tarodev controller which uses legacy input APIs
- Setting location: Edit → Project Settings → Player → Other Settings → Active Input Handling

**Render Pipeline:** Universal Render Pipeline (URP)
- Mobile and PC render asset configurations available in `Assets/Settings/`

---

## Git Repository

**Remote:** `git@github.com:yinthewind/LD58.git`
- Both `origin` and `upstream` point to the same repository

**Branches:**
- `master` - Main development branch

---

## Notes for Future Sessions

- The project uses Unity with URP
- The Tarodev 2D Controller is physics-based (Rigidbody2D)
- All controller parameters are configurable via ScriptableStats asset
- Documentation should be kept in the `docs/` folder and referenced here
