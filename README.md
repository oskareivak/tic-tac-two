# Tic-Tac-Two — Cross-Interface Multiplayer Strategy Game (C# / .NET)

This project implements a **Tic-Tac-TWO** style game, which expands on classic Tic-Tac-Toe.  
The twist is that the **winning condition is evaluated inside a movable “win window”** on the board, adding strategy and depth.

The game can be played:
- In a **web interface** (Razor Pages)
- In a **console application**

Both interfaces share the **same game logic and data**, so you can start a game in one interface and continue it in the other.

---

### Features
- **Console and Web UI** built on top of the same core logic layer
- **Game state storage options**:
  - SQL database (EF)
  - JSON file storage  
  *(switchable with a single configuration change)*
- **LAN multiplayer** (players can join each other's games on the same network)
- **Multiple gamemodes**:
  - Player vs Player
  - Player vs AI
  - AI vs AI
- **Customizable game configurations**, including:
  - `BoardSize` (overall board dimensions)
  - `GridSize` (size of the movable “win window”)
  - `WinCondition` (how many marks are needed to win)
  - `WhoStarts` (which player starts the game)
  - `MovePieceAfterNMoves` (how many moves must be made until extra rules unlock)
  - `NumberOfPiecesPerPlayer` (self explanatory)
- **Pieces can be moved**, not only placed — allowing strategic repositioning once the board is full
- Written in **C#** with **.NET** and **Razor Pages**

---

### Architectural Notes
- Follows separation of concerns:
  - Core game logic is independent of UI frameworks
  - Database layer shared across UIs
- Demonstrates use of **interfaces** to reuse logic
  
---

### Technologies Used
- C# / .NET 9
- Razor Pages
- Entity Framework (SQL)
- JSON serialization
- Console & Web front-ends
