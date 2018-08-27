using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MediaManager.Media
{
    public class MediaQueue : ObservableCollection<IMediaItem>, IMediaQueue
    {
        List<IMediaItem> originalList { get; set; }

        private List<T> Randomize<T>(List<T> list)
        {
            List<T> randomizedList = new List<T>();
            Random rnd = new Random();
            while (list.Count > 0)
            {
                int index = rnd.Next(0, list.Count); //pick a random item from the master list
                randomizedList.Add(list[index]); //place it at the end of the randomized list
                list.RemoveAt(index);
            }
            return randomizedList;
        }

        public IMediaItem Current => this[0];
    }
}
