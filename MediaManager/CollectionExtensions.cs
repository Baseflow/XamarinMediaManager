namespace MediaManager
{
    public static class CollectionExtensions
    {
        public static int[] GetShuffleExchanges(int size, int key)
        {
            int[] exchanges = new int[size - 1];
            var rand = new Random(key);
            for (int i = size - 1; i > 0; i--)
            {
                int n = rand.Next(i + 1);
                exchanges[size - 1 - i] = n;
            }
            return exchanges;
        }

        public static void Shuffle<T>(this IList<T> list, int key)
        {
            int size = list.Count;
            var exchanges = GetShuffleExchanges(size, key);
            for (int i = size - 1; i > 0; i--)
            {
                int n = exchanges[size - 1 - i];
                var tmp = list[i];
                list[i] = list[n];
                list[n] = tmp;
            }
        }

        public static void DeShuffle<T>(this IList<T> list, int key)
        {
            int size = list.Count;
            var exchanges = GetShuffleExchanges(size, key);
            for (int i = 1; i < size; i++)
            {
                int n = exchanges[size - i - 1];
                var tmp = list[i];
                list[i] = list[n];
                list[n] = tmp;
            }
        }
    }
}
