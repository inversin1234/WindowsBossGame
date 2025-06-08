# Boss Window Gam

Boss Window Game is a tongue‑in‑cheek Windows Forms mini‑game written in C#.
Your mouse cursor is the target and you must evade a variety of windows for a
full minute. Each of the ten phases introduces new enemy behaviour and faster
movement.

## Requirements

- Windows with **.NET Framework 4.7.2** installed
- Visual Studio 2019 or newer to build the project

## Building

1. Clone this repository.
2. Open `BossWindowGame.csproj` in Visual Studio.
3. Choose **Debug** or **Release** and build the solution.
4. Launch the resulting `BossWindowGame.exe` from the `bin` folder.

The game relies on WinForms and Windows specific APIs, so it must be run on a
Windows machine.

## How to Play

When started, the game minimises all your other windows and displays the
"BOSS" window at the top of the screen. Enemy windows then spawn below it and
try to touch your mouse cursor. You have **three lives** and 60 seconds to
survive.

Enemy types introduced throughout the phases:

- **Chaser** – relentlessly follows the cursor.
- **Fast** – quickly accelerates in a straight line.
- **ZigZag** – oscillates while moving toward you.
- **Rain** – drops from the top of the screen.
- **Laser** – horizontal beams sweeping across the desktop.

A heads‑up display at the bottom of the boss window shows the remaining time,
current phase, lives and score. Losing all lives causes dozens of red "Fail"
windows to appear. If you last until the timer reaches zero, you win!

## Development

The code uses a mix of WinForms controls and direct Win32 calls for actions such
as minimising other windows. Enemy behaviour is implemented in separate classes
inside `MainForm.cs`.

## License

This repository does not currently include a license file. Contact the original
author if you wish to reuse or redistribute the code.
=======
Boss Window Game is a small Windows Forms mini-game created in C#. The boss window moves across the top of the screen while different enemy windows chase your cursor. Survive all ten phases (about 60 seconds) without losing your three lives to win.


![{6E267F06-6869-4F70-B662-C4318EDA5874}](https://github.com/user-attachments/assets/644c90ec-57e6-448f-bd7b-c5ca57e0a101)


## Features

- Written for **.NET Framework 4.7.2** in C# 7.3
- 10 phases with increasing difficulty
- Several enemy types: chaser, fast, zig-zag, rain and laser beams
- Heads-up display showing time, phase, lives and score
- Failure spawns multiple "Fail" windows across the screen

## Building

1. Open `BossWindowGame.csproj` with Visual Studio (tested with VS 2019 or later).
2. Build the project in **Debug** or **Release** configuration.
3. Run the generated `BossWindowGame.exe` from `bin/Debug` or `bin/Release`.

This project targets the classic .NET Framework and needs Windows to run.

## Gameplay

When launched, the game minimizes other windows and starts a 60‑second survival challenge. The main window (the boss) bounces horizontally while enemy windows spawn below it and attempt to catch your mouse cursor. If an enemy touches your cursor, you lose a life. Lose all three lives and the screen floods with failure windows. Survive until the timer ends to win!
