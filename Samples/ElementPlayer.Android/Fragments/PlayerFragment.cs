using Android.OS;
using Android.Views;
using ElementPlayer.Core.ViewModels;
using MediaManager;
using MediaManager.Platforms.Android.Video;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace ElementPlayer.Android.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.player_frame, false)]
    public class PlayerFragment : BaseFragment<PlayerViewModel>
    {
        private VideoView playerView;

        protected override int FragmentId => Resource.Layout.player_fragment;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            playerView = view.FindViewById<VideoView>(Resource.Id.exoplayerview_activity_video);
            CrossMediaManager.Current.MediaPlayer.VideoView = playerView;
            //CrossMediaManager.Current.Play(ViewModel.MediaItemToPlay);
        }
    }
}
