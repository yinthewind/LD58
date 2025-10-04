# Camera Follow System

## Overview

The Camera Follow System is a deadzone-based camera controller that keeps the player character centered on screen while providing smooth, non-jittery camera movement. The camera only moves when the player approaches the edges of an invisible "deadzone" area at the center of the screen.

**Key Features:**
- **Deadzone Box** - Configurable center area where camera stays still
- **Smooth Following** - Lerp-based camera movement for cinematic feel
- **Velocity Lookahead** - Optional forward-looking based on player velocity
- **World Bounds** - Optional clamping to prevent showing areas outside the level
- **ScriptableObject Presets** - Reusable camera configurations
- **Visual Debug Gizmos** - Real-time visualization in Scene view

---

## Quick Start

### Basic Setup (5 minutes)

1. **Add Component to Main Camera:**
   - Select your Main Camera in the scene
   - Add Component → Camera System → Follow Camera
   - Drag your Player GameObject to the "Target" field

2. **Choose a Preset (Recommended):**
   - In the Follow Camera component, assign a Camera Stats preset:
     - [Standard Follow.asset](../Assets/Settings/CameraPresets/Standard%20Follow.asset) - Balanced (default)
     - [Tight Follow.asset](../Assets/Settings/CameraPresets/Tight%20Follow.asset) - Fast-paced platformers
     - [Loose Follow.asset](../Assets/Settings/CameraPresets/Loose%20Follow.asset) - Exploration games
     - [Lookahead Follow.asset](../Assets/Settings/CameraPresets/Lookahead%20Follow.asset) - High-speed movement

3. **Test in Play Mode:**
   - Press Play and move your character
   - Camera should stay still when player is near center
   - Camera smoothly follows when player approaches edges
   - Toggle "Show Debug Gizmos" to see the deadzone box in Scene view

---

## How It Works

### Deadzone Concept

The deadzone is an invisible rectangular area at the center of the screen. When the player is inside this area, the camera doesn't move. When the player exits the deadzone, the camera smoothly follows to bring the player back to the deadzone edge.

```
Screen View:
┌─────────────────────────────┐
│                             │
│      ┌─────────────┐        │  ← Camera viewport
│      │             │        │
│      │      P      │        │  ← Player in deadzone (camera still)
│      │             │        │
│      └─────────────┘        │  ← Deadzone (30% of screen)
│                             │
└─────────────────────────────┘

When player moves right:
┌─────────────────────────────┐
│                             │
│      ┌─────────────┐        │
│      │             │     P  │  ← Player outside deadzone
│      │             │  →→→   │  ← Camera starts moving right
│      └─────────────┘        │
│                             │
└─────────────────────────────┘
```

### Update Flow

The camera updates in `LateUpdate()` to ensure it runs after player movement:

1. **Calculate Target Velocity** (for lookahead)
2. **Convert to Viewport Space** (player position → 0-1 coordinates)
3. **Check Deadzone Boundaries** (is player inside or outside?)
4. **Calculate Delta Movement** (how far to move camera)
5. **Smooth Lerp** (gradually move camera to target position)
6. **Apply World Bounds** (optional clamping)
7. **Update Camera Position**

---

## Configuration

### Camera Stats Data (ScriptableObject)

Create custom camera presets via: **Right-click in Project → Create → Camera → Camera Stats Data**

#### Parameters

| Parameter | Range | Default | Description |
|-----------|-------|---------|-------------|
| **Deadzone Width** | 0.1 - 0.8 | 0.3 | Horizontal deadzone as % of screen width |
| **Deadzone Height** | 0.1 - 0.8 | 0.3 | Vertical deadzone as % of screen height |
| **Smooth Speed** | 1 - 20 | 5 | How quickly camera follows (higher = faster) |
| **Use Velocity Lookahead** | bool | false | Enable forward-looking camera |
| **Lookahead Amount** | 0 - 10 | 2 | Distance to look ahead (world units) |
| **Description** | text | - | Preset description for documentation |

### Manual Override

If no preset is assigned, you can configure settings directly in the Follow Camera component:

- **Deadzone Width** - Same as preset
- **Deadzone Height** - Same as preset
- **Smooth Speed** - Same as preset
- **Use Velocity Lookahead** - Same as preset
- **Lookahead Amount** - Same as preset

### World Bounds (Optional)

Prevent the camera from showing areas outside your level:

1. Check **"Use Bounds"** in Follow Camera component
2. Set **Min Bounds** (bottom-left corner of world)
3. Set **Max Bounds** (top-right corner of world)

Example for a level that's 100 units wide and 50 units tall:
- Min Bounds: `(-50, -25)`
- Max Bounds: `(50, 25)`

---

## Presets Guide

### Standard Follow
**Best for:** General platformers, balanced gameplay

- Deadzone: 0.3 x 0.3 (30% of screen)
- Smooth Speed: 5
- Lookahead: Disabled

**Feel:** Smooth, responsive, minimal camera motion during small movements.

---

### Tight Follow
**Best for:** Fast-paced platformers, precision gameplay

- Deadzone: 0.2 x 0.2 (20% of screen)
- Smooth Speed: 8
- Lookahead: Disabled

**Feel:** Camera closely follows player, responds quickly to direction changes.

---

### Loose Follow
**Best for:** Exploration games, slower-paced gameplay

- Deadzone: 0.4 x 0.4 (40% of screen)
- Smooth Speed: 3
- Lookahead: Disabled

**Feel:** Camera gives player more freedom to move before following, lazy cinematic feel.

---

### Lookahead Follow
**Best for:** High-speed games, runner-style gameplay

- Deadzone: 0.3 x 0.25
- Smooth Speed: 6
- Lookahead: Enabled (3 units)

**Feel:** Camera anticipates movement direction, shows more space ahead of player.

---

## Advanced Usage

### Runtime Camera Control

The Follow Camera exposes a public API for runtime control:

```csharp
using CameraSystem;

// Get reference to camera
FollowCamera cam = Camera.main.GetComponent<FollowCamera>();

// Change target
cam.SetTarget(newPlayerTransform);

// Set world bounds
cam.SetBounds(new Vector2(-100, -50), new Vector2(100, 50));

// Disable bounds
cam.DisableBounds();
```

### Multiple Camera Zones

For levels with different camera requirements, you can:

1. Create multiple camera presets
2. Use triggers to swap presets at runtime
3. Create a zone manager that changes settings based on player position

Example trigger script:
```csharp
using UnityEngine;
using CameraSystem;

public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField] private CameraStatsData zonePreset;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var cam = Camera.main.GetComponent<FollowCamera>();
            // You can create a method to change preset at runtime
        }
    }
}
```

### Debug Visualization

Enable **"Show Debug Gizmos"** in the Follow Camera component to see:

- **Green Wire Box** - Deadzone area (where camera won't move)
- **Red Wire Box** - World bounds (if enabled)
- **Cyan Wire Sphere** - Player position indicator

---

## Integration with Existing Systems

### Tarodev Player Controller

The camera system works seamlessly with the Tarodev 2D Controller:

1. Drag the Player Controller prefab into scene
2. Add Follow Camera to Main Camera
3. Assign Player Controller as target
4. Done!

The camera uses `Transform.position`, so it works with any player controller.

### Fungus Camera Manager

The Follow Camera can coexist with Fungus CameraManager:

- **Follow Camera** - Active during gameplay
- **Fungus CameraManager** - Used for cutscenes and special transitions

To switch between them:
1. Disable FollowCamera component during cutscenes
2. Use Fungus camera commands (FadeToView, PanToPosition)
3. Re-enable FollowCamera when returning to gameplay

---

## Troubleshooting

### Camera is jittery or stuttering
- Move FollowCamera update to `LateUpdate()` (already done)
- Ensure player movement is in `FixedUpdate()` (Tarodev controller already does this)
- Check that Rigidbody2D interpolation is set to "Interpolate" on player

### Camera doesn't follow player
- Verify Target is assigned in Follow Camera component
- Check that camera and target are in same scene
- Ensure Follow Camera component is enabled

### Deadzone feels too small/large
- Adjust Deadzone Width/Height in preset or component
- Smaller values = tighter follow
- Larger values = more freedom before camera moves

### Camera moves too fast/slow
- Adjust Smooth Speed parameter
- Higher values = faster response
- Lower values = smoother, lazier feel

### Camera shows areas outside level
- Enable "Use Bounds" option
- Set Min/Max Bounds to match your level size
- Red gizmo box shows the clamped area

---

## File Locations

**Scripts:**
- [FollowCamera.cs](../Assets/Scripts/Camera/FollowCamera.cs) - Main camera controller
- [CameraStatsData.cs](../Assets/Scripts/Camera/CameraStatsData.cs) - ScriptableObject for presets

**Presets:**
- [Standard Follow.asset](../Assets/Settings/CameraPresets/Standard%20Follow.asset)
- [Tight Follow.asset](../Assets/Settings/CameraPresets/Tight%20Follow.asset)
- [Loose Follow.asset](../Assets/Settings/CameraPresets/Loose%20Follow.asset)
- [Lookahead Follow.asset](../Assets/Settings/CameraPresets/Lookahead%20Follow.asset)

---

## Tips & Best Practices

1. **Start with Standard Follow preset** - Good default for most games
2. **Tune based on playtesting** - Camera feel is highly subjective
3. **Use lookahead sparingly** - Can feel disorienting if too strong
4. **Set world bounds for fixed-size levels** - Prevents showing empty space
5. **Disable bounds for infinite runners** - Let camera scroll forever
6. **Keep deadzone asymmetric if needed** - Wider horizontally for platformers
7. **Test at different resolutions** - Deadzone % stays consistent across aspect ratios

---

## Credits

Camera system designed for LudumDare58 project, following patterns from:
- Tarodev 2D Controller (ScriptableObject configuration)
- Celeste/Hollow Knight deadzone implementation
- Unity Cinemachine concepts (without the dependency)
