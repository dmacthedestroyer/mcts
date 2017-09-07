using System;
using System.Collections.Generic;
using System.Linq;

namespace MonteCarlo
{
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

            foreach(var element in source) {
                var value = selector(element);
                if(currentMaxValue < value)
                {
                    currentMaxValue = value;
                    currentMaxElement = element;
                }
            }

            return currentMaxElement;
        }
    }
}
