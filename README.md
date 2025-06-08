# Boss Window Game

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

## License

This repository does not include a specific license. If you plan to reuse or distribute this project, please contact the original author.
