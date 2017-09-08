using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe;
using Xunit;

namespace TicTacToeTests
{
    public class TicTacToeStateTest
    {
        private static void AssertActionsEqual(IList<TicTacToeAction> expected, IList<TicTacToeAction> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            foreach(var a in expected)
                Assert.True(actual.Any(aa => aa.Player == a.Player && aa.Position == a.Position));
            foreach (var aa in actual)
                Assert.True(expected.Any(a => a.Player == aa.Player && a.Position == aa.Position));
        }

        [Fact]
        public void ToString_DoesntLookTooStupid()
        {
            var expected = "0|1|2\n-----\nX|X|X\n-----\nO|O|8\n-----\n";
            var actual = new TicTacToeState(new[] {
                null, null, null,
                TicTacToePlayer.X, TicTacToePlayer.X, TicTacToePlayer.X,
                TicTacToePlayer.O, TicTacToePlayer.O, null },
                TicTacToePlayer.O)
                .ToString();
            Xunit.Assert.Equal(expected, actual);
        }

        [Fact]
        public void Actions_EmptyBoard_9PossibleActionsForXPlayer()
        {
            var expected = Enumerable.Range(0, 9).Select(i => new TicTacToeAction(i, TicTacToePlayer.X)).ToList();
            var actual = new TicTacToeState().Actions.ToList();

            AssertActionsEqual(expected, actual);
        }

        [Fact]
        public void Actions_ExcludesOptionsAlreadyPlayed()
        {
            var expected = new[] { 2, 3, 4, 7, 8 }.Select(position => new TicTacToeAction(position, TicTacToePlayer.O)).ToList();
            var actual = new TicTacToeState(new[] {
                TicTacToePlayer.X, TicTacToePlayer.O, null,
                null, null, TicTacToePlayer.X,
                TicTacToePlayer.O, null, null
            },
            TicTacToePlayer.O)
            .Actions.ToList();

            AssertActionsEqual(expected, actual);
        }

        [Fact]
        public void Actions_NoneWhenWinnerOnBoard()
        {
            var expected = new TicTacToeAction[0];
            var actual = new TicTacToeState(new[] { TicTacToePlayer.X, TicTacToePlayer.X, TicTacToePlayer.X, null, null, null, null, null, null }, TicTacToePlayer.O).Actions.ToList();
            AssertActionsEqual(expected, actual);
        }

        [Fact]
        public void Actions_DebugThisSetup()
        {
            var state = new TicTacToeState(new[] {
                TicTacToePlayer.X, null, null,
                null, null, null,
                null, null, null}, TicTacToePlayer.O);
            var actual = state.Actions;
            var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8 }.Select(position => new TicTacToeAction(position, TicTacToePlayer.O)).ToList();
            AssertActionsEqual(expected, actual);
        }

        [Fact]
        public void ApplyAction_ModifiesAvailableActions()
        {
            var game = new TicTacToeState();

            for(var i = 9; i>2; i--)
            {
                Assert.Equal(i, game.Actions.Count);
                game.ApplyAction(game.Actions[0]);
            }
        }

        [Fact]
        public void GetResult_BoardHasWin_DifferentValueForEachPlayer()
        {
            var game = new TicTacToeState(new[]
            {
                TicTacToePlayer.X, TicTacToePlayer.X, TicTacToePlayer.X,
                TicTacToePlayer.O, TicTacToePlayer.O, null,
                TicTacToePlayer.O, null, null
            }, TicTacToePlayer.X);

            Assert.Equal(1, game.GetResult(TicTacToePlayer.X));
            Assert.Equal(0, game.GetResult(TicTacToePlayer.O));
        }

        [Fact]
        public void GetResult_BoardIsTie_EqualsHalf()
        {
            var game = new TicTacToeState(new[]
            {
                TicTacToePlayer.X, TicTacToePlayer.X, TicTacToePlayer.O,
                TicTacToePlayer.O, TicTacToePlayer.X, TicTacToePlayer.X,
                TicTacToePlayer.X, TicTacToePlayer.O, TicTacToePlayer.O
            }, TicTacToePlayer.O);

            Assert.Equal(0.5, game.GetResult(TicTacToePlayer.X));
            Assert.Equal(0.5, game.GetResult(TicTacToePlayer.O));
        }
    }
}
