# Pong Multiplayer Online

## Overview

**Elevator Pitch**  
Una reinterpretazione online del classico Pong, progettata per due giocatori. Questo progetto si propone di realizzare una versione multiplayer giocabile via rete, dove i partecipanti possono sfidarsi in tempo reale. Il gioco include lobby multiplayer, punteggio dinamico e fisica di gioco fedele al Pong originale, con particolare attenzione al principio di *server authority*.

**Style**  
Minimalista e funzionale, il gioco replica l’estetica e le dinamiche essenziali del Pong classico. L’interfaccia è semplice e leggibile, orientata alla reattività e all’accessibilità, con un sistema di rete che garantisce sincronia tra i client.

**Targeting**  
Destinato a sviluppatori e studenti interessati al networking nei giochi multiplayer, oltre ai nostalgici del Pong originale. Utile come base per comprendere lobby online, sincronizzazione client-server e gestione della latenza.

## Table of Contents

- [Key Features](#key-features)  
- [Mechanics](#mechanics)  
- [Installation](#installation-instructions)  
- [Build](#Or-take-the-last-Build)
- [Credits](#credits)

---

## Key Features

- **Lobby Online**  
  Il giocatore host può creare una stanza che il secondo giocatore può raggiungere tramite sistema di invito diretto o join.

- **Controllo delle Racchette**  
  Ogni giocatore controlla una racchetta in tempo reale per colpire la pallina.

- **Fisica della Pallina**  
  La pallina rimbalza su muri e racchette seguendo una fisica semplice ma coerente.

- **Sistema di Goal**  
  Ogni lato dello schermo rappresenta una porta in cui segnare.

- **Punteggio e Vittoria**  
  Il gioco tiene traccia dei goal e termina quando uno dei due giocatori raggiunge un punteggio stabilito.

- **Server Authority**  
  Il server gestisce tutta la logica di gioco per prevenire cheating e desincronizzazioni.

---

## Mechanics

### Networking

- **Lobby System**:  
  Un giocatore crea la partita (host), l’altro si connette (client).

- **Sincronizzazione**:  
  Il server ha piena autorità sulla posizione degli oggetti di gioco (racchette, pallina, punteggio).

- **Input Handling**:  
  I comandi del client vengono inviati al server, che li valida e li applica.

### Gameplay Loop

1. Il giocatore host crea la lobby.
2. Il secondo giocatore si unisce.
3. Entrambi controllano le racchette per colpire la pallina.
4. Ogni goal viene registrato e aggiornato sul server.
5. Al raggiungimento dello score target, viene dichiarato il vincitore.

---
## Installation Instructions

To copy from GitHub and build a Unity project without installing additional files, follow these general steps:

### Clone the Repository:

Open the GitHub repository page containing the Unity project.
Click on the green "Code" button and select "Download ZIP" if you don't have Git installed, or use Git to clone the repository if you have it installed.
If you downloaded the ZIP file, extract its contents to a dedicated folder on your computer.
### Open Unity Hub:

Ensure you have Unity Hub installed on your machine. Unity Hub is a management tool for Unity projects.
Open Unity Hub and click on the "Add" button to add a new project.
### Select the Project:

Navigate to the folder where you extracted or cloned the Unity project.
Select the project folder and click "Open" or "Select Folder."
Install Unity Version (if needed):

If the Unity project requires a specific Unity version that you don't have installed, Unity Hub will prompt you to download and install the required version. Follow the prompts to install it.
### Open the Project:

Once the Unity version is installed, click on the project thumbnail in Unity Hub to open the project.
Resolve Dependencies (if needed):

Unity projects may have dependencies or plugins. Check the project documentation or readme for any specific instructions on resolving dependencies. You might need to import certain assets or packages from the Unity Asset Store.
### Build the Project:

Once the project is open, go to "File" -> "Build Settings."
Choose the target platform for your build (e.g., PC, Mac, Android, iOS).
Click on "Switch Platform" if necessary.
Click on "Build" to generate the executable or project files.
### Run the Project:

After building, you can run the project by executing the generated executable or running it directly from the Unity Editor.
These steps should allow you to copy a Unity project from GitHub, set it up in Unity Hub, and build it without needing to install additional files beyond Unity Hub and the required Unity version. Note that specific projects may have unique requirements, so always check the project documentation for any additional instructions.

## Or take the last Build

### Release

[Download](https://github.com/STRANOstudios/Pong/releases/tag/V0.0.1)

## Credits
### Sviluppo e Programmazione
Andrea Frigerio
