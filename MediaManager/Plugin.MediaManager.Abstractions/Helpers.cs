using System;
using System.Collections.Generic;

namespace Plugin.MediaManager.Abstractions
{
    public static class Helpers
    {
        public static void AddRange<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}

