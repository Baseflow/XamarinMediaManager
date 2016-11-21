using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Plugin.MediaManager.Abstractions.EventArguments;
using Plugin.MediaManager.Abstractions.Implementations;

namespace Plugin.MediaManager.Reactive
{
    public class ReactiveMediaManager : IDisposable
    {
        private readonly BehaviorSubject<bool> _canPlaySubject = new BehaviorSubject<bool>(false);
        private readonly CompositeDisposable _cd = new CompositeDisposable();

        private readonly BehaviorSubject<MediaPlayerStatus> _playbackStateSubject =
            new BehaviorSubject<MediaPlayerStatus>(MediaPlayerStatus.Stopped);

        private readonly BehaviorSubject<RepeatType> _repeatModeSubject =
            new BehaviorSubject<RepeatType>(RepeatType.None);

        private readonly BehaviorSubject<string> _titleSubject = new BehaviorSubject<string>(string.Empty);
        private string _filePath;

        public ReactiveMediaManager()
        {
            var bufferObservable = Observable.FromEventPattern<BufferingChangedEventHandler, BufferingChangedEventArgs>(
                    h => CrossMediaManager.Current.BufferingChanged += h,
                    h => CrossMediaManager.Current.BufferingChanged -= h)
                .Select(pattern => pattern.EventArgs);

            BufferedTime = bufferObservable.Select(e => e.BufferedTime);
            BufferedProgress = bufferObservable.Select(e => e.BufferProgress);

            PlaybackState = Observable.FromEventPattern<StatusChangedEventHandler, StatusChangedEventArgs>(
                    h => CrossMediaManager.Current.StatusChanged += h, h => CrossMediaManager.Current.StatusChanged -= h)
                .Select(pattern => pattern.EventArgs.Status);

            var playingObservable = Observable.FromEventPattern<PlayingChangedEventHandler, PlayingChangedEventArgs>(
                    h => CrossMediaManager.Current.PlayingChanged += h,
                    h => CrossMediaManager.Current.PlayingChanged -= h)
                .Select(pattern => pattern.EventArgs);

            PlaybackDuration = playingObservable.Select(e => e.Duration);
            PlaybackPosition = playingObservable.Select(e => e.Position);
            PlaybackProgress = playingObservable.Select(e => e.Progress);

            IsPlaying = PlaybackState.Select(status => status == MediaPlayerStatus.Playing);

            MediaFinished = Observable.FromEventPattern<MediaFinishedEventHandler, MediaFinishedEventArgs>(
                    h => CrossMediaManager.Current.MediaFinished += h, h => CrossMediaManager.Current.MediaFinished -= h)
                .Select(e => e.EventArgs.File);

            MediaFailed = Observable.FromEventPattern<MediaFileFailedEventHandler, MediaFailedEventArgs>(
                    h => CrossMediaManager.Current.MediaFileFailed += h,
                    h => CrossMediaManager.Current.MediaFileFailed -= h)
                .Select(e => e.EventArgs);

            MediaFileChanged = Observable.FromEventPattern<MediaFileChangedEventHandler, MediaFileChangedEventArgs>(
                    h => CrossMediaManager.Current.MediaFileChanged += h,
                    h => CrossMediaManager.Current.MediaFileChanged -= h)
                .Select(e => e.EventArgs.File);

            QueueEnded = Observable.FromEventPattern<QueueEndedEventHandler, QueueEndedEventArgs>(
                    h => CrossMediaManager.Current.MediaQueue.QueueEnded += h,
                    h => CrossMediaManager.Current.MediaQueue.QueueEnded -= h)
                .Select(e => true);

            ActiveItemInQueue = Observable.FromEventPattern<QueueMediaChangedEventHandler, QueueMediaChangedEventArgs>(
                    h => CrossMediaManager.Current.MediaQueue.QueueMediaChanged += h,
                    h => CrossMediaManager.Current.MediaQueue.QueueMediaChanged -= h)
                .Select(e => e.EventArgs.File);

            QueueChanged = Observable
                .FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                    h => CrossMediaManager.Current.MediaQueue.CollectionChanged += h,
                    h => CrossMediaManager.Current.MediaQueue.CollectionChanged -= h)
                .Select(e => e.EventArgs);
        }

        public IObservable<IMediaFile> MediaFileChanged { get; }
        public IObservable<NotifyCollectionChangedEventArgs> QueueChanged { get; private set; }
        public IObservable<IMediaFile> ActiveItemInQueue { get; private set; }
        public IObservable<bool> QueueEnded { get; private set; }
        public IObservable<MediaFailedEventArgs> MediaFailed { get; }
        public IObservable<IMediaFile> MediaFinished { get; }
        public IObservable<double> BufferedProgress { get; }
        public IObservable<TimeSpan> PlaybackPosition { get; }
        public IObservable<double> PlaybackProgress { get; }
        public IObservable<TimeSpan> BufferedTime { get; }
        public IObservable<MediaPlayerStatus> PlaybackState { get; }
        public IObservable<bool> IsPlaying { get; }
        public IObservable<RepeatType> RepeatMode => _repeatModeSubject;
        public IObservable<TimeSpan> PlaybackDuration { get; }
        public IObservable<string> Title => _titleSubject;

        public void Dispose()
        {
            _cd.Dispose();
        }

        public void SetRepeatMode(RepeatType repeatType)
        {
            CrossMediaManager.Current.MediaQueue.Repeat = repeatType;
            _repeatModeSubject.OnNext(repeatType);
        }

        public async Task Play(string filePath, MediaFileType mediaFileType, string title = null)
        {
            try
            {
                _filePath = filePath;
                if (_playbackStateSubject.Latest().First() == MediaPlayerStatus.Paused)
                {
                    await Pause();
                    return;
                }

                _canPlaySubject.OnNext(true);
                await CrossMediaManager.Current.Play(filePath, mediaFileType);
                _titleSubject.OnNext(title);
            }
            catch (Exception e)
            {
                _canPlaySubject.OnNext(false);
                Debug.WriteLine(e);
            }
        }

        public async Task Stop()
        {
            try
            {
                await CrossMediaManager.Current.Stop();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public async Task Pause()
        {
            try
            {
                await CrossMediaManager.Current.Pause();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void AddItemToQueue(IMediaFile mediaFile)
        {
            CrossMediaManager.Current.MediaQueue.Add(mediaFile);
        }

        public bool RemoveItemFromQueue(IMediaFile mediaFile)
        {
            return CrossMediaManager.Current.MediaQueue.Remove(mediaFile);
        }

        public void AddItemsToQueue(IEnumerable<IMediaFile> items)
        {
            CrossMediaManager.Current.MediaQueue.AddRange(items);
        }

        public void RemoveAt(int index)
        {
            CrossMediaManager.Current.MediaQueue.RemoveAt(index);
        }
    }
}