using System;
using System.Collections.Generic;
using System.Text;

namespace MonteCarlo
{
    public interface IState<TPlayer, TAction>
    {
        IState<TPlayer, TAction> Clone();

        TPlayer CurrentPlayer { get; }

        IList<TAction> Actions { get; }

        void ApplyAction(TAction action);

        double GetResult(TPlayer forPlayer);
    }
}
