using MediaManager;
using MvvmCross.Forms.Platforms.Uap.Core;
using MvvmCross.Forms.Platforms.Uap.Views;
using Windows.ApplicationModel.Activation;

namespace ElementPlayer.Forms.UWP
{
    sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs activationArgs)
        {
            CrossMediaManager.Current.Init();

            base.OnLaunched(activationArgs);
        }
    }

    public abstract class ElementPlayerApp : MvxWindowsApplication<MvxFormsWindowsSetup<Core.App, FormsApp>, Core.App, FormsApp, MainPage>
    {
    }
}
