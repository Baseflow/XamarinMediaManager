using System;
using System.Linq;
using Android.Graphics;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Metadata;
using Com.Google.Android.Exoplayer2.Metadata.Id3;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;

namespace MediaManager.Platforms.Android.Media
{
    public class PlayerEventListener : Java.Lang.Object, IPlayerEventListener
    {
        public PlayerEventListener()
        {
        }

        protected PlayerEventListener(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public Action<TrackGroupArray, TrackSelectionArray> OnTracksChangedImpl { get; set; }
        public Action<int> OnPositionDiscontinuityImpl { get; set; }
        public Action<bool, int> OnPlayerStateChangedImpl { get; set; }
        public Action<ExoPlaybackException> OnPlayerErrorImpl { get; set; }
        public Action<bool> OnLoadingChangedImpl { get; set; }
        public Action<PlaybackParameters> OnPlaybackParametersChangedImpl { get; set; }
        public Action<int> OnRepeatModeChangedImpl { get; set; }
        public Action OnSeekProcessedImpl { get; set; }
        public Action<bool> OnShuffleModeEnabledChangedImpl { get; set; }
        public Action<Timeline, Java.Lang.Object, int> OnTimelineChangedImpl { get; set; }

        public void OnTracksChanged(TrackGroupArray trackGroups, TrackSelectionArray trackSelections)
        {
            for (int i = 0; i < trackGroups.Length; i++)
            {
                TrackGroup trackGroup = trackGroups.Get(i);
                for (int j = 0; j < trackGroup.Length; j++)
                {
                    Metadata trackMetadata = trackGroup.GetFormat(j).Metadata;

                    if (trackMetadata != null)
                    {
                        for (int v = 0; v < trackMetadata.Length(); v++)
                        {
                            Metadata.IEntry entry = trackMetadata.Get(v);

                            switch (entry)
                            {
                                case ApicFrame apicFrame:
                                    byte[] bitmapData = apicFrame.PictureData.ToArray();
                                    Bitmap bitmap = BitmapFactory.DecodeByteArray(bitmapData, 0, bitmapData.Length);
                                    var test1 = apicFrame.MimeType;
                                    break;
                                case BinaryFrame binaryFrame:
                                    var test2 = binaryFrame.Data;
                                    break;
                                case ChapterFrame chapterFrame:
                                    var test3 = chapterFrame.ChapterId;
                                    break;
                                case ChapterTocFrame chapterTocFrame:
                                    var test4 = chapterTocFrame.IsRoot;
                                    break;
                                case CommentFrame commentFrame:
                                    var test5 = commentFrame.Text;
                                    break;
                                case GeobFrame geobFrame:
                                    var test6 = geobFrame.Data;
                                    break;
                                case InternalFrame internalFrame:
                                    var test7 = internalFrame.Text;
                                    break;
                                case PrivFrame privFrame:
                                    var test8 = privFrame.PrivateData;
                                    break;
                                case TextInformationFrame textInformationFrame:
                                    var test9 = textInformationFrame.Value;
                                    break;
                                case UrlLinkFrame urlLinkFrame:
                                    var test10 = urlLinkFrame.Url;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }

            OnTracksChangedImpl?.Invoke(trackGroups, trackSelections);
        }

        public void OnPositionDiscontinuity(int reason)
        {
            OnPositionDiscontinuityImpl?.Invoke(reason);
        }

        public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
        {
            OnPlayerStateChangedImpl?.Invoke(playWhenReady, playbackState);
        }

        public void OnPlayerError(ExoPlaybackException error)
        {
            OnPlayerErrorImpl?.Invoke(error);
        }

        public void OnLoadingChanged(bool isLoading)
        {
            OnLoadingChangedImpl?.Invoke(isLoading);
        }

        public void OnPlaybackParametersChanged(PlaybackParameters playbackParameters)
        {
            OnPlaybackParametersChangedImpl?.Invoke(playbackParameters);
        }

        public void OnRepeatModeChanged(int repeatMode)
        {
            OnRepeatModeChangedImpl?.Invoke(repeatMode);
        }

        public void OnSeekProcessed()
        {
            OnSeekProcessedImpl?.Invoke();
        }

        public void OnShuffleModeEnabledChanged(bool shuffleModeEnabled)
        {
            OnShuffleModeEnabledChangedImpl?.Invoke(shuffleModeEnabled);
        }

        public void OnTimelineChanged(Timeline timeline, Java.Lang.Object manifest, int reason)
        {
            OnTimelineChangedImpl?.Invoke(timeline, manifest, reason);
        }
    }
}
