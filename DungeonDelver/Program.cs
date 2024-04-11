using System;

namespace DungeonDelver 
{
    class Program 
    {
        static void Main(string[] args) 
        {
            Console.WriteLine("┳┓              ┳┓  ┓      \n┃┃┓┏┏┓┏┓┏┓┏┓┏┓  ┃┃┏┓┃┓┏┏┓┏┓\n┻┛┗┻┛┗┗┫┗ ┗┛┛┗  ┻┛┗ ┗┗┛┗ ┛\n");
            Console.WriteLine("\n1. New Game\n2. Exit\n");
            string userInput = Console.ReadLine().Trim().ToLower();
            int startAction;
            if (int.TryParse(userInput, out startAction))
            switch (startAction) {
                case 1:
                    GameManager gameManager = new GameManager();
                    gameManager.NewGame();
                    break;
                default:
                    Console.WriteLine("Game exited.");
                    return;
            }
        }
    }
}