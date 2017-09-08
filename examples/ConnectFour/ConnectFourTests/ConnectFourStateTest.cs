using ConnectFour;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectFourTests
{
    [TestClass]
    public class ConnectFourStateTest
    {
        private static ConnectFourPlayer X = ConnectFourPlayer.X;
        private static ConnectFourPlayer Y = ConnectFourPlayer.O;
        private static ConnectFourPlayer _ = null;

        private static IList<ConnectFourPlayer> EmptyRow = Enumerable.Repeat(_, ConnectFourState.NumCols).ToList();

        /// <summary>
        /// creates a full game board by filling in missing coordinates with no player.  The string representation is from top-to-bottom, to make it easier to read.
        /// 
        /// i.e. the following string:
        /// 
        ///  XOX
        /// XXOO
        /// 
        /// will translate into:
        /// 
        /// [   X,    X,    O,    O, null, null, null,
        ///  null,    X,    O,    X, null, null, null,
        ///  null, null, null, null, null, null, null, 
        ///  null, null, null, null, null, null, null, 
        ///  null, null, null, null, null, null, null, 
        ///  null, null, null, null, null, null, null,]
        /// 
        /// </summary>
        /// <param name="boardStr"></param>
        /// <param name="currentPlayer"></param>
        /// <returns></returns>
        private static ConnectFourState ParseBoard(string boardStr, ConnectFourPlayer currentPlayer = null)
        {
            var board = boardStr.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Reverse()
                .TakeMin(ConnectFourState.NumRows, "")
                .SelectMany(row => row
                .PadRight(ConnectFourState.NumCols)
                .Select(c => c == 'X' ? X : c == 'Y' ? Y : _))
                .ToArray();

            return new ConnectFourState(board, currentPlayer);
        }

        [TestMethod]
        public void GetResult_Rows()
        {
            foreach(var b in new[]
            {
                "XXXX",
                " XXXX",
                "  XXXX",
                "   XXXX",

                "XXXX\n",
                " XXXX\n",
                "  XXXX\n",
                "   XXXX\n",

                "XXXX\n\n",
                " XXXX\n\n",
                "  XXXX\n\n",
                "   XXXX\n\n",

                "XXXX\n\n\n",
                " XXXX\n\n\n",
                "  XXXX\n\n\n",
                "   XXXX\n\n\n",

                "XXXX\n\n\n\n",
                " XXXX\n\n\n\n",
                "  XXXX\n\n\n\n",
                "   XXXX\n\n\n\n",
            })
            {
                var board = ParseBoard(b);
                Assert.AreEqual(1, board.GetResult(X), $"|{b}|");
                Assert.AreEqual(-1, board.GetResult(Y), $"|{b}|");
            }
        }
    }

    public static class ExtensionMethods
    {
        /// <summary>
        /// Iterates through all of <paramref name="source"/>, then will repeat <paramref name="pad"/> until the sequence is as long as <paramref name="minCount"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="minCount"></param>
        /// <param name="pad"></param>
        /// <returns></returns>
        public static IEnumerable<T> TakeMin<T>(this IEnumerable<T> source, int minCount, T pad)
        {
            var count = 0;
            foreach(var t in source)
            {
                yield return t;
                count++;
            }

            for (; count < minCount; count++)
                yield return pad;
        }
    }
}
