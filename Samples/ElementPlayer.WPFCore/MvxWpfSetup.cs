using MvvmCross.ViewModels;

namespace ElementPlayer.WpfCore
{
    public class MvxWpfSetup<TApplication> : MvvmCross.Platforms.Wpf.Core.MvxWpfSetup<TApplication>
        where TApplication : class, IMvxApplication, new()

    {
        protected override void InitializeFirstChance()
        {
            CrossMediaManager.Current.Init();

            base.InitializeFirstChance();
        }
    }
}
