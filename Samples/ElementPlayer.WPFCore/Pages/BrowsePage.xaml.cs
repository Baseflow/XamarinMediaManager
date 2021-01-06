using ElementPlayer.Core.ViewModels;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;

namespace ElementPlayer.WpfCore.Pages
{
    [MvxContentPresentation(WindowIdentifier = nameof(MainWindow), StackNavigation = true)]
    public partial class BrowsePage : MvxWpfView<BrowseViewModel>
    {
        public BrowsePage()
        {
            InitializeComponent();
        }
    }
}
