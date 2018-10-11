using Android.OS;
using Android.Views;
using Com.Google.Android.Exoplayer2.UI;
using ElementPlayer.Core.ViewModels;
using MediaManager;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace ElementPlayer.Android.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.content_frame, false)]
    public class PlayerFragment : BaseFragment<PlayerViewModel>
    {
        private PlayerView playerView;

        protected override int FragmentId => Resource.Layout.player_fragment;

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            playerView = view.FindViewById<PlayerView>(Resource.Id.exoplayerview_activity_video);
            CrossMediaManager.Current.MediaPlayer.SetPlayerView(playerView);
        }
    }
}
