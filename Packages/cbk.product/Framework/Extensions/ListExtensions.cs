using System;
using System.Collections.Generic;

namespace CBK.Framework.Extensions
{
    public static class ListExtensions
    {
        public static void AddSorted<T>(this List<T> @this, T item, Comparison<T> comparison)
        {
            if (@this.Count == 0)
            {
                @this.Add(item);
                return;
            }

            if (comparison(@this[@this.Count - 1], item) <= 0)
            {
                @this.Add(item);
                return;
            }

            if (comparison(@this[0], item) >= 0)
            {
                @this.Insert(0, item);
                return;
            }

            var index = @this.BinarySearch(item, Comparer<T>.Create(comparison));
            if (index < 0)
                index = ~index;

            @this.Insert(index, item);
        }
    }
}