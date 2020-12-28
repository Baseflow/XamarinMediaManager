using ElementPlayer.Core;
using MediaManager;
using MvvmCross.Platforms.Ios.Core;

namespace ElementPlayer.iOS
{
    public class Setup : MvxIosSetup<App>
    {
        protected override void InitializeLastChance()
        {
            CrossMediaManager.Current.Init();

            base.InitializeLastChance();
        }
    }
}
