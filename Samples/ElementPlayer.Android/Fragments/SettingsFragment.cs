using ElementPlayer.Core.ViewModels;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace ElementPlayer.Android.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, false)]
    public class SettingsFragment : BaseFragment<SettingsViewModel>
    {
        protected override int FragmentId => Resource.Layout.settings_fragment;
    }
}
