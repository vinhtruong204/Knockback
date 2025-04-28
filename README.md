# Knockback Multiplayer Demo

> A sample project for a **multiplayer game** using **Unity Netcode for GameObjects**, supporting **LAN** and **Unity Relay Server** connections.

## Introduction

Knockback is a simple 2D shooting game where players can:

- Connect over LAN or via Unity Relay servers.
- Move, shoot, and knock back opponents.
- Test synchronization of characters, bullets, and knockback mechanics over the network.

Gameplay demo: [Video](https://youtu.be/dmKAxPemb70?si=H7UkXSUUtcF_SWoL)<br>

## Main Features

- **LAN** connection (Host/Client over local network).
- **Unity Relay** connection (Internet-based).
- Character spawn, movement, and shooting with proper **network synchronization**.
- **Knockback** effect when hit by bullets.
- **Object Pooling** for bullets (network performance optimization).

## Technologies Used

- **Unity 6000.0.x** `LTS`
- **Netcode for GameObjects** (Unity Multiplayer)
- **Unity Transport**
- **Unity Relay Service**

## How to Setup and Run

### 1. Install Unity

- Recommended Unity version: **6000.0.x LTS**

### 2. Set up Unity Services

- Go to `Window > Services > Multiplayer > Relay`
- Sign in with your Unity ID.
- Enable the Relay service.

### 3. Build and Run

- Build the project normally (`File > Build Settings > Build`).
- Run two instances on the same machine or different machines in the same network or over the internet.

### 4. How to Play

1. Open the game, select **Host** to create a room.
2. Other players select **Client** and input the **Join Code** (for Relay connection).
3. Once all players are ready, the Host clicks **Start Game** to begin.
4. Control your character to move and shoot opponents.

## Controls

| Action | Key               |
| ------ | ----------------- |
| Move   | Left joystick     |
| Shoot  | On-screen Attack Button |
| Jump   | On-screen Jump Button |
| Bomb   | On-screen Bomb Button   |

## Project Structure

```
Assets/
├── Scripts/          // Character movement, shooting, knockback
├── Prefabs/          // Character and bullet prefabs
├── Scenes/           // Lobby and Gameplay scenes
├── Network/          // Netcode scripts, Spawn Handlers, Relay Manager
└── Pooling/          // Bullet Object Pooling system
```

## Screenshots
### LAN UI
![LAN UI](https://github.com/user-attachments/assets/ddd9dc0d-46dc-4a5c-a806-6aced80910eb)

### Online UI
![Online UI](https://github.com/user-attachments/assets/1080a7b8-e151-449c-8832-c4a4254fc817)


### Main game
![Main Game](https://github.com/user-attachments/assets/c4989e17-7554-4ef3-83ea-16976f5d3af9)



## Additional Notes

- **LAN Play** does not require Relay Service.
- **Internet Play** requires Relay Service to be enabled in Unity Services.
