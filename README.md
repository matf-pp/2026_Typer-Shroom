# Typer Shroom

A typing game built with C# and MonoGame where bugs crawl toward your mushroom, and you must type their words before they reach it.

## Gameplay

Waves of bugs march across the screen toward a mushroom on the right. Each bug carries a word above it. Start typing any word to lock onto that bug and kill it before it reaches the mushroom. If a bug makes it through, the mushroom takes damage. You have **3 lives**.

Each wave adds more bugs and increases spawn/movement speed, so later waves require fast and accurate typing.

### Controls

| Key | Action |
|-----|--------|
| Letters | Type to target and kill bugs |
| `ESC` | Quit from main menu or mid-game |
| `SPACE` | View high scores (from main menu) |
| `Enter` | Confirm name entry |

## Features

- 6 animated bug types: spider, ant, fly, mosquito, worm, butterfly
- Mushroom with 4 progressive damage states and hit flash
- Splash death animation when a bug is killed
- Background music + mistype, squash, and eat sound effects
- Wave cleared notification between waves
- Lives displayed as hearts in the HUD
- Game over → name entry → result screen (top 5) → main menu
- Local high scores (top 10) saved to `scores.json`

## Project Structure

```
TyperShroom.Core/    — Game logic (GameEngine, Bug, GameState, WordManager)
TyperShroom.UI/      — MonoGame frontend, screens, rendering
TyperShroom.Data/    — Score persistence (ScoreRepository → scores.json)
TyperShroom.Tests/   — Console test runner
```

## Running the Game

### Linux
```bash
chmod +x TyperShroom.UI
./TyperShroom.UI
```

### Windows
Run `TyperShroom.UI.exe` from the publish folder.

## Authors

- Dimitrije Vujko
- Mihajlo Tasić
- Sreten Milekić

