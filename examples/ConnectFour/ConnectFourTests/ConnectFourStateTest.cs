using ConnectFour;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ConnectFourTests
{
    [TestClass]
    public class ConnectFourStateTest
    {
        private static ConnectFourPlayer X = ConnectFourPlayer.X;
        private static ConnectFourPlayer O = ConnectFourPlayer.O;
        private static ConnectFourPlayer _ = null;

        private IEnumerable<ConnectFourState> GetStatesFromData(string dataFileName)
        {
            var fileProvider = new EmbeddedFileProvider(Assembly.GetAssembly(GetType()));

            var boards = fileProvider.ReadAllText(dataFileName)
                .SplitOnNewLine()
                .Batch(ConnectFourState.NumRows + 1);

            foreach(var rows in boards)
            {
                var board = rows
                    .Reverse() // reverse the order because the bottom-left position is actually the first element in the board state
                    .Skip(1) // skip the delimiter row at the bottom of the board (i.e. "-------")
                    .Select(row => row.PadRight(ConnectFourState.NumCols))
                    .SelectMany(s => s.Select(c => c == 'X' ? X : c == 'O' ? O : _))
                    .ToArray();

                var state = new ConnectFourState(board);
                yield return state;
            }
        }

        [TestMethod]
        public void GetResult_Rows()
        {
            var boards = GetStatesFromData("TestData.GetResult_Rows.txt");
            foreach(var state in boards)
            {
                Assert.AreEqual(1, state.GetResult(X), state.ToString());
                Assert.AreEqual(0, state.GetResult(O), state.ToString());
                Assert.IsFalse(state.Actions.Any());
            }
        }

        [TestMethod]
        public void GetResult_Columns()
        {
            var boards = GetStatesFromData("TestData.GetResult_Columns.txt");

            foreach(var state in boards)
            {
                Assert.AreEqual(1, state.GetResult(X), state.ToString());
                Assert.AreEqual(0, state.GetResult(O), state.ToString());
                Assert.IsFalse(state.Actions.Any());
            }
        }

        [TestMethod]
        public void GetResult_DiagonalBottomTop()
        {
            var boards = GetStatesFromData("TestData.GetResult_DiagonalBottomTop.txt");

            foreach (var state in boards)
            {
                Assert.AreEqual(1, state.GetResult(X), state.ToString());
                Assert.AreEqual(0, state.GetResult(O), state.ToString());
                Assert.IsFalse(state.Actions.Any());
            }
        }

        [TestMethod]
        public void GetResult_DiagonalTopBottom()
        {
            var boards = GetStatesFromData("TestData.GetResult_DiagonalTopBottom.txt");

            foreach (var state in boards)
            {
                Assert.AreEqual(1, state.GetResult(X), state.ToString());
                Assert.AreEqual(0, state.GetResult(O), state.ToString());
                Assert.IsFalse(state.Actions.Any());
            }
        }

        [TestMethod]
        public void FullRun_Simple()
        {
            var state = new ConnectFourState();
            foreach (var position in new[] { 0, 0, 1, 1, 2, 2, 3 })
                state.ApplyAction(new ConnectFourAction(position));

            Assert.IsFalse(state.Actions.Any());
            Assert.AreEqual(1, state.GetResult(X));
            Assert.AreEqual(0, state.GetResult(O));
        }
    }

    public static class ExtensionMethods
    {
        public static IEnumerable<string> SplitOnNewLine(this string source)
        {
            return source.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public static IEnumerable<IList<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            var batch = new List<T>(batchSize);
            foreach(var t in source)
            {
                batch.Add(t);
                if(batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>(batchSize);
                }
            }

            if (batch.Any())
                yield return batch;
        }

        public static string ReadAllText(this IFileProvider fileProvider, string fileName)
        {
            var file = fileProvider.GetFileInfo(fileName);
            using (var stream = file.CreateReadStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }
    }
}
