using System.Windows.Input;
using MediaManager.Player;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MediaManager.Forms.Xaml
{
    public abstract class MediaExtensionBase : IMarkupExtension<ICommand>
    {
        protected IMediaManager MediaManager { get; }
        private Command _command { get; }

        protected MediaExtensionBase()
        {
            MediaManager = CrossMediaManager.Current;
            _command = new Command(Execute, CanExecute);
            MediaManager.StateChanged += (s, e) => RaiseCanExecuteChanged();
        }

        protected virtual bool CanExecute() =>
            !IsLoadingOrFaulted();

        protected abstract void Execute();

        public ICommand ProvideValue(IServiceProvider serviceProvider) =>
            _command;

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) =>
            ProvideValue(serviceProvider);

        protected bool IsLoadingOrFaulted()
        {
            switch (MediaManager.State)
            {
                case MediaPlayerState.Buffering:
                case MediaPlayerState.Failed:
                case MediaPlayerState.Loading:
                    return true;
                default:
                    return false;
            }
        }

        protected void RaiseCanExecuteChanged() =>
            _command.ChangeCanExecute();
    }
}
