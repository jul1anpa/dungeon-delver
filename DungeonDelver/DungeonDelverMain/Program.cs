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
            //Console.WriteLine(userInput);
            if (userInput == 1) {
                user.SetOccupation("Warrior", 100, 50); 
            }
            else if (userInput == 2) {
                user.SetOccupation("Wizard", 100, 25);    
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

        player.TakeDMG(20);
        Console.WriteLine("You got hit for 20 damage!");
        Console.WriteLine($"Your health is now at {player.HP}");

    }
}

