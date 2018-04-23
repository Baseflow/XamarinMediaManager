using Plugin.MediaManager.Abstractions;
using Plugin.MediaManager.Abstractions.Enums;
using Tizen.Applications;

namespace Plugin.MediaManager
{
    public class MediaNotificationManagerImplementation : IMediaNotificationManager
    {
        readonly ToastMessage _toast;

        public MediaNotificationManagerImplementation()
        {
            _toast = new ToastMessage();
        }

        public void StartNotification(IMediaFile mediaFile)
        {
            Log.Warn("Tizen 4.0 does not support the Minicontrol(Notification Extension) feature.");
            ShowToast("Start playing : " + mediaFile.Url);
        }

        public void StopNotifications()
        {
            Log.Warn("Tizen 4.0 does not support the Minicontrol(Notification Extension) feature.");
            ShowToast("Stop playing");
        }

        public void UpdateNotifications(IMediaFile mediaFile, MediaPlayerStatus status)
        {
            Log.Warn("Tizen 4.0 does not support the Minicontrol(Notification Extension) feature.");
        }

        private void ShowToast(string message)
        {
            _toast.Message = message;
            _toast.Post();
        }
    }
}