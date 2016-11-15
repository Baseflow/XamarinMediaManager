using Android.Runtime;
using MvvmCross.Droid.Shared.Attributes;
using MyMediaPlayer.Core.ViewModels;

namespace MyMediaPlayer.Droid.Fragments
{
    [MvxFragment(typeof(MainViewModel), Resource.Id.content_frame, true)]
    [Register("mymediaplayer.droid.fragments.AudioPlayerFragment")]
    public class AudioPlayerFragment : BaseFragment<AudioPlayerViewModel>
    {
        protected override int FragmentId => Resource.Layout.fragment_home;
    }
}
