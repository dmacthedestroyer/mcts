using MonteCarlo;
using System;

namespace TicTacToe
{
    public struct TicTacToeAction : IAction
    {
        public TicTacToeAction(int position, TicTacToePlayer player)
        {
            if (position < 0 || position > 8)
                throw new ArgumentException("position must be between 0 and 8, inclusive", nameof(position));

            Position = position;
            Player = player;
        }

        public int Position { get; private set; }
        public TicTacToePlayer Player { get; private set; }

        public override string ToString()
        {
            return $"{Player}: {Position}";
        }
    }
}
