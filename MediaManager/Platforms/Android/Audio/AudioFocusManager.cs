using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Com.Google.Android.Exoplayer2;

namespace MediaManager.Platforms.Android.Audio
{
    public class AudioFocusManager
    {
        Context context;
        AudioManager mAudioManager;
        AudioAttributes mAudioAttributes;
        AudioFocusListener mFocusListener;
        AudioFocusRequestClass mAudioFocusRequest;

        private const float MEDIA_VOLUME_DEFAULT = 1.0f;
        private const float MEDIA_VOLUME_DUCK = 0.2f;

        private IMediaPlayer player;

        private SimpleExoPlayer exoPlayer
        {
            get
            {
                if (player is Utils.IExoPlayerPlayer exoPlayerPlayer)
                    return exoPlayerPlayer.Player as SimpleExoPlayer;

                return null;
            }
        }

        public bool ShouldPlayWhenReady { get; private set; }

        public AudioFocusManager(IMediaPlayer player)
        {
            this.player = player;
            context = global::Android.App.Application.Context;
            mAudioManager = (AudioManager)context.GetSystemService(Context.AudioService);

            mAudioAttributes = new AudioAttributes.Builder()
                .SetUsage(AudioUsageKind.Media)
                .SetContentType(AudioContentType.Music)
                .Build();

            mFocusListener = new AudioFocusListener
            {
                OnAudioFocusChangeImpl = (focusChange) =>
                {
                    switch (focusChange)
                    {
                        case AudioFocus.Gain:
                            if (ShouldPlayWhenReady && !IsPlaying())
                                exoPlayer.PlayWhenReady = true; //play.
                            else if (IsPlaying())
                                exoPlayer.Volume = MEDIA_VOLUME_DEFAULT;

                            ShouldPlayWhenReady = false;
                            break;
                        case AudioFocus.LossTransientCanDuck:
                            exoPlayer.Volume = MEDIA_VOLUME_DUCK;
                            break;
                        case AudioFocus.LossTransient:
                            if (IsPlaying())
                            {
                                ShouldPlayWhenReady = true;

                                exoPlayer.PlayWhenReady = false; //pause.
                            }
                            break;
                        case AudioFocus.Loss:
                            AbandonAudioFocus();
                            ShouldPlayWhenReady = false;
                            break;
                    }
                }
            };

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                mAudioFocusRequest = new AudioFocusRequestClass
                    .Builder(AudioFocus.Gain)
                    .SetAudioAttributes(mAudioAttributes)
                    .SetAcceptsDelayedFocusGain(true)
                    .SetOnAudioFocusChangeListener(mFocusListener)
                    .Build();
        }

        public void RequestAudioFocus()
        {
            AudioFocusRequest result;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                result = mAudioManager.RequestAudioFocus(mAudioFocusRequest);
            else
                //only when lower than Oreo (Android 8.0 / API 27).
#pragma warning disable CS0618 // Type or member is obsolete
                result = mAudioManager.RequestAudioFocus(mFocusListener, Stream.Music, AudioFocus.Gain);
#pragma warning restore CS0618 // Type or member is obsolete

                // Call the listener whenever focus is granted - even the first time!
            if (result == AudioFocusRequest.Granted)
            {
                ShouldPlayWhenReady = true;
                mFocusListener.OnAudioFocusChange(AudioFocus.Gain);
            }
        }

        public void AbandonAudioFocus()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                mAudioManager.AbandonAudioFocusRequest(mAudioFocusRequest);
            else
#pragma warning disable CS0618 // Type or member is obsolete
                mAudioManager.AbandonAudioFocus(mFocusListener);
#pragma warning restore CS0618 // Type or member is obsolete

            exoPlayer.PlayWhenReady = false; //pause.
        }

        private bool IsPlaying()
        {
            return player.State == Media.MediaPlayerState.Playing;
        }

        public bool GetPlayWhenReady()
        {
            return exoPlayer.PlayWhenReady || ShouldPlayWhenReady;
        }

        private class AudioFocusListener : Java.Lang.Object, AudioManager.IOnAudioFocusChangeListener
        {
            public Action<AudioFocus> OnAudioFocusChangeImpl { get; set; }

            public void OnAudioFocusChange([GeneratedEnum] AudioFocus focusChange)
            {
                OnAudioFocusChangeImpl?.Invoke(focusChange);
            }
        }
    }
}
