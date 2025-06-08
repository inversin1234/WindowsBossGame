# Boss Window Game

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
