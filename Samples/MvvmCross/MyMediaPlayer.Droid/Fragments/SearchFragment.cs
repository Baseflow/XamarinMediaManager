using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using MvvmCross.Droid.Shared.Attributes;
using MyMediaPlayer.Core.ViewModels;

namespace MyMediaPlayer.Droid.Fragments
{
    [MvxFragment(typeof(MainViewModel), Resource.Id.content_frame, true)]
    [Register("mymediaplayer.droid.fragments.SearchFragment")]
    public class SearchFragment : BaseFragment<SearchViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_home;
    }
}
