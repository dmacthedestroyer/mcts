using MonteCarlo;

namespace TicTacToe
{
    public class TicTacToePlayer : IPlayer
    {
        private TicTacToePlayer(bool isX)
        {
            IsX = isX;
        }

        public bool IsX { get; }

        public TicTacToePlayer NextPlayer => IsX ? O : X;

        public override string ToString()
        {
            return IsX ? "X" : "O";
        }

        public static TicTacToePlayer X = new TicTacToePlayer(true);
        public static TicTacToePlayer O = new TicTacToePlayer(false);
    }
}
