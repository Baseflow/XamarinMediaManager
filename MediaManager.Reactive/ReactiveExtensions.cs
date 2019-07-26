using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using MediaManager.Playback;
using MediaManager.Player;

namespace MediaManager.Reactive
{
    public class ReactiveExtensions : IDisposable
    {
        private readonly CompositeDisposable _cd = new CompositeDisposable();

        public ReactiveExtensions()
        {


            State = Observable.FromEventPattern<StateChangedEventHandler, StateChangedEventArgs>(
                    h => CrossMediaManager.Current.StateChanged += h, h => CrossMediaManager.Current.StateChanged -= h)
                .Select(pattern => pattern.EventArgs.State);

            Position = Observable.FromEventPattern<PositionChangedEventHandler, PositionChangedEventArgs>(
                    h => CrossMediaManager.Current.PositionChanged += h, h => CrossMediaManager.Current.PositionChanged -= h)
                .Select(pattern => pattern.EventArgs.Position);

            Buffered = Observable.FromEventPattern<BufferedChangedEventHandler, BufferedChangedEventArgs>(
                    h => CrossMediaManager.Current.BufferedChanged += h, h => CrossMediaManager.Current.BufferedChanged -= h)
                .Select(pattern => pattern.EventArgs.Buffered);
        }

        public IObservable<MediaPlayerState> State { get; }

        public IObservable<TimeSpan> Position { get; }

        public IObservable<TimeSpan> Buffered { get; }

        public void Dispose()
        {
            _cd.Dispose();
        }
    }
}
