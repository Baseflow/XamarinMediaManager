using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Support.V4.Content;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.UI;
using MediaManager.Platforms.Android.Media;

namespace MediaManager.Platforms.Android.Notifications
{
    public class NotificationManager : NotificationManagerBase
    {
        protected MediaManagerImplementation MediaManager = CrossMediaManager.Android;

        public PlayerNotificationManager PlayerNotificationManager { get; set; }

        private IPlayer _player;
        internal IPlayer Player
        {
            get
            {
                return _player;
            }
            set
            {
                _player = value;
                PlayerNotificationManager?.SetPlayer(_player);
            }
        }

        public NotificationManager()
        {
        }

        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                if (Enabled)
                    Player = MediaManager.AndroidMediaPlayer.Player;
                else
                    Player = null;
            }
        }

        public override bool ShowPlayPauseControls
        {
            get => base.ShowPlayPauseControls;
            set
            {
                base.ShowPlayPauseControls = value;
                PlayerNotificationManager?.SetUsePlayPauseActions(ShowPlayPauseControls);
            }
        }

        public override bool ShowNavigationControls
        {
            get => base.ShowNavigationControls;
            set
            {
                base.ShowNavigationControls = value;
                PlayerNotificationManager?.SetUseNavigationActions(ShowNavigationControls);
            }
        }

        //TODO: Maybe add IMediaItem = null as parameter and allow to pass that to notification, otherwise use current queue item
        public override void UpdateNotification()
        {
            if (PlayerNotificationManager != null)
            {
                if (Enabled && Player == null && !MediaManager.IsStopped())
                    Player = MediaManager.AndroidMediaPlayer.Player;

                if (ShowNavigationControls && MediaManager.MediaQueue.Count > 1)
                {
                    PlayerNotificationManager.SetUseNavigationActions(true);
                }
                else
                {
                    PlayerNotificationManager.SetUseNavigationActions(false);
                }
                //TODO: Call PlayerNotificationManager.Invalidate(); on exoplayer 2.9.6 when metadata is updated
            }
        }
    }
}
