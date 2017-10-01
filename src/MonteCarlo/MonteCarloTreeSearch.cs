using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonteCarlo
{
    public interface IAction {}

    public interface IPlayer {}

    public interface IState<TPlayer, TAction>
    {
        IState<TPlayer, TAction> Clone();

        TPlayer CurrentPlayer { get; }

        IList<TAction> Actions { get; }

        void ApplyAction(TAction action);

        double GetResult(TPlayer forPlayer);
    }

    public class MonteCarloTreeSearch
    {
        private class Node<TPlayer, TAction> : IMctsNode<TAction> where TPlayer : IPlayer where TAction: IAction
        {
            public Node(IState<TPlayer, TAction> state, TAction action = default(TAction), Node<TPlayer, TAction> parent = null)
            {
                this.Parent = parent;
                Player = state.CurrentPlayer;
                State = state;
                Action = action;
                UntriedActions = new HashSet<TAction>(state.Actions);
            }

            public Node<TPlayer, TAction> Parent { get; }

            public IList<Node<TPlayer, TAction>> Children { get; } = new List<Node<TPlayer, TAction>>();

            public int NumRuns { get; set; }

            public double NumWins { get; set; }

            public TPlayer Player { get; }

            public IState<TPlayer, TAction> State { get; }

            public TAction Action { get; }

            public ISet<TAction> UntriedActions { get; }

            public IList<TAction> Actions => State.Actions;

            private static double c = Math.Sqrt(2);

            public double ExploitationValue => NumWins / NumRuns;

            public double ExplorationValue => (Math.Sqrt(2*Math.Log(Parent.NumRuns) / NumRuns));

            private double UCT => ExploitationValue + ExplorationValue;

            public Node<TPlayer, TAction> SelectChild()
            {
                return Children.MaxElementBy(e => e.UCT);
            }

            public Node<TPlayer, TAction> AddChild(TAction action, IState<TPlayer, TAction> state)
            {
                var child = new Node<TPlayer, TAction>(state, action, this);
                UntriedActions.Remove(action);
                Children.Add(child);

                return child;
            }

            public void BuildTree(Func<int, long, bool> shouldContinue)
            {
                var iterations = 0;
                var timer = Stopwatch.StartNew();
                while (shouldContinue(iterations++, timer.ElapsedMilliseconds))
                {
                    var node = this;
                    var state = State.Clone();

                    //select
                    while (!node.UntriedActions.Any() && node.Actions.Any())
                    {
                        node = node.SelectChild();
                        state.ApplyAction(node.Action);
                    }

                    //expand
                    if (node.UntriedActions.Any())
                    {
                        var action = node.UntriedActions.RandomChoice();
                        state.ApplyAction(action);
                        node = node.AddChild(action, state);
                    }

                    //simulate
                    while (state.Actions.Any())
                        state.ApplyAction(state.Actions.RandomChoice());

                    //backpropagate
                    while (node != null)
                    {
                        node.NumRuns++;
                        node.NumWins += state.GetResult(this.Player);
                        node = node.Parent;
                    }
                }
            }

            public override string ToString()
            {
                return $"{NumWins}/{NumRuns}: ({ExploitationValue}/{ExplorationValue}={UCT})";
            }
        }

        public static IEnumerable<IMctsNode<TAction>> GetTopActions<TPlayer, TAction>(IState<TPlayer, TAction> state, int maxIterations) where TPlayer : IPlayer where TAction : IAction
        {
            return GetTopActions(state, maxIterations, long.MaxValue);
        }

        public static IEnumerable<IMctsNode<TAction>> GetTopActions<TPlayer, TAction>(IState<TPlayer, TAction> state, long timeBudget) where TPlayer : IPlayer where TAction : IAction
        {
            return GetTopActions(state, int.MaxValue, timeBudget);
        }

        public static IEnumerable<IMctsNode<TAction>> GetTopActions<TPlayer, TAction>(IState<TPlayer, TAction> state, int maxIterations, long timeBudget) where TPlayer : IPlayer where TAction : IAction
        {
            var root = new Node<TPlayer, TAction>(state);
            root.BuildTree((numIterations, elapsedMs) => numIterations < maxIterations && elapsedMs < timeBudget);
            return root.Children
                .OrderByDescending(n => n.NumRuns);
        }
    }

    public interface IMctsNode<TAction> where TAction : IAction
    {
        TAction Action { get; }

        int NumRuns { get; }

        double NumWins { get; }
    }

    internal static class CollectionExtensions
    {
        private static Random _random = new Random();

        public static T RandomChoice<T>(this ICollection<T> source, Random random = null)
        {
            var i = (random ?? _random).Next(source.Count);
            return source.ElementAt(i);
        }

        public static T MaxElementBy<T>(this IEnumerable<T> source, Func<T, double> selector)
        {
            var currentMaxElement = default(T);
            var currentMaxValue = double.MinValue;

            foreach (var element in source)
            {
                var value = selector(element);
                if (currentMaxValue < value)
                {
                    currentMaxValue = value;
                    currentMaxElement = element;
                }
            }

            return currentMaxElement;
        }
    }
}
