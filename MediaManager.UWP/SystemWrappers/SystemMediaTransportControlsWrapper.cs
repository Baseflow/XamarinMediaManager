using Windows.Foundation;
using Windows.Media;

namespace Plugin.MediaManager.SystemWrappers
{
    class SystemMediaTransportControlsWrapper : ISystemMediaTransportControlsWrapper
    {
        private readonly SystemMediaTransportControls _controls;

        public SystemMediaTransportControlsWrapper(SystemMediaTransportControls controls)
        {
            _controls = controls;
        }

        public void SubscribeToMediaButtonEvents()
        {
            _controls.ButtonPressed += _controls_ButtonPressed;
        }

        public void UnsubscribeFromMediaButtonEvents()
        {
            _controls.ButtonPressed -= _controls_ButtonPressed;
        }

        private void _controls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            ButtonPressed?.Invoke(sender, new SystemMediaTransportControlsButtonPressedEventArgsWrapper(args));
        }

        public bool IsNextEnabled
        {
            get => _controls.IsNextEnabled;
            set => _controls.IsNextEnabled = value;
        }

        public bool IsPreviousEnabled
        {
            get => _controls.IsPreviousEnabled;
            set => _controls.IsPreviousEnabled = value;
        }

        public bool IsPauseEnabled
        {
            get => _controls.IsPauseEnabled;
            set => _controls.IsPauseEnabled = value;
        }

        public bool IsPlayEnabled
        {
            get => _controls.IsPlayEnabled;
            set => _controls.IsPlayEnabled = value;
        }

        public bool IsStopEnabled
        {
            get => _controls.IsStopEnabled;
            set => _controls.IsStopEnabled = value;
        }

        public event TypedEventHandler<SystemMediaTransportControls, ISystemMediaTransportControlsButtonPressedEventArgsWrapper> ButtonPressed;
    }
}