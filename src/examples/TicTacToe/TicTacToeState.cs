using MonteCarlo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
{
    public class TicTacToeState : IState<TicTacToePlayer, TicTacToeAction>
    {
        private IList<TicTacToePlayer> board;

        public TicTacToeState() : this(new TicTacToePlayer[9], TicTacToePlayer.X) { }

        public TicTacToeState(TicTacToePlayer[] board, TicTacToePlayer currentPlayer)
        {
            this.board = board;
            CurrentPlayer = currentPlayer;
        }

        public TicTacToePlayer CurrentPlayer { get; private set; }

        public IList<TicTacToeAction> Actions
        {
            get
            {
                if (hasWinner(TicTacToePlayer.X) || hasWinner(TicTacToePlayer.O))
                    return new TicTacToeAction[0];

                return board
                    .Take(9)
                    .Select((player, position) => new { player, position})
                    .Where(o => o.player == null)
                    .Select((o) => new TicTacToeAction(o.position, CurrentPlayer))
                    .ToList();
            }
        }

        public void ApplyAction(TicTacToeAction action)
        {
            board[action.Position] = action.Player;
            CurrentPlayer = CurrentPlayer.NextPlayer;
        }

        public IState<TicTacToePlayer, TicTacToeAction> Clone()
        {
            return new TicTacToeState(board.ToArray(), CurrentPlayer);
        }

        private static int[][] winningCombos = new[]
        {
            new [] {0, 1, 2},
            new [] {0, 4, 8},
            new [] {0, 3, 6},
            new [] {1, 4, 7},
            new [] {2, 4, 6},
            new [] {2, 5, 8},
            new [] {3, 4, 5}
        };

        private bool hasWinner(TicTacToePlayer forPlayer)
        {
            return winningCombos.Any(c => c.All(i => board[i] != null && board[i].IsX == forPlayer.IsX));
        }

        public double GetResult(TicTacToePlayer forPlayer)
        {
            return hasWinner(forPlayer) ? 1
                : hasWinner(forPlayer.NextPlayer) ? 0
                : 0.5;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for(var rowOffset = 0; rowOffset < 9; rowOffset+=3)
            {
                for(var col = 0; col < 3; col++)
                {
                    var inputValue = rowOffset + col;
                    var player = board[inputValue];
                    sb.Append(player == null ? inputValue.ToString() : player.ToString());
                    if (col < 2)
                        sb.Append("|");
                }
                if(rowOffset < 8)
                    sb.Append("\n-----\n");
            }

            return sb.ToString();
        }
    }
}
