using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;

namespace ElementPlayer.WpfCore.Pages
{
    [MvxContentPresentation(WindowIdentifier = nameof(MainWindow), StackNavigation = true)]
    public partial class PlayerPage : MvxWpfView<PlayerViewModel>
    {
        public PlayerPage()
        {
            InitializeComponent();
        }
    }
}
