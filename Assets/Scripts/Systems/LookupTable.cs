using System;
using System.Collections.Generic;

namespace NeonBlack.Systems
{
    public class LookupTable<T1, T2>
    {
        private readonly Dictionary<T1, T2> dict = new();
        private readonly Func<T1, T2> func;

        public LookupTable(Func<T1, T2> func)
        {
            this.func = func;
        }

        public T2 Calculate(T1 value)
        {
            if (dict.TryGetValue(value, out var calculate))
            {
                return calculate;
            }

            var result = func(value);
            dict.Add(value, result);

            return result;
        }

        public void Clear()
        {
            dict.Clear();
        }
    }
}
