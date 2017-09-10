using MonteCarlo;
using System;
using System.Linq;

namespace ConnectFour
{
    class Program
    {
        private const string SEPARATOR = "-------";
        static void Main(string[] args)
        {
            var game = new ConnectFourState();
            while (game.Actions.Any())
            {
                Console.WriteLine($"CurrentPlayer: {game.CurrentPlayer}");
                Console.WriteLine(game);
                Console.WriteLine("0123456");
                Console.WriteLine(SEPARATOR);

                var position = -1;
                while(position < 0 || position > 6)
                {
                    Console.WriteLine("Choose a free space: (0-6)");

                    var input = Console.ReadKey();
                    int.TryParse(input.KeyChar.ToString(), out position);
                }

                Console.WriteLine();
                game.ApplyAction(new ConnectFourAction(position));
                var computer = MonteCarloTreeSearch.GetTopActions(game, 50000, 1000).ToList();

                Console.WriteLine(SEPARATOR);
                if (computer.Count > 0)
                {
                    Console.WriteLine("Computer's ranked plays:");
                    foreach (var a in computer)
                        Console.WriteLine($"\t{a.Action}\t{a.NumWins}/{a.NumRuns} ({a.NumWins / a.NumRuns})");
                    game.ApplyAction(computer[0].Action);
                }

                position = -1;
            }

            Console.WriteLine(SEPARATOR);
            Console.WriteLine(game.ToString());
            Console.WriteLine("Game Over");
            Console.ReadKey();
        }
    }
}
