using System;
using Windows.Foundation;
using Windows.Media;

namespace Plugin.MediaManager.SystemWrappers
{
    public interface ISystemMediaTransportControlsWrapper
    {
        void SubscribeToMediaButtonEvents();

        void UnsubscribeFromMediaButtonEvents();

        bool IsNextEnabled { get; set; }

        bool IsPreviousEnabled { get; set; }

        bool IsPauseEnabled { get; set; }

        bool IsPlayEnabled { get; set; }

        event TypedEventHandler<SystemMediaTransportControls, ISystemMediaTransportControlsButtonPressedEventArgsWrapper> ButtonPressed;
    }
}