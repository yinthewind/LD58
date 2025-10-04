# Tarodev 2D Controller

## Overview

The Tarodev 2D Controller is a physics-based 2D platformer controller package with polished movement mechanics. It's designed for tight, responsive platformer controls with advanced features like coyote time, jump buffering, and variable jump heights.

**Package Location:** `Assets/Tarodev 2D Controller/`

**Main Prefab:** `Assets/Tarodev 2D Controller/Prefabs/Player Controller.prefab`

---

## Architecture

The package consists of three main components:

### 1. PlayerController.cs - Core Movement Logic

**Location:** [Assets/Tarodev 2D Controller/_Scripts/PlayerController.cs](../Assets/Tarodev%202D%20Controller/_Scripts/PlayerController.cs)

**Key Features:**
- Physics-based movement using **Rigidbody2D** (not transform manipulation)
- Separates input gathering (`Update`) from physics calculations (`FixedUpdate`)
- Configuration-driven via **ScriptableStats** asset
- Exposes events for animation/audio systems to hook into

**Update Loop (Every Frame):**
```csharp
void Update()
{
    _time += Time.deltaTime;
    GatherInput();  // Read player input (WASD/Arrow keys + Space/C for jump)
}
```

**FixedUpdate Loop (Physics Timestep):**
```csharp
void FixedUpdate()
{
    CheckCollisions();   // Ground/ceiling detection using CapsuleCast
    HandleJump();        // Jump mechanics with advanced features
    HandleDirection();   // Horizontal acceleration/deceleration
    HandleGravity();     // Custom gravity system
    ApplyMovement();     // Apply calculated velocity to Rigidbody2D
}
```

---

### 2. ScriptableStats.cs - Configuration System

**Location:** [Assets/Tarodev 2D Controller/_Scripts/ScriptableStats.cs](../Assets/Tarodev%202D%20Controller/_Scripts/ScriptableStats.cs)

**Default Preset:** [Assets/Tarodev 2D Controller/Stat Presets/Player Controller.asset](../Assets/Tarodev%202D%20Controller/Stat%20Presets/Player%20Controller.asset)

All movement parameters are stored in a **ScriptableObject** asset, allowing tweaking without code changes. Just adjust values in the Unity Inspector.

#### Key Parameters

**Input Settings:**
- `SnapInput` (bool, default: true) - Converts analog input to digital for gamepad/keyboard parity
- `VerticalDeadZoneThreshold` (0.3) - Minimum input for vertical actions
- `HorizontalDeadZoneThreshold` (0.1) - Minimum input for horizontal actions

**Movement Settings:**
- `MaxSpeed` (14) - Top horizontal movement speed
- `Acceleration` (120) - How quickly the player gains horizontal speed
- `GroundDeceleration` (60) - How quickly the player stops on ground
- `AirDeceleration` (30) - How quickly the player stops in air (slower for better air control)
- `GroundingForce` (-1.5) - Constant downward force while grounded (helps on slopes)
- `GrounderDistance` (0.05) - Detection distance for ground/ceiling checks

**Jump Settings:**
- `JumpPower` (36) - Initial velocity applied when jumping
- `MaxFallSpeed` (40) - Terminal velocity when falling
- `FallAcceleration` (110) - Gravity strength (how quickly you gain fall speed)
- `JumpEndEarlyGravityModifier` (3) - Extra gravity multiplier when jump is released early
- `CoyoteTime` (0.15s) - Grace period to jump after leaving a ledge
- `JumpBuffer` (0.2s) - Time window to buffer jump input before landing

**Layer Settings:**
- `PlayerLayer` - Set this to the layer your player is on (used to ignore player collider in ground checks)

---

### 3. PlayerAnimator.cs - Visual & Audio Feedback

**Location:** [Assets/Tarodev 2D Controller/_Scripts/PlayerAnimator.cs](../Assets/Tarodev%202D%20Controller/_Scripts/PlayerAnimator.cs)

Handles all visual and audio polish:

**Visual Features:**
- **Sprite Flipping** - Automatically flips sprite based on movement direction
- **Character Tilting** - Leans character when running for dynamic feel
- **Particle Effects:**
  - Jump particles when leaving ground
  - Landing particles (scaled by impact velocity)
  - Movement dust particles that follow the player
  - Dynamic color matching - particles adapt to ground surface color

**Audio Features:**
- Footstep sounds on landing
- Configurable audio clips

**Event-Driven Design:**
- Subscribes to `IPlayerController` events:
  - `Jumped` → Trigger jump animation/particles
  - `GroundedChanged(bool grounded, float impact)` → Landing effects, scale particles by impact

---

## Advanced Jump Features

### Coyote Time

**What it is:** Allows jumping for ~0.15 seconds **after** walking off a ledge.

**Why it matters:** Prevents the frustrating "I pressed jump but nothing happened!" moment when players input slightly late.

**Implementation:** [PlayerController.cs:128](../Assets/Tarodev%202D%20Controller/_Scripts/PlayerController.cs#L128)
```csharp
private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;
```

### Jump Buffering

**What it is:** Remembers jump input for ~0.2 seconds **before** landing.

**Why it matters:** If you press jump while falling, it executes immediately upon landing. Makes the game feel more responsive.

**Implementation:** [PlayerController.cs:127](../Assets/Tarodev%202D%20Controller/_Scripts/PlayerController.cs#L127)
```csharp
private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
```

### Variable Jump Height

**What it is:**
- Short tap = low jump
- Hold jump button = high jump

**How it works:** When the player releases the jump button early while moving upward, extra gravity is applied to cut the jump short.

**Implementation:** [PlayerController.cs:132](../Assets/Tarodev%202D%20Controller/_Scripts/PlayerController.cs#L132)
```csharp
if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0)
    _endedJumpEarly = true;

// In HandleGravity:
if (_endedJumpEarly && _frameVelocity.y > 0)
    inAirGravity *= _stats.JumpEndEarlyGravityModifier; // 3x gravity
```

---

## Key Design Patterns

### Velocity-Based Movement

Instead of setting velocity multiple times per frame, the controller:
1. Modifies `_frameVelocity` throughout `FixedUpdate()`
2. Applies it once at the end via `ApplyMovement()`

This prevents conflicting velocity changes and makes the code easier to reason about.

### Event-Driven Animation

The `PlayerController` doesn't know about visuals or audio. It simply fires events:
```csharp
public event Action<bool, float> GroundedChanged;
public event Action Jumped;
```

The `PlayerAnimator` subscribes to these events and handles all visual/audio feedback. This is a clean separation of concerns.

### Physics-Based Ground Detection

Uses `Physics2D.CapsuleCast` instead of trigger colliders:
```csharp
bool groundHit = Physics2D.CapsuleCast(
    _col.bounds.center,
    _col.size,
    _col.direction,
    0,
    Vector2.down,
    _stats.GrounderDistance,  // Check slightly below
    ~_stats.PlayerLayer       // Ignore player's own collider
);
```

**Benefits:**
- More reliable than OnTriggerEnter/OnCollisionEnter
- Predictable behavior
- Uses layer masks to avoid detecting the player's own collider

---

## How to Use

### Quick Start

1. **Add the player to your scene:**
   - Drag `Assets/Tarodev 2D Controller/Prefabs/Player Controller.prefab` into your scene

2. **Configure the ScriptableStats:**
   - The prefab should already reference `Assets/Tarodev 2D Controller/Stat Presets/Player Controller.asset`
   - If not, assign it in the Inspector under the PlayerController component

3. **Set up your environment:**
   - Ensure ground objects have a **2D collider** (BoxCollider2D, etc.)
   - Set appropriate **layers** (the player layer should be different from ground layer)
   - Update the `PlayerLayer` field in the ScriptableStats asset to match your player's layer

4. **Input configuration:**
   - The controller uses legacy Input Manager by default
   - Default controls:
     - **Move:** Arrow Keys or WASD (Horizontal/Vertical axes)
     - **Jump:** Space bar (Jump button) or C key
   - To change controls, modify `GatherInput()` in PlayerController.cs or update Input Manager settings

### Demo Scene

**Location:** `Assets/Tarodev 2D Controller/Demo/Scene.unity`

Open this scene to see a working example with platforms and the player controller set up.

---

## Customization Guide

### Adjusting Movement Feel

All movement parameters can be tweaked in the **Player Controller.asset** ScriptableStats:

**For faster/snappier movement:**
- Increase `Acceleration`
- Increase `GroundDeceleration`
- Decrease `AirDeceleration` (for better air control)

**For floatier jumps:**
- Decrease `FallAcceleration` (weaker gravity)
- Decrease `JumpEndEarlyGravityModifier`
- Increase `CoyoteTime`

**For more responsive controls:**
- Increase `JumpBuffer`
- Increase `CoyoteTime`
- Set `SnapInput` to true

### Creating Custom Stats Presets

1. Right-click in Project window
2. Create → Scriptable Stats
3. Name it (e.g., "Heavy Character Stats")
4. Configure values for different character types
5. Assign to PlayerController component

This allows having multiple character types with different movement feels without duplicating code.

---

## Troubleshooting

### Player falls through ground
- Check that ground has a 2D collider
- Ensure player's Rigidbody2D is set to **Dynamic** (not Kinematic)
- Verify `GrounderDistance` in ScriptableStats isn't too small

### Jump not working
- Verify Input Manager has a "Jump" button defined (Edit → Project Settings → Input Manager)
- Check that `JumpPower` in ScriptableStats is set (try 36)
- Ensure player has required components (Rigidbody2D, CapsuleCollider2D)

### Player sliding on slopes
- Increase `GroundingForce` (make it more negative, e.g., -3)
- Add a Physics Material 2D with friction to the player's collider

### Controls feel unresponsive
- Increase `CoyoteTime` and `JumpBuffer`
- Set `SnapInput` to true
- Increase `Acceleration` for faster movement response

---

## Component Requirements

The `PlayerController` script requires these components on the same GameObject:
- **Rigidbody2D** - Physics simulation
- **CapsuleCollider2D** (or any Collider2D) - For collision detection

The `PlayerAnimator` script requires:
- **Animator** - For animation state machine
- **SpriteRenderer** - For sprite flipping
- **AudioSource** - For footstep sounds
- **ParticleSystem** components - For visual effects

The prefab includes all of these pre-configured.

---

## Credits

Created by Tarodev. This is the free version of the controller.

- Premium version: https://www.patreon.com/tarodev
- Play demo: https://tarodev.itch.io/extended-ultimate-2d-controller
- Discord: https://discord.gg/tarodev

---

## Related Files

- [PlayerController.cs](../Assets/Tarodev%202D%20Controller/_Scripts/PlayerController.cs) - Core movement logic
- [ScriptableStats.cs](../Assets/Tarodev%202D%20Controller/_Scripts/ScriptableStats.cs) - Configuration system
- [PlayerAnimator.cs](../Assets/Tarodev%202D%20Controller/_Scripts/PlayerAnimator.cs) - Visual/audio feedback
- [Player Controller.prefab](../Assets/Tarodev%202D%20Controller/Prefabs/Player%20Controller.prefab) - Ready-to-use prefab
