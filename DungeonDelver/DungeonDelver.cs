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
        int roomsCompleted;
        bool playerAlive;

        /// <summary>
        /// Initializes the game.
        /// </summary>
        public void NewGame()
        {
            playerAlive = true;
            roomsCompleted = 0;
            player = OutputIntroduction();
            while (playerAlive && roomsCompleted < 5)
            {
                OutputNewRoom(player);
                roomsCompleted++;
            }
            OutputBossRoom(player);
            if (playerAlive) {
                OutputEnding();
            } else {
                OutputDeath();
            }
        }

        private Player OutputIntroduction()
        {
            Console.WriteLine("\nYou awaken... You cannot recall any memories of the past and carry no longing for the future.\nThe ground beneath you is cold to the touch and a piercing white light washes over you.\nIt emanates from beyond the corridors of this room.");
            Console.WriteLine("\nYou lie there in silence..\nTime is an abstract concept to you, moving neither forward nor backward.\nYou try to remember your name...");
            Console.Write("\nYour name: ");
            string name = Console.ReadLine();
            player = new Player(name);
            Console.WriteLine($"\nYou decide the name {name} is befitting enough.\nYou gather your senses and whatever courage that resides within you.\nYour body feels frail as you struggle to get to your feet.\n\nAs you look around the room you notice something glimmering on the ground directly in front of you.");
            Console.WriteLine("You reach for it.");
            Weapon starterWeapon = new Weapon();
            player.EquipWeapon(starterWeapon);
            Console.WriteLine($"You manage to grasp a {starterWeapon.Rarity} {starterWeapon.WeaponName} with what little strength you have left.\nYou flail it about, getting acquainted with it's weight. The motions begin to feel eerily natural.");
            Console.WriteLine("\nThere is only one way out of this place you awoke in..\nand that is through the wall of light that shines before you.\nYou step forth...");
            return player;
        }

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

        private void OutputBossRoom(Player player)
        {
            Console.WriteLine($"\n—————————————————————————————————————————————————————————————————————————————————————————————————————————\nEntering the final room...\n—————————————————————————————————————————————————————————————————————————————————————————————————————————\n");
            DungeonRoom bossRoom = new DungeonRoom();
            bossRoom.GenerateBossRoom(player);
            OutputCombatLog(bossRoom.boss);
        }

        private void OutputCombatLog(Enemy enemy)
        {
            int turn = 1;
            string actions = "\n:::::::::::::::::::\n1. Attack\n2. Use Consumable\n3. Flee\n:::::::::::::::::::"; 
            while (player.Health > 0 && enemy.Health > 0)
            {
                // Begin player turn
                Console.WriteLine($"==================\n      Turn {turn}\n==================");
                Console.WriteLine($"\n      HP: {player.Health}");
                Console.WriteLine($"   Enemy HP: {enemy.Health}");
                Console.WriteLine(actions);
                bool validAction = false;
                bool itemUsed = false;
                while (!validAction) 
                {
                    Console.Write("\nYou choose to... \n");
                    string playerInput = Console.ReadLine().Trim();
                    int playerAction;
                    string itemName = "";
                    if (int.TryParse(playerInput, out playerAction))
                    {
                        switch (playerAction) 
                        {
                            case 1:
                                player.Attack(enemy);
                                validAction = true;
                                break;
                            case 2:
                                player.OpenInventory();
                                bool validChoice = false;
                                while (!validChoice)
                                {
                                    Console.WriteLine("Enter the name of the item you would like to consume or X to return to combat: ");
                                    itemName = Console.ReadLine().Trim().ToLower();
                                    if (itemName == "x")
                                    {
                                        Console.WriteLine("You fumble through your inventory to no avail..");
                                        break;
                                    }
                                    itemUsed = player.UseConsumable(itemName);
                                    validChoice = itemUsed;
                                }
                                validAction = true;
                                break;
                            case 3:
                                Console.WriteLine($"\nYour courage leaves you and you flee from battle!\nYou hastily scamper for the door..\n\nThe {enemy.EnemyName} manages to make a final blow on your way out..");
                                enemy.Attack(player);
                                validAction = true;
                                if (player.Health <= 0)
                                {
                                    playerAlive = false;
                                }
                                return;
                            default:
                                Console.WriteLine($"The {enemy.EnemyName} stares as you decide what to do...");
                                break;
                        }
                    } else {
                        Console.WriteLine("Faint whispers echo within your ears, they beckon you to make a decision...");
                    }
                }
                // Begin enemy turn
                if (enemy.Health >= 0 && !itemUsed)
                {
                    enemy.Attack(player);
                    continue;
                }
                turn++;
            }
            if (player.Health <= 0)
            {
                playerAlive = false;
            } else if (enemy.Health <= 0)
            {
                Console.WriteLine($"The {enemy.EnemyName} perished...");
                Consumable droppedLoot = enemy.DropLoot();
                Console.WriteLine($"You search it's corpse and find a {droppedLoot.ConsumableName}");
                player.PickupItem(droppedLoot);
            }
        }

        private void OutputEnding()
        {
            Console.WriteLine("T H E   E N D .");
        }

        private void OutputDeath()
        {
            Console.WriteLine("You feel your spirit leave your body..\nYou fall to your knees and accept your fate...\n");
            Console.WriteLine("Y O U   D I E D .\n");
            Console.WriteLine("Play again?\n1. Yes\n2. No");
            string startAgain = Console.ReadLine().Trim();
            int action;
            if (int.TryParse(startAgain, out action))
            {
                switch (action) 
                {
                    case 1:
                        this.NewGame();
                        break;
                    default:
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
        int _health = 100;
        public Weapon equippedWeapon = null;

        List<Consumable> inventory = new List<Consumable>();

        public Player(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the player's health points.
        /// </summary>
        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public bool Attack(Enemy enemy)
        {
            int hitChance = random.Next(1, 101);
            bool hit = hitChance <= 90;

            if (hit) 
            {
                int damageDealt = equippedWeapon.CalculateDamage();
                enemy.TakeDamage(damageDealt);
                Console.WriteLine($"\nYou hit the {enemy.EnemyName} for {damageDealt} damage!");
            } 
            else 
            {
                Console.WriteLine("\nYour attack missed..");
            }
            return hit;
        }

        public void TakeDamage(int damage)
        {
            this.Health -= damage;
        }

        public void PickupItem(Consumable pickedupItem)
        {
            foreach (Consumable item in inventory)
            {
                if (pickedupItem.ConsumableName == item.ConsumableName)
                {
                    item.Quantity += 1;
                    return;
                }
            }
            inventory.Add(pickedupItem);
        }

        public void EquipWeapon(Weapon weapon)
        {
            equippedWeapon = weapon;
        }

        public void OpenInventory()
        {
            Console.WriteLine($"\n===========Inventory===========\n");
            foreach (Consumable item in inventory)
            {
                Console.WriteLine($"x{item.Quantity} {item.ConsumableName}");
            }
            Console.WriteLine ("\n===============================\n");
        }

        public bool UseConsumable(string consumableName)
        {
            bool itemUsed = false;
            foreach (Consumable item in inventory)
            {
                if (consumableName == item.ConsumableName.ToLower())
                {
                    itemUsed = item.Consume(this);
                    Console.WriteLine($"The {item.ConsumableName} restored {item.RestorePoints} health points!\n");
                    if (item.Quantity <= 0)
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
        string[] enemyRoomDescriptions = new string[] 
            {"The walls of the room are reflective, almost as if made of glass..\nA ray of white light penetrates the room from a gaping hole in the ceiling.\nThis light reflects every which way, creating abtract patterns of light in the air.",
            "The walls of the room are lined with green hellish flames that spew from several sconces.\nGreat masses of bone protrude from the ground..\ntheir shadows resemble lost souls grasping out above for some sort of savior.",
            "A thick mist fills the room, obscuring vision beyond a few feet.\nFaint whispers echo off the walls, carrying fragmented memories and forgotten secrets.\nShapes loom in the haze.. flickering in and out of existence like specters from another realm.",
            "The room is a surreal dreamscape, with shifting walls of iridescent colors and swirling patterns.\nTime seems to warp and bend, creating a sense of disorientation.\nStrange creatures made of pure energy flit in and out of existence..\nleaving trails of shimmering light in their wake.",
            "The room appears to be frozen in time..\nwith delicate icicles hanging from the ceiling and frost coating every surface.\nA soft, blue light emanates from within the ice, casting an ethereal glow.\nFrozen in mid-motion, statues of ancient beings stand sentinel..\ntheir faces etched with expressions of sorrow and longing."
            };
        public Enemy enemy = null;
        public Enemy boss = null;
        public Weapon loot = null;

        public string CalculateRoomChance()
        {
            int roomChance = random.Next(1, 101);

            if (1 <= roomChance && roomChance <= 80) {
                return "ENEMY";
            } else {
                return "LOOT";
            }
        }

        public void GenerateRoom(Player player)
        {
            string roomType = CalculateRoomChance();
            switch (roomType) {
                case "ENEMY":
                    Console.WriteLine(enemyRoomDescriptions[random.Next(enemyRoomDescriptions.Length)]);
                    enemy = new Enemy();
                    enemy.GenerateMinion();
                    Console.WriteLine($"\nA {enemy.EnemyName} appears before you...\n");
                    break;
                case "LOOT":
                    Console.WriteLine("Within granite walls, a steel chest sits atop a pedestal emitting a soft golden light.\n..a solitary enigma in a barren chamber.");
                    Console.WriteLine("\nDo you approach the chest and attempt to open it?\n1. Open it\n2. Move on");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int decision))
                    {
                        switch (decision)
                        {
                            case 1:
                                bool isTrap = CalculateTrap(player);
                                if (!isTrap)
                                {
                                    Weapon newWeapon = new Weapon();
                                    Console.WriteLine($"You reach into the light, pulling out a {newWeapon.Rarity} {newWeapon.WeaponName}!");
                                    Console.WriteLine($"Would you like to equip it?\n1. Equip {newWeapon.WeaponName}\n2. Leave it");
                                    string equipChoice = Console.ReadLine().Trim();
                                    if (equipChoice == "1")
                                    {
                                        Console.WriteLine($"\nYou equipped the {newWeapon.WeaponName}!");
                                        player.EquipWeapon(newWeapon);
                                        break;
                                    } else 
                                    {
                                        Console.WriteLine("You choose to leave it behind...");
                                        break;
                                    }
                                }
                                break;
                            default:
                                Console.WriteLine("You decide to keep pushing forward..");
                                break;
                        }
                    }
                    break;
                default:
                    Console.WriteLine("ERROR: Unknown room type...");
                    break;
            }
        }

        public void GenerateBossRoom(Player player)
        {
            boss = new Enemy();
            boss.GenerateBoss();
            Console.WriteLine("As you step into the final chamber, the air crackles with an electrifying energy.\nDark clouds swirl overhead, illuminated by flashes of lightning that streak across the sky..\nThe ground trembles beneath your feet as if the very earth is anticipating the imminent confrontation...");
            Console.WriteLine($"\n{boss.EnemyName} appears before you...\n");
        }

        public bool CalculateTrap(Player player)
        {
            int trapChance = random.Next(1, 101);
            int trapDamage = random.Next(10, 40);
            bool isTrap;
            if (1 <= trapChance && trapChance <= 75)
            {
                isTrap = false;
                Console.WriteLine("\nYou unlatch the locking mechanism of the chest and lift the lid..\nA golden light eminates from within the chest and blinds you for a brief moment..");
                return isTrap;
            } else {
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
        int attackPower;
        string[] minionNames = new string[] {"SKELETAL WARRIOR", "VAMPIRIC SEDUCTRESS", "UNDEAD HUMAN", "RADIATING ORB OF LIGHT", "RUSTED GOLEM"};
        string[] bossNames = new string[] {"TADZIU THE DEMON KING", "KREE THE ICE WIZARD", "MURTOG THE TOAD"};
        Consumable loot = new Consumable();

        public void GenerateMinion()
        {
            this.enemyType = "MINION";
            this.Health = 100;
            this.attackPower = random.Next(10, 20);
            this.EnemyName = minionNames[random.Next(minionNames.Length)];
        }

        public void GenerateBoss()
        {
            this.enemyType = "BOSS";
            this.Health = 300;
            this.attackPower = random.Next(30, 50);
            this.EnemyName = bossNames[random.Next(bossNames.Length)];
        }

        public bool Attack(Player player)
        {
            int hitChance = random.Next(1, 101);
            bool hit = hitChance <= 60;

            if (hit)
            {
                int damageDealt = attackPower;
                player.TakeDamage(damageDealt);
                Console.WriteLine($"The {EnemyName} hit you for {damageDealt} damage!\n");
            }
            else
            {
                Console.WriteLine($"The {EnemyName}'s attack missed!\n");
            }
            return hit;
        }

        public void TakeDamage(int damage)
        {
            this.Health -= damage;
        }

        public Consumable DropLoot()
        {
            return loot;
        }
    }

    /// <summary>
    /// Represents a weapon in the game, which inherits the item parent class.
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

        public Weapon()
        {
            WeaponName = weaponNames[random.Next(weaponNames.Length)];
            Rarity = CalculateRarity();
        }

        private string CalculateRarity()
        {
            int rarityChance = random.Next(1, 101);

            if (1 <= rarityChance && rarityChance <= 60) 
            {
                return "COMMON";
            } 
            else if (61 <= rarityChance && rarityChance <= 89) 
            {
                return "RARE";
            } 
            else if (90 <= rarityChance && rarityChance <= 100) 
            {
                return "LEGENDARY";
            }
            return "UNKNOWN";
        }

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

        public Consumable()
        {
            ConsumableName = healthConsumables[random.Next(healthConsumables.Length)];
            CalculateRestorationPoints();
            Quantity = 1;
        }

        public void CalculateRestorationPoints()
        {
            RestorePoints = random.Next(20, 60);
        }

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