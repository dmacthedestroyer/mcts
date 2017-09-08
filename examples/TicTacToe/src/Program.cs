using MonteCarlo;
using System;
using System.Linq;

namespace TicTacToe
{
    class Program
    {
        private const string SEPARATOR = "______";
        static void Main(string[] args)
        {
            var game = new TicTacToeState();
            while(game.Actions.Any())
            {
                Console.WriteLine($"CurrentPlayer: {game.CurrentPlayer}");
                Console.WriteLine(game);
                Console.WriteLine(SEPARATOR);

                var position = -1;
                while (position < 0)
                {
                    Console.WriteLine("Choose a free space: (0-8)");

                    var input = Console.ReadKey();
                    int.TryParse(input.KeyChar.ToString(), out position);
                }

                Console.WriteLine();

                game.ApplyAction(new TicTacToeAction(position, TicTacToePlayer.X));
                var computer = MonteCarloTreeSearch.GetTopActions(game, 50000).ToList();
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
