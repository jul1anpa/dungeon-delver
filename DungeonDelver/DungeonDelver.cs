using System;
using System.Collections.Generic;

namespace DungeonDelver 
{

    /// <summary>
    /// Represents the game state manager.
    /// </summary>
    public class GameManager 
    {
        Player player;
        bool playerAlive;
        int roomsCompleted; 
        bool finalBossDefeated;

        /// <summary>
        /// Initializes a new game.
        /// </summary>
        public void NewGame()
        {
            playerAlive = true; 
            roomsCompleted = 0; 
            finalBossDefeated = false;

            player = OutputIntroduction(); 

            while (playerAlive && roomsCompleted < 7) 
            {
                OutputNewRoom(player); 
                roomsCompleted++; 
            }

            OutputBossRoom(); 

            if (playerAlive) 
            { 
                OutputEnding();
            } 
            else 
            {
                OutputDeath();
            }
        }

        /// <summary>
        /// Outputs text-based introduction, creates a new Player instance, and a starter Weapon instance.
        /// </summary>
        /// <returns>An instance of a player.</returns>
        private Player OutputIntroduction()
        {
            Console.WriteLine("—————————————————————————————————————————————————————————————————————————————————————————————————————————");
            Console.WriteLine("You awaken... You cannot recall any memories of the past and carry no longing for the future.\nThe ground beneath you is cold to the touch and a piercing white light washes over you.\nIt emanates from beyond the corridors of this room.");
            Console.WriteLine("\nYou lie there in silence..\nTime is an abstract concept to you, moving neither forward nor backward.\nYou try to remember your name...");
            Console.WriteLine("—————————————————————————————————————————————————————————————————————————————————————————————————————————");

            Console.Write("\nYour name: ");
            string name = Console.ReadLine();

            player = new Player(name);

            Console.WriteLine("\n—————————————————————————————————————————————————————————————————————————————————————————————————————————");
            Console.WriteLine($"You decide the name {name} is befitting enough.\nYou gather your senses and whatever courage that resides within you.\nYour body feels frail as you struggle to get to your feet.\n\nAs you look around the room you notice something glimmering on the ground directly in front of you.");
            Console.WriteLine("You reach for it.");

            Weapon starterWeapon = new Weapon();
            player.EquipWeapon(starterWeapon);

            Console.WriteLine($"You manage to grasp a {starterWeapon.Rarity} {starterWeapon.WeaponName} with what little strength you have left.\nYou flail it about, getting acquainted with it's weight. The motions begin to feel eerily natural.");
            Console.WriteLine("\nThere is only one way out of this place you awoke in..\nand that is through the wall of light that shines before you.\nYou step forth...");
            Console.WriteLine("—————————————————————————————————————————————————————————————————————————————————————————————————————————");

            return player;
        }

        /// <summary>
        /// Outputs user interface for a new room, creates an instance of a DungeonRoom, and handles output logic for the combat user interface.
        /// </summary>
        /// <param name="player">Passes the Player instance to be handled in room generation.</param>
        private void OutputNewRoom(Player player)
        {
            Console.WriteLine($"\n—————————————————————————————————————————————————————————————————————————————————————————————————————————\nEntering room {roomsCompleted + 1}...\n—————————————————————————————————————————————————————————————————————————————————————————————————————————\n");

            DungeonRoom room = new DungeonRoom();
            room.GenerateRoom(player);

            if (room.enemy != null)
            {
                OutputCombatLog(room.enemy);
            } 
        }

        /// <summary>
        /// Outputs user interface for a boss room, creates an instance of a DungeonRoom, and calls combat log output.
        /// </summary>
        private void OutputBossRoom()
        {
            Console.WriteLine($"\n—————————————————————————————————————————————————————————————————————————————————————————————————————————\nEntering the final room...\n—————————————————————————————————————————————————————————————————————————————————————————————————————————\n");
  
            DungeonRoom bossRoom = new DungeonRoom();
            bossRoom.GenerateBossRoom();

            OutputCombatLog(bossRoom.boss);

            if (bossRoom.boss.Health <= 0)
            {
                finalBossDefeated = true;
            }
        }

        /// <summary>
        /// Outputs the user interface for combat, handles combat logic, and enemy actions. 
        /// </summary>
        /// <param name="enemy">Passes an Enemy instance to be processed in combat logic.</param>
        private void OutputCombatLog(Enemy enemy)
        {
            int turn = 1;
            string actions = "\n:::::::::::::::::::\n1. Attack\n2. Use Consumable\n3. Flee\n:::::::::::::::::::";

            while (player.Health > 0 && enemy.Health > 0) // Loops while Player and Enemy health are both above 0.
            {
                // Outputs combat interface.
                Console.WriteLine($"==================\n      Turn {turn}\n==================");
                Console.WriteLine($"\n      HP: {player.Health}");
                Console.WriteLine($"   Enemy HP: {enemy.Health}");
                Console.WriteLine(actions);

                bool validAction = false;
                bool itemUsed = false; // Tracks whether an item was used on the current turn.

                while (!validAction) // Loops until Player performs a valid action.
                {
                    Console.Write("\nYou choose to... \n");
                    string playerInput = Console.ReadLine().Trim();
                    string itemName = "";

                    if (int.TryParse(playerInput, out int playerAction))
                    {
                        switch (playerAction) 
                        {
                            case 1: // Handles Attack command logic.
                                player.Attack(enemy);
                                validAction = true;
                                break;

                            case 2: // Handles Use Consumable command logic.
                                player.OpenInventory();
                                bool validChoice = false; // Tracks whether a user has inputted a valid command.
                                while (!validChoice)
                                {
                                    Console.WriteLine("Enter the name of the item you would like to consume or X to return to combat: ");
                                    itemName = Console.ReadLine().Trim().ToLower();

                                    if (itemName == "x") // Exits Use Consumable command if player inputs X.
                                    {
                                        Console.WriteLine("\nYou fumble through your inventory to no avail..");
                                        break;
                                    }

                                    itemUsed = player.UseConsumable(itemName); // Returns whether a consumable was used or not.
                                    
                                    if (itemName == "x")
                                    {
                                        validChoice = true;
                                    }
                                    else
                                    {
                                        validChoice = itemUsed; // Stores a boolean to determine if the player successfully performed a Use Consumable action.
                                    }
                                }
                                validAction = true;
                                break;

                            case 3: // Handles Flee command logic.
                                Console.WriteLine($"\nYour courage leaves you and you flee from battle!\nYou hastily scamper for the door..\n\n>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>\n\nThe {enemy.EnemyName} manages to swipe a final blow on your way out..");

                                enemy.Attack(player);

                                validAction = true;

                                if (player.Health <= 0) // Checks if a player was killed while fleeing.
                                {
                                    playerAlive = false;
                                }
                                return;

                            default: // Handles invalid integer command logic.
                                Console.WriteLine($"The {enemy.EnemyName} stares as you decide what to do...");
                                break;
                        }
                    } 
                    else // Handles invalid data type command logic.
                    {
                        Console.WriteLine("Faint whispers echo within your ears, they beckon you to make a decision...");
                    }
                }

                if (enemy.Health >= 0 && !itemUsed) // Allows an enemy to attack if their health is above 0 and the Player did not use a consumable on their turn.
                {
                    enemy.Attack(player);
                    turn++;
                }
                else if (itemUsed)
                {
                    continue;
                }
            }

            if (player.Health <= 0) // Checks if Player health is still above 0.
            {
                playerAlive = false;
            } 
            else if (enemy.Health <= 0) // Handles Enemy death text and loot logic.
            {
                Consumable droppedLoot = enemy.DropLoot();

                Console.WriteLine($"The {enemy.EnemyName} perished...");
                Console.WriteLine($"You search it's corpse and find a {droppedLoot.ConsumableName}");

                player.PickupItem(droppedLoot);
            }
        }

        /// <summary>
        /// Outputs the ending text.
        /// </summary>
        private void OutputEnding()
        {
            if (finalBossDefeated)
            {
                Console.WriteLine("You leave a trail of corpses behind you.. but to what end?\nYou are filled with despair, knowing there is no way out of this hell.\nYou resign yourself to your fate...");
                Console.WriteLine("\nT H E   E N D .\n");
            }
            else
            {
                Console.WriteLine("You attempt to flee but find yourself surrounded on all sides by the forces of evil..\nYou see red.. And know only pain as they rip you limb from limb..\nCowards have no place here...");
                Console.WriteLine("\nT H E   E N D .\n");
            }
        }

        /// <summary>
        /// Outputs death text and handles new game logic.
        /// </summary>
        private void OutputDeath()
        {
            Console.WriteLine("You feel your spirit leave your body..\nYou fall to your knees and accept your fate...\n");
            Console.WriteLine("Y O U   D I E D .\n");
            Console.WriteLine("Play again?\n1. Yes\n2. No");
            string startAgain = Console.ReadLine().Trim();

            if (int.TryParse(startAgain, out int action))
            {
                switch (action) 
                {
                    case 1: // Starts a new game.
                        this.NewGame();
                        break;

                    default: // Ends game session.
                        return;
                }
            }
        }
    }

    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player 
    {
        static Random random = new Random();
        public string Name { get; private set; }
        public int Health { get; set; }
        public Weapon equippedWeapon = null;

        List<Consumable> inventory = new List<Consumable>();

        /// <summary>
        /// Constructs a Player instance with a name parameter and initializes Health to 100.
        /// </summary>
        /// <param name="name">Set to the Name field.</param>
        public Player(string name)
        {
            Name = name;
            Health = 100;
        }

        /// <summary>
        /// Handles Attack command logic.
        /// </summary>
        /// <param name="enemy">Passes an Enemy instance to be processed.</param>
        /// <returns>Boolean representing whether an attack successfully hit or not.</returns>
        public bool Attack(Enemy enemy)
        {
            int hitChance = random.Next(1, 101);
            bool hit = hitChance <= 90; // 90% chance for a Player attack to hit.

            if (hit) 
            {
                int damageDealt = equippedWeapon.CalculateDamage(); // Calculates attack damage.
                enemy.TakeDamage(damageDealt);

                Console.WriteLine("\n>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine($"\nYou hit {enemy.EnemyName} for {damageDealt} damage!");
            } 
            else 
            {
                Console.WriteLine("\n>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine("\nYour attack missed..");
            }

            return hit;
        }

        /// <summary>
        /// Handles damage dealt to Player.
        /// </summary>
        /// <param name="damage">The amount of damage to be dealt.</param>
        public void TakeDamage(int damage)
        {
            this.Health -= damage;
        }

        /// <summary>
        /// Handles logic for adding Consumables to Player inventory.
        /// </summary>
        /// <param name="pickedupItem">The Consumable instance to be added.</param>
        public void PickupItem(Consumable pickedupItem)
        {
            foreach (Consumable item in inventory)
            {
                if (pickedupItem.ConsumableName == item.ConsumableName) // If Consumable exists in inventory, increment it's Quantity.
                {
                    item.Quantity += 1;
                    return;
                }
            }

            inventory.Add(pickedupItem); // If item does not exist in inventory, it is added.
        }

        /// <summary>
        /// Handles weapon equip logic.
        /// </summary>
        /// <param name="weapon">Weapon instance to be equipped.</param>
        public void EquipWeapon(Weapon weapon)
        {
            equippedWeapon = weapon;
        }

        /// <summary>
        /// Handles inventory access logic.
        /// </summary>
        public void OpenInventory()
        {
            Console.WriteLine($"\n===========Inventory===========\n");

            foreach (Consumable item in inventory)
            {
                Console.WriteLine($"x{item.Quantity} {item.ConsumableName}");
            }

            Console.WriteLine ("\n===============================\n");
        }

        /// <summary>
        /// Handles Use Consumable command logic.
        /// </summary>
        /// <param name="consumableName">User input that was passed to the Use Consumable command.</param>
        /// <returns>Boolean representing whether a Consumable was used.</returns>
        public bool UseConsumable(string consumableName)
        {
            bool itemUsed = false;

            foreach (Consumable item in inventory)
            {
                if (consumableName == item.ConsumableName.ToLower()) // If user input matches an existing Consumable, the Consumable is used.
                {
                    itemUsed = item.Consume(this);

                    Console.WriteLine($"The {item.ConsumableName} restored {item.RestorePoints} health points!\n");

                    if (item.Quantity <= 0) // If the Quantity of the Consumable is less than or equal to 0, it is removed from the inventory.
                    {
                        inventory.Remove(item);
                    }

                    return itemUsed;
                }
            }

            return itemUsed;
        }
    }

    /// <summary>
    /// Represents a dungeon room in the game.
    /// </summary>
    public class DungeonRoom
    {
        static Random random = new Random();
        string[] enemyRoomDescriptions = new string[] // Stores each text-based room description.
            {"The walls of the room are reflective, almost as if made of glass..\nA ray of white light penetrates the room from a gaping hole in the ceiling.\nThis light reflects every which way, creating abtract patterns of light in the air.",
            "The walls of the room are lined with green hellish flames that spew from several sconces.\nGreat masses of bone protrude from the ground..\ntheir shadows resemble lost souls grasping out above for some sort of savior.",
            "A thick mist fills the room, obscuring vision beyond a few feet.\nFaint whispers echo off the walls, carrying fragmented memories and forgotten secrets.\nShapes loom in the haze.. flickering in and out of existence like specters from another realm.",
            "The room is a surreal dreamscape, with shifting walls of iridescent colors and swirling patterns.\nTime seems to warp and bend, creating a sense of disorientation.\nStrange creatures made of pure energy flit in and out of existence..\nleaving trails of shimmering light in their wake.",
            "The room appears to be frozen in time..\nwith delicate icicles hanging from the ceiling and frost coating every surface.\nA soft, blue light emanates from within the ice, casting an ethereal glow.\nFrozen in mid-motion, statues of ancient beings stand sentinel..\ntheir faces etched with expressions of sorrow and longing."
            };
        public Enemy enemy = null;
        public Enemy boss = null;
        public Weapon loot = null;

        /// <summary>
        /// Calculates whether a room will be of the ENEMY or LOOT type.
        /// </summary>
        /// <returns>String representation of room type.</returns>
        public string CalculateRoomChance()
        {
            int roomChance = random.Next(1, 101);

            if (1 <= roomChance && roomChance <= 80) // 80% chance for ENEMY type room.
            {
                return "ENEMY";
            } 
            else // 20% chance for LOOT type room.
            {
                return "LOOT";
            }
        }

        /// <summary>
        /// Determines room type, populates room based on type, and outputs text for Enemy approach or Loot actions.
        /// </summary>
        /// <param name="player">Passes a Player instance to be processed in room generation.</param>
        public void GenerateRoom(Player player)
        {
            string roomType = CalculateRoomChance();

            switch (roomType) 
            {
                case "ENEMY": // Handles population and text output for ENEMY room type.
                    Console.WriteLine(enemyRoomDescriptions[random.Next(enemyRoomDescriptions.Length)]); // Outputs random room description.

                    enemy = new Enemy(); // Initializes Enemy.
                    enemy.GenerateMinion();

                    Console.WriteLine($"\nA {enemy.EnemyName} appears before you...\n");
                    break;

                case "LOOT": // Handles population and text output for LOOT room type.
                    Console.WriteLine("Within granite walls, a steel chest sits atop a pedestal emitting a soft golden light.\n..a solitary enigma in a barren chamber.");
                    Console.WriteLine("\nDo you approach the chest and attempt to open it?\n1. Open it\n2. Move on");
                    string input = Console.ReadLine().Trim();

                    if (input == "1") // Handles Open it command logic.
                    {
                        bool isTrap = CalculateTrap(player);

                        if (!isTrap) 
                        {
                            Weapon newWeapon = new Weapon();

                            Console.WriteLine($"You reach into the light, pulling out a {newWeapon.Rarity} {newWeapon.WeaponName}!");
                            Console.WriteLine($"\nWould you like to equip it?\n1. Equip {newWeapon.WeaponName}\n2. Leave it");
                            string equipChoice = Console.ReadLine().Trim();

                            if (equipChoice == "1") // Handles Equip command logic.
                            {
                                Console.WriteLine($"\nYou equipped the {newWeapon.WeaponName}!");

                                player.EquipWeapon(newWeapon);
                                break;
                            } 
                            else // Handles Leave it command logic.
                            {
                                Console.WriteLine("You choose to leave it behind...");
                                break;
                            }
                        }
                        break;
                    }
                    else // Handles Move on command logic.
                    {
                        Console.WriteLine("You decide to keep pushing forward..");
                        break;
                    }
                default: // Handles logic for room type that is not ENEMY or LOOT.
                    Console.WriteLine("ERROR: Unknown room type...");
                    break;
            }
        }

        /// <summary>
        /// Populates room with boss Enemy and handles text output.
        /// </summary>
        public void GenerateBossRoom()
        {
            boss = new Enemy();
            boss.GenerateBoss();

            Console.WriteLine("As you step into the final chamber, the air crackles with an electrifying energy.\nDark clouds swirl overhead, illuminated by flashes of lightning that streak across the sky..\nThe ground trembles beneath your feet as if the very earth is anticipating the imminent confrontation...");
            Console.WriteLine($"\n{boss.EnemyName} appears before you...\n");
        }

        /// <summary>
        /// Determines whether a chest is trapped or not.
        /// </summary>
        /// <param name="player">Passes a Player instance to handle damage logic.</param>
        /// <returns>Boolean representing whether or not a chest is a trap.</returns>
        public bool CalculateTrap(Player player)
        {
            int trapChance = random.Next(1, 101);
            int trapDamage = random.Next(10, 40);
            bool isTrap;

            if (1 <= trapChance && trapChance <= 75) // 75% chance for chest to not be a trap.
            {
                isTrap = false;

                Console.WriteLine("\nYou unlatch the locking mechanism of the chest and lift the lid..\nA golden light eminates from within the chest and blinds you for a brief moment..");

                return isTrap;
            } 
            else // 25% chance for chest to be a trap.
            {
                isTrap = true;

                player.TakeDamage(trapDamage);

                Console.WriteLine("You unlatch the locking mechanism of the chest and lift the lid..\nthe golden light dissipates, you hear a faint clicking sound and then..\nWHOOOSH!!!\nthe light bursts forth in a frenzy, it leaves you burned..");
                Console.WriteLine($"It was a trap! The chest damaged you for {trapDamage} damage!");

                return isTrap;
            }
        }
    }

    /// <summary>
    /// Represents an enemy in the game.
    /// </summary>
    public class Enemy
    {
        static Random random = new Random();
        string enemyType;
        public string EnemyName { get; private set; }
        public int Health { get; private set; }
        string[] minionNames = new string[] // Stores names of Minion enemy types.
        {"SKELETAL WARRIOR", "VAMPIRIC SEDUCTRESS", "UNDEAD HUMAN", "RADIATING ORB OF LIGHT", "RUSTED GOLEM"};
        string[] bossNames = new string[] // Stores names of Boss enemy types.
        {"TADZIU THE DEMON KING", "KREE THE ICE WIZARD", "MURTOG THE TOAD"};
        Consumable loot = new Consumable(); // Constructs Consumable to be stored as loot on Enemy.

        /// <summary>
        /// Initializes fields for Minion enemy type.
        /// </summary>
        public void GenerateMinion()
        {
            enemyType = "MINION";
            Health = 100;
            EnemyName = minionNames[random.Next(minionNames.Length)];
        }

        /// <summary>
        /// Initializes fields for Boss enemy type.
        /// </summary>
        public void GenerateBoss()
        {
            enemyType = "BOSS";
            Health = 300;
            EnemyName = bossNames[random.Next(bossNames.Length)];
        }

        /// <summary>
        /// Handles Enemy Attack logic.
        /// </summary>
        /// <param name="player">Passes a Player instance to handle damage logic.</param>
        /// <returns>boolean representing whether or not the attack was successful.</returns>
        public bool Attack(Player player)
        {
            int hitChance = random.Next(1, 101);
            bool hit = hitChance <= 60; // 60% chance for Enemy to hit.
            int attackPower;

            if (enemyType == "MINION") // Generates random attack power for Minion type.
            {
                attackPower = random.Next(10, 20);
            }
            else // Generates random attack power for Boss type.
            {
                attackPower = random.Next(21, 40);
            }

            if (hit)
            {
                player.TakeDamage(attackPower);

                if (enemyType != "BOSS") // Alters text output based on enemy type.
                {
                    Console.WriteLine($"The {EnemyName} hit you for {attackPower} damage!\n");
                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<\n");
                }
                else
                {
                    Console.WriteLine($"{EnemyName} hit you for {attackPower} damage!\n");
                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<\n");
                }
            }
            else
            {
                if (enemyType != "BOSS")
                {
                    Console.WriteLine($"The {EnemyName}'s attack missed!\n");
                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<\n");
                }
                else
                {
                    Console.WriteLine($"{EnemyName}'s attack missed!\n");
                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<\n");
                }
            }
            return hit;
        }

        /// <summary>
        /// Handles damage taken by Enemy.
        /// </summary>
        /// <param name="damage">The amount of damage to be dealt to Enemy instance.</param>
        public void TakeDamage(int damage)
        {
            this.Health -= damage;
        }

        /// <summary>
        /// Returns loot Consumable.
        /// </summary>
        /// <returns>Consumable instance.</returns>
        public Consumable DropLoot()
        {
            return loot;
        }
    }

    /// <summary>
    /// Represents a weapon in the game.
    /// </summary>
    public class Weapon
    {
        static Random random = new Random();
        public string WeaponName { get; private set; }
        public string Rarity { get; private set; }
        bool isEquipped = false;
        string[] weaponNames = new string[]
        {
            "Broadsword", "Morningstar", "Pike", "Dagger",
            "Greatsword", "Warhammer", "Sickle", "Falchion", "Rapier"
        };

        /// <summary>
        /// Constructs a Weapon instance and initializes the WeaponName and Rarity.
        /// </summary>
        public Weapon()
        {
            WeaponName = weaponNames[random.Next(weaponNames.Length)];

            Rarity = CalculateRarity();
        }

        /// <summary>
        /// Determines Rarity of a Weapon instance.
        /// </summary>
        /// <returns>String representation of rarity.</returns>
        private string CalculateRarity()
        {
            int rarityChance = random.Next(1, 101);

            if (1 <= rarityChance && rarityChance <= 60)  // 60% chance for COMMON rarity.
            {
                return "COMMON";
            } 
            else if (61 <= rarityChance && rarityChance <= 90) // 30% chance for RARE rarity.
            {
                return "RARE";
            } 
            else if (91 <= rarityChance && rarityChance <= 100) // 10% chance for LEGENDARY rarity.
            {
                return "LEGENDARY";
            }
            return "UNKNOWN";
        }

        /// <summary>
        /// Generates a random damage integer based on Weapon Rarity.
        /// </summary>
        /// <returns>Damage to be dealt by Weapon.</returns>
        public int CalculateDamage()
        {
            switch (Rarity) 
            {
                case "COMMON":
                    return random.Next(10, 19);
                case "RARE":
                    return random.Next(20, 29);
                case "LEGENDARY":
                    return random.Next(30, 50);
                default:
                    return 0;
            }
        }
    }

    /// <summary>
    /// Represents a consumable in the game, which inherits the item parent class.
    /// </summary>
    public class Consumable
    {
        static Random random = new Random();
        public string ConsumableName { get; private set; }
        public int RestorePoints { get; private set; }
        public int Quantity { get; set ; }
        string[] healthConsumables = new string[] {"Potion", "Tonic", "Elixir"};

        /// <summary>
        /// Constructs a Consumable instance and initializes ConsumableName, RestorePoints, and Quantity.
        /// </summary>
        public Consumable()
        {
            ConsumableName = healthConsumables[random.Next(healthConsumables.Length)];

            CalculateRestorationPoints();

            Quantity = 1;
        }

        /// <summary>
        /// Generates random integer to represent RestorePoints of Consumable.
        /// </summary>
        public void CalculateRestorationPoints()
        {
            RestorePoints = random.Next(20, 60);
        }

        /// <summary>
        /// Handles consumption logic of a Consumable by the Player.
        /// </summary>
        /// <param name="player">Passes a Player instance to restore health points.</param>
        /// <returns>Boolean representing whether or not the Consumable was consumed.</returns>
        public bool Consume(Player player)
        {
            if (Quantity > 0)
            {
                player.Health += RestorePoints;
                Quantity -= 1;
                return true;
            }
            return false;
        }
    }
}