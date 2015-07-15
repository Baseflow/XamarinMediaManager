using System;
using Android.OS;

namespace MediaManager.Plugin
{
    public class MediaPlayerServiceBinder : Binder
    {
        private MediaPlayerService service;

        public MediaPlayerServiceBinder(MediaPlayerService service)
        {
            this.service = service;
        }

        public MediaPlayerService GetMediaPlayerService()
        {
            return service;
        }
    }
}

