using System;
using System.Collections.Generic;

namespace Kontur.GameStats.Server.Extentions
{
    public static class ListExtentions
    {
        public static void AddOrUpdate<T1>(this List<T1> list, Func<T1, bool> selector, Func<T1> creator,
            Action<T1> updator)
        {
            var index = list.FindIndex(x => selector(x));
            if (index == -1)
                list.Add(creator());
            else
                updator(list[index]);
        }
    }
}
