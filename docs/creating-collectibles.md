# Creating Collectible Items

This guide shows you how to create collectible items that fly to your inventory when the player gets close.

## System Overview

The collection system is **already fully implemented** with these components:

- **Collectible.cs** - Detects player and triggers collection
- **ItemCollector.cs** - Manages the collection process and animation
- **CollectionTween.cs** - Animates items flying to inventory with arc effect
- **ItemData.cs** - ScriptableObject defining item properties

## Step 1: Create Item Sprites (Optional)

For now, you can use the existing placeholder sprites:
- `Assets/Tarodev 2D Controller/Sprites/Circle.png`
- `Assets/Tarodev 2D Controller/Demo/Square.png`

Or import your own 2D sprites into `Assets/Items/Sprites/`

## Step 2: Create ItemData Assets

1. In Unity Project window, navigate to `Assets/Items/`
2. Right-click → **Create → Inventory → Item Data**
3. Name it (e.g., "RedGem")
4. In Inspector, configure:
   - **Item Name**: "Red Gem"
   - **Icon**: Drag a sprite (e.g., Circle.png)
   - **Is Stackable**: ✓ (if you want multiple to stack)
   - **Max Stack Size**: 99 (if stackable)
   - **Description**: "A shiny red gem"

**Create at least 3-5 different items** for variety.

### Example Items:
- **Red Gem** - Stackable, Max 99
- **Blue Potion** - Stackable, Max 10
- **Gold Coin** - Stackable, Max 999
- **Magic Sword** - Not stackable
- **Green Herb** - Stackable, Max 50

## Step 3: Create Collectible Prefab

1. In Hierarchy, create empty GameObject, name it "Collectible_Template"
2. Add components:
   - **SpriteRenderer** (will auto-update from ItemData)
   - **CircleCollider2D** (or BoxCollider2D)
     - Set **Is Trigger**: ✓
     - Adjust size to match sprite
   - **Collectible** script
3. In Collectible component:
   - **Item Data**: Leave empty (we'll override per instance)
   - **Collect Radius**: 1.0
   - **Auto Collect On Trigger**: ✓
4. Set **Tag** to "Collectible" (optional)
5. Drag to Project window to create prefab → `Assets/Prefabs/Collectible_Template.prefab`
6. Delete from Hierarchy

## Step 4: Add ItemCollector to Player

1. Select **Hero** GameObject in InventoryScene
2. Add Component → **Item Collector**
3. Settings:
   - **Inventory Display**: Leave empty (auto-finds at runtime)
   - **Collect Sound**: Optional - assign audio clip
4. Make sure Hero has **Tag "Player"** (required for Collectible detection)

## Step 5: Place Collectibles in Scene

1. Drag `Collectible_Template` prefab into scene
2. Position it where you want
3. Select the instance in Hierarchy
4. In Inspector, **Collectible** component:
   - **Item Data**: Assign one of your ItemData assets (e.g., RedGem)
5. The sprite will automatically update to match the item's icon!
6. Repeat for multiple items with different ItemData

**Pro Tip**: Create variants of the prefab for each item type to save time.

## Step 6: Test!

1. Play the scene
2. Move Hero near a collectible
3. Watch it fly to the inventory with a beautiful arc!
4. It will automatically:
   - Find the next available slot
   - Stack if the item is stackable
   - Animate with arc and scale effect
   - Disappear after reaching the slot

## How It Works

```
Player touches Collectible (trigger)
    ↓
Collectible calls ItemCollector.CollectItem()
    ↓
ItemCollector adds item to InventorySystem
    ↓
Finds which slot the item was added to
    ↓
Gets slot world position from InventoryDisplay
    ↓
Adds CollectionTween component to item
    ↓
Item animates to slot (arc + scale)
    ↓
Item destroyed when animation completes
```

## Customization

### Collectible Settings
- **Collect Radius**: Detection range (shows yellow circle in editor)
- **Auto Collect On Trigger**: Disable for manual pickup

### CollectionTween Animation
Edit the CollectionTween component on items during animation:
- **Duration**: 0.6s (how fast it flies)
- **Arc Height**: 2.0 (how high the arc)
- **Curve**: EaseInOut (animation curve)

### ItemData Properties
- **Is Stackable**: Items with same type combine into one slot
- **Max Stack Size**: Maximum quantity per slot
- **Description**: For tooltips (not implemented yet)

## Troubleshooting

**Items not collecting:**
- Ensure Hero has Tag "Player"
- Ensure Collectible has trigger collider
- Check ItemCollector component is on Hero

**Items not animating:**
- Ensure InventoryDisplay exists in scene
- Check ItemCollector can find InventoryDisplay

**Items disappear immediately:**
- Check if InventoryDisplay is positioned correctly
- Verify CollectionTween duration > 0
