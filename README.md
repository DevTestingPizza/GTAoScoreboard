# GTAoScoreboard
**A GTA Online styled scoreboard/playerlist for FiveM.**

I was bored, so I made this. Hate it, love it. I don't care.

### License
No official license, ***HOWEVER***, please only (re-)release actually modified versions of this, not just 1 variable name change and release it. Other than that, feel free to do whatever you want, as long as:
- You credit me.
- You link this github page when you share (a modified version) of this, might as well just fork it and PR it instead if you have some cool ideas.
- You **DON'T** claim it to be yours.

Enjoy.

# Features
- Lists all players + their server ID's.
- You can set it to appear either on the left side, center, or right side of your screen. (Go to the `__resource.lua` file and change `displayType 'left'` to either `'left'`, `'center'` or `'right'`.)
- The list is responsive, meaning it will scale with the player's safezone scale settings & resolution.

# Installation
1. Go to [releases](https://github.com/TomGrobbe/gtaoscoreboard/releases/), download the latest version. Extract the files and put them into your resources folder.
2. Add `start gtaoscoreboard` to your server.cfg file.
3. (Optionally) go to `/releases/gtaoscoreboard/__resource.lua` and edit the `displayType:` (instructions are in there).
4. (re)Start your server.
5. Press `Z` or `DPAD_DOWN` in-game to load the first page. If there's more than 16 players online, pressing the same button again will show the second page. 
Pressing it again will hide the scoreboard. If you don't hide it yourself, then the scoreboard will automatically hide after a couple of seconds.

# Preview
**Of course the `Players Online (1)` will show the accurate online player count, and the players won't all be called `Player #`, that's just for this demo.**

|Page 1|Page 2|
|:-:|:-:|
|![](https://www.vespura.com/hi/i/5881228.png)|![](https://www.vespura.com/hi/i/7a3c1e4.png)|

