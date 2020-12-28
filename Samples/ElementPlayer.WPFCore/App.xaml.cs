using System.Windows;
using MediaManager;

namespace ElementPlayer.WPFCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CrossMediaManager.Current.Init();
        }
    }
}
