using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;

namespace MediaManager.Platforms.Android.Audio
{
    internal class AudioFocusManager
    {
        Context context;
        AudioManager mAudioManager;
        AudioAttributes mAudioAttributes;
        AudioFocusListener mFocusListener;
        AudioFocusRequestClass mAudioFocusRequest;

        private const float MEDIA_VOLUME_DEFAULT = 1.0f;
        private const float MEDIA_VOLUME_DUCK = 0.2f;

        private IMediaPlayer player;

        private Com.Google.Android.Exoplayer2.SimpleExoPlayer exoPlayer
        {
            get
            {
                if (player is Utils.IExoPlayerPlayer)
                    return ((Utils.IExoPlayerPlayer)player).Player;

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

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                mAudioFocusRequest = new AudioFocusRequestClass
                    .Builder(AudioFocus.Gain)
                    .SetAudioAttributes(mAudioAttributes)
                    .SetAcceptsDelayedFocusGain(true)
                    .SetOnAudioFocusChangeListener(mFocusListener)
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
        }

        public void RequestAudioFocus()
        {
            AudioFocusRequest result;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                result = mAudioManager.RequestAudioFocus(mAudioFocusRequest);
            else
                //only when lower than Oreo (Android 8.0 / API 27).
                result = mAudioManager.RequestAudioFocus(mFocusListener, Stream.Music, AudioFocus.Gain);

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
                mAudioManager.AbandonAudioFocus(mFocusListener);

            exoPlayer.PlayWhenReady = false; //pause.
        }

        private bool IsPlaying()
        {
            return player.Status == Media.MediaPlayerStatus.Playing;
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
