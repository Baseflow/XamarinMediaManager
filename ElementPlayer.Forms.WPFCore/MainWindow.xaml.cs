using MediaManager;
using MvvmCross.Platforms.Wpf.Views;

namespace ElementPlayer.Forms.WPFCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MvxWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            CrossMediaManager.Current.Init();
        }
    }
}
