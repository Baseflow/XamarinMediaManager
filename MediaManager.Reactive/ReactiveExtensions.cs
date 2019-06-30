using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using MediaManager.Playback;

namespace MediaManager.Reactive
{
    public class ReactiveExtensions : IDisposable
    {
        public ReactiveExtensions()
        {
            State = Observable.FromEventPattern<StateChangedEventHandler, StateChangedEventArgs>(
                    h => CrossMediaManager.Current.StateChanged += h, h => CrossMediaManager.Current.StateChanged -= h)
                .Select(pattern => pattern.EventArgs.State);
        }

        public IObservable<MediaPlayerState> State { get; }

        public void Dispose()
        {

        }
    }
}
