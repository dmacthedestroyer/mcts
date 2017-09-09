using MonteCarlo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConnectFour
{
    public class ConnectFourPlayer : IPlayer
    {
        public string Name { get; private set; }

        private ConnectFourPlayer(string name)
        {
            Name = name;
        }

        public ConnectFourPlayer NextPlayer => this == X ? O : X;

        public override string ToString()
        {
            return Name;
        }

        public static readonly ConnectFourPlayer X = new ConnectFourPlayer("X");
        public static readonly ConnectFourPlayer O = new ConnectFourPlayer("O");
    }

    public struct ConnectFourAction : IAction
    {
        public int Column { get; private set; }

        public ConnectFourAction(int column)
        {
            Column = column;
        }

        public override string ToString()
        {
            return Column.ToString();
        }
    }

    public class ConnectFourState : IState<ConnectFourPlayer, ConnectFourAction>
    {
        public const int NumRows = 6;
        public const int NumCols = 7;

        // board is a NumRows * NumCols array, with the zeroeth index being the bottom left and the last value being the top right
        private ConnectFourPlayer[] board;
        // to save time, we precompute the available actions and store them in this variable
        private IList<ConnectFourAction> availableActions;
        // to save time, we precompute the available winning runs of 4 ahead of time and prune the list as we apply actions
        private IList<int[]> availableWinningRuns = new List<int[]>();

        /// <summary>
        /// Converts a row, column coordinate into a flat index in the board
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private int GetBoardIndex(int row, int col)
        {
            return row * NumCols + col;
        }

        private IList<int[]> GetDefaultAvailableWinningRuns()
        {
            var runs = new List<int[]>();

            for(var row = 0; row < NumRows; row++)
                for(var col = 0; col < NumCols; col++)
                {
                    if (col < NumCols - 3)
                    {
                        var horizontalRun = Enumerable.Range(col, 4).Select(c => GetBoardIndex(row, c)).ToArray();
                        runs.Add(horizontalRun);
                    }
                    if (row < NumRows - 3)
                    {
                        var verticalRun = Enumerable.Range(row, 4).Select(r => GetBoardIndex(r, col)).ToArray();
                        runs.Add(verticalRun);
                    }
                }

            // TODO: diagonal bottom-to-top runs

            // TODO: diagonal top-to-bottom runs

            return runs;
        }

        public ConnectFourPlayer CurrentPlayer { get; private set; }

        public IList<ConnectFourAction> Actions => availableActions;

        public ConnectFourState(ConnectFourPlayer[] board, ConnectFourPlayer currentPlayer = null)
        {
            this.board = board;
            availableActions = GetRow(NumCols - 1)
                .Select((p, i) => new { p, i })
                .Where(o => o.p == null)
                .Select(o => new ConnectFourAction(o.i))
                .ToList();

            // TODO: prune runs that can't produce wins
            availableWinningRuns = GetDefaultAvailableWinningRuns();
            CurrentPlayer = CurrentPlayer ?? ConnectFourPlayer.X;
        }

        public ConnectFourState() {
            this.board = new ConnectFourPlayer[NumRows * NumCols];
            availableActions = Enumerable.Range(0, NumCols).Select(i => new ConnectFourAction(i)).ToList();
            availableWinningRuns = GetDefaultAvailableWinningRuns();
            CurrentPlayer = ConnectFourPlayer.X;
        }

        public ConnectFourState(ConnectFourState toClone)
        {
            board = toClone.board.ToArray();
            availableActions = toClone.availableActions.ToList();
            availableWinningRuns = toClone.availableWinningRuns.ToList();
            CurrentPlayer = toClone.CurrentPlayer;
        }

        public IState<ConnectFourPlayer, ConnectFourAction> Clone()
        {
            return new ConnectFourState(this);
        }

        public void ApplyAction(ConnectFourAction action)
        {
            var row = GetCol(action.Column)
                .Select((p, i) => new { p, i })
                .SkipWhile(o => o.p != null)
                .FirstOrDefault()?.i;

            if (row == null)
                throw new ArgumentException($"Column {action.Column} is already full", nameof(action));

            // if this was the top row, then remove this column as an available action
            if (row >= NumRows - 1)
                availableActions.Remove(availableActions.FirstOrDefault(a => a.Column == action.Column));

            // TODO: prune the available winning runs list

            board[row.Value * action.Column] = CurrentPlayer;
            CurrentPlayer = CurrentPlayer.NextPlayer;
        }

        public ConnectFourPlayer GetWinner()
        {
            return availableWinningRuns
                .Select(run => run.Select(i => board[i]).ToList()) // convert indexes to board values
                .Where(run => run.IsDistinctNotNull()) // if there's a run that has all the same (non-null) player, it's a win
                .Select(run => run[0]) // so take the first one
                .FirstOrDefault(); // and return it
        }

        public double GetResult(ConnectFourPlayer forPlayer)
        {
            var winner = GetWinner();
            if (winner == forPlayer) return 1;
            if (winner == null) return 0.5;
            return 0;
        }

        public IEnumerable<ConnectFourPlayer> GetRow(int rowNum)
        {
            var startIndex = NumCols * rowNum;
            var endIndex = startIndex + NumCols;
            for(var i = startIndex; i < endIndex && i < board.Length; i++)
                yield return board[i];
        }

        public IEnumerable<ConnectFourPlayer> GetCol(int colNum)
        {
            for(var i = colNum; i < board.Length; i += NumCols)
                yield return board[i];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for(var rowNum = NumRows-1; rowNum >=0; rowNum--)
                sb.AppendJoin("", GetRow(rowNum).Select(c => c == null ? " " : c.ToString().Substring(0, 1)))
                    .Append("\n");
            return sb.ToString();
        }
    }

    public static class ExtensionMethods
    {
        public static bool IsDistinctNotNull<T>(this IEnumerable<T> source)
        {
            var hasEncounteredFirst = false;
            T first = default(T);

            foreach (var t in source)
            {
                if (!hasEncounteredFirst)
                {
                    hasEncounteredFirst = true;
                    first = t;
                }

                if (t == null || !ReferenceEquals(first, t))
                    return false;
            }

            return true;
        }
    }
}
