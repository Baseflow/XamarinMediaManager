using System.Windows;
using MvvmCross.Core;
using MvvmCross.Forms.Platforms.Wpf.Core;
//using MvvmCross.Platforms.Wpf.Views;

namespace ElementPlayer.Forms.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : MvvmCross.Platforms.Wpf.Views.MvxApplication
    {
        protected override void RegisterSetup()
        {
            this.RegisterSetupType<MvxFormsWpfSetup<Core.App, FormsApp>>();
        }
    }
}
