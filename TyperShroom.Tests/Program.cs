using System;

namespace TyperShroom.Core {
    internal class Program {
        static void PrintState(GameEngine gameEngine) {
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------------------------------------------------");
            Console.WriteLine("STATE INFO");
            Console.WriteLine($"ActiveBugs : {gameEngine.CurrentState.ActiveBugs.Count}");
            Console.WriteLine($"Lives : {gameEngine.CurrentState.Lives}");
            Console.WriteLine($"Wave : {gameEngine.CurrentState.Wave}");
            Console.WriteLine($"Score : {gameEngine.CurrentState.Score}");
            if (gameEngine.CurrentState.CurrentTarget != null) {
                Console.WriteLine($"TargetedBug : {gameEngine.CurrentState.CurrentTarget.Word}");
                Console.WriteLine($"RemaningWord : {gameEngine.CurrentState.CurrentTarget.RemainingWord}");
            }
            else
                Console.WriteLine("TargetedBug : None");

            Console.WriteLine();
            Console.WriteLine("Active Bugs");
            foreach (Bug bug in gameEngine.CurrentState.ActiveBugs) {
                Console.WriteLine(bug.Word);
            }
        }

        static void Main(string[] args) {
            GameEngine gameEngine = new GameEngine();
            gameEngine.StartGame(); 

            while (true) {         
                PrintState(gameEngine);  

                char key = Console.ReadKey().KeyChar;
                gameEngine.ProcessKeystroke(key);

                gameEngine.Update(0.1);
            }
        }
    }   
}
