using System;
using System.Collections.Generic;
using System.Text;
using MediaManager.Reactive;

namespace MediaManager
{
    public static partial class MediaManagerExtensions
    {
        private static ReactiveExtensions _reactive;
        private static ReactiveExtensions reactive
        {
            get
            {
                if(_reactive == null)
                    _reactive = new ReactiveExtensions();
                return _reactive;
            }
        }

        //TODO: Find a nicer way to do this.
        public static ReactiveExtensions Reactive (this IMediaManager mediaManager)
        {
            return reactive;
        }
    }
}
