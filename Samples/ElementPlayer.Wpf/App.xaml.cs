using MvvmCross.Core;
using MvvmCross.Platforms.Wpf.Views;

namespace ElementPlayer.Wpf
{
    public partial class App : MvxApplication
    {
        protected override void RegisterSetup()
        {
            base.RegisterSetup();
            this.RegisterSetupType<ElementPlayer.Wpf.MvxWpfSetup<ElementPlayer.Core.App>>();
        }
    }
}
