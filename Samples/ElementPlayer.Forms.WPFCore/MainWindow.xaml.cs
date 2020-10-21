using MediaManager;
using MvvmCross.Forms.Platforms.Wpf.Views;

namespace ElementPlayer.Forms.WPFCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MvxFormsWindowsPage
    {
        public MainWindow()
        {
            InitializeComponent();
            CrossMediaManager.Current.Init();
        }
    }
}
