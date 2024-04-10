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
            OutputIntroduction();
            while (playerAlive && roomsCompleted < 5)
            {
                OutputNewRoom();
                roomsCompleted++;
            }
            if (playerAlive) {
                OutputEnding();
            } else {
                OutputDeath();
            }
        }

        private void OutputIntroduction()
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
            Console.WriteLine($"You manage to grasp a {starterWeapon.WeaponName} with what little strength you have left.\nYou flail it about, getting acquainted with it's weight. The motions begin to feel eerily natural.");
            Console.WriteLine("\nThere is only one way out of this place you awoke in..\nand that is through the wall of light that shines before you.\nYou step forth...");
            
        }

        private void OutputNewRoom()
        {
            Console.WriteLine($"\n—————————————————————————————————————————————————————————————————————————————————————————————————————————\nEntering room {roomsCompleted + 1}...\n—————————————————————————————————————————————————————————————————————————————————————————————————————————\n");
            DungeonRoom room = new DungeonRoom();
            room.GenerateRoom();
            if (room.enemy != null)
            {
                OutputCombatLog(room.enemy);
            } 
        }

        private void OutputCombatLog(Enemy enemy)
        {
            int turn = 1;
            string actions = "\n:::::::::::::::::\n1. Attack\n2. Use Consumable\n3. Flee\n:::::::::::::::::"; 
            while (player.Health > 0 && enemy.Health > 0)
            {
                // Begin player turn
                Console.WriteLine($"==================\n      Turn {turn}\n==================");
                Console.WriteLine(actions);
                bool validAction = false;

                while (!validAction) 
                {
                    Console.Write("\nYou choose to... \n");
                    string playerInput = Console.ReadLine().Trim().ToLower();
                    int playerAction;
                    if (int.TryParse(playerInput, out playerAction))
                    {
                        switch (playerAction) 
                        {
                            case 1:
                                player.Attack(enemy);
                                validAction = true;
                                break;
                            case 2:
                                Console.WriteLine("Placeholder text...");
                                validAction = true;
                                break;
                            case 3:
                                Console.WriteLine("Placeholder text...");
                                validAction = true;
                                break;
                            default:
                                Console.WriteLine($"The {enemy.EnemyName} stares as you decide what to do...");
                                break;
                        }
                    } else {
                        Console.WriteLine("Faint whispers echo within your ears, they beckon you to make a decision...");
                    }
                }
                // Begin enemy turn
                enemy.Attack(player);
                turn++;
            }
            if (player.Health <= 0)
            {
                playerAlive = false;
            } else if (enemy.Health <= 0)
            {
                Console.WriteLine($"The {enemy.EnemyName} perished...");
                Item droppedLoot = enemy.DropLoot();
                Console.WriteLine($"You search it's corpse and find {droppedLoot}");
            }
        }

        private void OutputEnding()
        {
            Console.WriteLine("END");
        }

        private void OutputDeath()
        {
            Console.WriteLine("YOU DIED");
        }
    }

    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player 
    {
        static Random random = new Random();
        string _name { get; set; }
        int _health = 100;
        public Weapon equippedWeapon = null;

        List<Item> inventory = new List<Item>();

        public Player(string name)
        {
            _name = name;
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
                Console.WriteLine($"You hit the {enemy.EnemyName} for {damageDealt} damage!");
            } 
            else 
            {
                Console.WriteLine("Your attack missed..");
            }
            return hit;
        }

        public void TakeDamage(int damage)
        {
            this.Health -= damage;
        }

        public void PickupItem(Item item)
        {
            inventory.Add(item);
        }

        public void EquipWeapon(Weapon weapon)
        {
            equippedWeapon = weapon;
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
            "The walls of the room are lined with green hellish flames that spew from several sconces.\nGreat masses of bone protrude from the ground,\ntheir shadows resemble lost souls grasping out above for some sort of savior.",
            "A thick mist fills the room, obscuring vision beyond a few feet.\nFaint whispers echo off the walls, carrying fragmented memories and forgotten secrets.\nShapes loom in the haze.. flickering in and out of existence like specters from another realm.",
            "The room is a surreal dreamscape, with shifting walls of iridescent colors and swirling patterns.\nTime seems to warp and bend, creating a sense of disorientation.\nStrange creatures made of pure energy flit in and out of existence..\nleaving trails of shimmering light in their wake.",
            "The room appears to be frozen in time..\nwith delicate icicles hanging from the ceiling and frost coating every surface.\nA soft, blue light emanates from within the ice, casting an ethereal glow.\nFrozen in mid-motion, statues of ancient beings stand sentinel..\ntheir faces etched with expressions of sorrow and longing."
            };
        string[] lootRoomDescriptions = new string[]
            {
            "",
            };
        public Enemy enemy = null;
        public Item loot = null;

        public string CalculateRoomChance()
        {
            int roomChance = random.Next(1, 101);

            if (1 <= roomChance && roomChance <= 70) {
                return "ENEMY";
            } else {
                return "LOOT";
            }
        }

        public void GenerateRoom()
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
                    Console.WriteLine(lootRoomDescriptions[random.Next(lootRoomDescriptions.Length)]);

                    Console.WriteLine("An object levitates in the middle in the room.. You decide to approach it.");
                    break;
                default:
                    Console.WriteLine("Error: Unknown room type...");
                    break;
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
        string _enemyName;
        int _health;
        int attackPower;
        string[] minionNames = new string[] {"SKELETAL WARRIOR", "VAMPIRIC SEDUCTRESS", "UNDEAD HUMAN", "RADIATING ORB OF LIGHT", "RUSTED GOLEM"};
        string[] bossNames = new string[] {"TADZIU THE DEMON KING", "KREE THE ICE WIZARD", "MURTOG THE TOAD"};
        Item loot = new Item();

        public string EnemyName
        {
            get { return _enemyName; }
            set { _enemyName = value; }
        }

        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public void GenerateMinion()
        {
            this.enemyType = "MINION";
            this.Health = 100;
            this.attackPower = random.Next(10, 30);
            this.EnemyName = minionNames[random.Next(minionNames.Length)];
        }

        public void GenerateBoss()
        {
            this.enemyType = "BOSS";
            this.Health = 300;
            this.attackPower = random.Next(40, 60);
            this.EnemyName = bossNames[random.Next(bossNames.Length)];
        }

        public bool Attack(Player player)
        {
            int hitChance = random.Next(1, 101);
            bool hit = hitChance <= 70;

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

        public Item DropLoot()
        {
            return loot;
        }
    }

    /// <summary>
    /// Represents an item in the game.
    /// </summary>
    public class Item
    {
        string itemType;
        static Random random = new Random();
        public Item()
        {

        }
    }

    /// <summary>
    /// Represents a weapon in the game, which inherits the item parent class.
    /// </summary>
    public class Weapon : Item
    {
        static Random random = new Random();
        string _weaponName;
        string _rarity;
        bool isEquipped = false;
        string[] weaponNames = new string[] 
        {
            "Broadsword", "Morningstar", "Pike", "Dagger",
            "Greatsword", "Warhammer", "Sickle", "Falchion", "Rapier"
        };

        public Weapon() : base()
        {
            this.WeaponName = weaponNames[random.Next(weaponNames.Length)];
            this.Rarity = CalculateRarity();
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
            switch (_rarity) 
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

        public string WeaponName
        {
            get { return _weaponName; }
            set { _weaponName = value; }
        }

        public string Rarity
        {
            get { return _rarity; }
            set { _rarity = value; }
        }
    }

    public class Consumable : Item
    {

    }
}