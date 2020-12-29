using MvvmCross.Core;
using MvvmCross.Platforms.Wpf.Views;

namespace ElementPlayer.WpfCore
{
    public partial class App : MvxApplication
    {
        protected override void RegisterSetup()
        {
            base.RegisterSetup();
            this.RegisterSetupType<ElementPlayer.WpfCore.MvxWpfSetup<ElementPlayer.Core.App>>();
        }
    }
}
