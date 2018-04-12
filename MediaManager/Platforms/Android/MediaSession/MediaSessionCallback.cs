using System;
using Android.Content;
using Android.Views;

namespace Plugin.MediaManager.MediaSession
{

    using Android.Support.V4.Media.Session;
    
    public class MediaSessionCallback : MediaSessionCompat.Callback
    {
        private readonly long CLICK_DELAY = 400;
        private MediaSessionManager _manager;
        private long _lastClick = 0;

        
        public MediaSessionCallback(MediaSessionManager manager)
        {
            _manager = manager;
        }

        public override void OnPlay()
        {
            _manager.HandleAction(MediaServiceBase.ActionPlay);
            base.OnPlay();
        }

        public override void OnPause()
        {
            _manager.HandleAction(MediaServiceBase.ActionPause);
            base.OnPause();
        }

        public override void OnSkipToNext()
        {
            _manager.HandleAction(MediaServiceBase.ActionNext);
            base.OnSkipToNext();
        }

        public override void OnSkipToPrevious()
        {
            _manager.HandleAction(MediaServiceBase.ActionPrevious);
            base.OnSkipToPrevious();
        }

        public override void OnStop()
        {
            _manager.HandleAction(MediaServiceBase.ActionStop);
            base.OnStop();
        }
     
        public override void OnFastForward()
        {
            _manager.HandleAction(MediaServiceBase.ActionStop);
            base.OnFastForward();
        }


        /// <summary>
        /// Called when [media button event]. Handle Headphone double click
        /// </summary>
        /// <param name="mediaButtonEvent">The media button event.</param>
        /// <returns></returns>
        public override bool OnMediaButtonEvent(Intent mediaButtonEvent)
        {
            var isDoubleClick = false;
            var key = (KeyEvent)mediaButtonEvent.Extras.Get(Intent.ExtraKeyEvent);

            if (key.Action != KeyEventActions.Down)
            {
                var currentClick = DateTime.Now.Ticks;
                if (_lastClick != 0 && TimeSpan.FromTicks(currentClick).TotalMilliseconds - TimeSpan.FromTicks(_lastClick).TotalMilliseconds < CLICK_DELAY)
                    isDoubleClick = true;
                _lastClick = currentClick;
            }
   
            if (!isDoubleClick)
                return base.OnMediaButtonEvent(mediaButtonEvent);

            _lastClick = 0;
            _manager.HandleAction(MediaServiceBase.ActionNext);
            return true;
        }
    }
}