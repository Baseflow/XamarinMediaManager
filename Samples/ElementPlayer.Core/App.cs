using ElementPlayer.Core.ViewModels;
using MediaManager;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.ViewModels;

namespace ElementPlayer.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            Mvx.IoCProvider.RegisterSingleton(CrossMediaManager.Current);
            CrossMediaManager.Current.Library.Providers.Add(new MediaItemProvider());

            RegisterAppStart<HomeViewModel>();
            // if you want to use a custom AppStart, you should replace the previous line with this one:
            // RegisterCustomAppStart<MyCustomAppStart>();
        }
    }
}
