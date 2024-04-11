# Dungeon Delver

## Project Description:
Dungeon Delver is a text-based dungeon crawler that exists in a medieval-fantasy setting.<br>
The core gameplay loop revolves around clearing rooms of enemies and looting to survive.<br>
After clearing 7 rooms, the player will be faced with the final room, and upon completion, will beat the game.<br>

## How to Play:
To begin, the user can run the ```dotnet run``` command in the console to boot up the game.<br>
The user will be presented with a text-based start up interface from which they can begin a new game.<br>
After an introduction, the player will then be tasked with clearing 7 rooms, which are randomly generated to contain an enemy or loot.<br>
Combat is turn-based, with the player moving first and the enemy second.<br>

If the player encounters an enemy, the text-based combat user interface will be displayed which provides a player with three options:<br>
1. Attack
2. Use Consumable
3. Flee<br>

If the player attacks, they have a chance to either damage or miss an enemy.<br>
If the player uses a consumable, they can view their inventory and input which consumable they would like to use.<br>
If the player flees, they will proceed to the next room, but the enemy has a chance to damage them on their way out.<br>

Loot rooms give the player a chance at opening a chest for a random weapon, however it is possible that the chest will contain a trap.
