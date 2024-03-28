using System.Dynamic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

public static class DungeonDelverMain{
    
    public class PlayerClass{
        public string title {get; set;}
        public int HP {get; set;}
        public int DEF {get; set;}

        public  void SetOccupation(string name, int health, int defense){
            title = name;
            HP = health;
            DEF = defense; 
        }

    }
    static void Main(String[] args){
        Console.WriteLine("Welcome adventurer! Which option relates to you more?");
        Console.WriteLine("     1. Cutting foes down mercessily.");
        Console.WriteLine("     2. Using profane magic to obliterate everything in your path.");         
        
       

        PlayerClass player = new PlayerClass();

        static void StartSetup(PlayerClass user) {
            int userInput = Convert.ToInt32(Console.ReadLine());
            if (userInput == 1) {
            user.SetOccupation("Warrior", 100, 50); 
            }
            if (userInput == 2) {
                user.SetOccupation("Wizard", 100, 25);    
            }
            else {
                Console.WriteLine("That doesn't sound like a 1 or 2 to me. Try again.");
                StartSetup(user);
            }
        }

        StartSetup(player);
        
        Console.WriteLine($"A {player.title}? Good Choice!");
    }
}

