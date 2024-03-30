using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

public static class DungeonDelverMain{
    
    public class Dungeon{
        public List<DungeonRoom> RoomsInDungeon {get; set;}
        public int RoomNumber {get; set;}
        public int difficulty {get; set;}


        public class DungeonRoom{
            public string RoomNameType {get; set;}
            public string RoomDescription {get; set;}
            public void CreateRoom(string name, string description){
                RoomNameType = name;
                RoomDescription = description;
            }
        }
        //Creates a dungeon, # of rooms depends on difficulty. All rooms are stored in RoomsInDungeon
        public void CreateDungeon(int diff) {
            Random rnd = new Random();
            RoomsInDungeon = [];
            difficulty = diff;
            switch (diff){
                case 1: RoomNumber = rnd.Next(4, 6); // creates a number between 4 and 5
                break;
                case 2: RoomNumber = rnd.Next(4, 7); // 4-6
                break;
                case 3: RoomNumber = rnd.Next(5, 7); // 5-6
                break;
                case 4: RoomNumber = rnd.Next(5, 7); 
                break;
                case 5: RoomNumber = rnd.Next(6, 8); // 5-7
                break;
                case 6: RoomNumber = rnd.Next(6, 8); 
                break;
                case 7: RoomNumber = rnd.Next(7, 9); // 7-8
                break;
                case 8: RoomNumber = rnd.Next(7, 10); // 7-9
                break;
                case 9: RoomNumber = rnd.Next(8, 12); // 8-11
                break;
                case 10: RoomNumber = rnd.Next(10, 16);// 10-15
                break;
                default: RoomNumber = 5;
                break;
            }

            void GenerateRooms(){ // treasure, trap, monster, puzzle
                for (int i = 0; i < RoomNumber; i++){
                    DungeonRoom testRoom = new DungeonRoom();
                    int RoomType = rnd.Next(1, 11);  // 1-10

                    //final room of each dungeon will always be a boss room
                    if (i == RoomNumber - 1) {
                        testRoom.CreateRoom("Boss", "This is a boss room.");
                    }
                    else {
                    // monster 60% 
                    // puzzle 20%
                    // trap 10%
                    // treasure 10%
                        if (RoomType >= 1 && RoomType <= 6) {
                            testRoom.CreateRoom("Monster", "This is a monster room.");
                        }
                        else if (RoomType == 7 || RoomType == 8){
                            testRoom.CreateRoom("Puzzle", "This is a puzzle room.");
                        }
                        else if (RoomType == 9) {
                            testRoom.CreateRoom("Trap", "This is a trap room.");
                        }
                        else if (RoomType == 10) {
                            testRoom.CreateRoom("Treasure", "This is a treasure room.");
                        }
                    }
                    RoomsInDungeon.Add(testRoom);
                }
            }
            GenerateRooms();
        }
    }

    public class Monster{
        public string Name {get; set;}
        public int HP {get; set;}
        public int SPD {get; set;}
        public int DEF {get; set;}
    }

    public class PlayerClass{
        public string title {get; set;}
        public int HP {get; set;}
        public int DEF {get; set;}
        public int SPD {get; set;}
        
        public  void SetOccupation(string name, int health, int defense, int speed){
            title = name;
            HP = health;
            DEF = defense; 
            SPD = speed;
        }

        public void TakeDMG(int attackDMG){
            int finalDMG = attackDMG - (int)Math.Ceiling((DEF / 5) * 0.5);
            HP -= finalDMG;
        }


    }
    static void Main(String[] args){
        Console.WriteLine("Welcome adventurer! Which option relates to you more?");
        Console.WriteLine("     1. Cutting foes down mercessily.");
        Console.WriteLine("     2. Using profane magic to obliterate everything in your path.");         
        
        PlayerClass player = new PlayerClass();

        static void SetOccupation(PlayerClass user) {
            int userInput = Convert.ToInt32(Console.ReadLine());
            if (userInput == 1) {
                user.SetOccupation("Warrior", 100, 50, 40); 
            }
            else if (userInput == 2) {
                user.SetOccupation("Wizard", 100, 25, 50);    
            }
            else {
                Console.WriteLine("That doesn't sound like a 1 or 2 to me. Try again.");
                SetOccupation(user);
            }
        }

        SetOccupation(player);
        
        Console.WriteLine($"A {player.title}? Good Choice!");
        Console.WriteLine("...");
        Console.WriteLine("Press anything to continue.");
        string userInput = Console.ReadLine();

        Dungeon testDungeon = new Dungeon();
        testDungeon.CreateDungeon(1);
        
        Console.WriteLine("You will be entering a Rank 1 dungeon.");
        
        // just a placeholder, idealy the room description would go here
        Console.WriteLine($"Your first dungeon room is a {testDungeon.RoomsInDungeon[0].RoomNameType} room.");
        
        for (int i = 0; i < testDungeon.RoomNumber; i++){
            //Console.WriteLine(testDungeon.RoomsInDungeon[i].RoomNameType);
        }
    }
}

