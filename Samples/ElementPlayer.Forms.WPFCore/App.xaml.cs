using MediaManager;
using MvvmCross.Core;
using MvvmCross.Forms.Platforms.Wpf.Core;

namespace ElementPlayer.Forms.WPFCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : MvvmCross.Platforms.Wpf.Views.MvxApplication
    {
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<MvxFormsWpfSetup<Core.App, FormsApp>>();

            CrossMediaManager.Current.Init();
        }
    }
}
