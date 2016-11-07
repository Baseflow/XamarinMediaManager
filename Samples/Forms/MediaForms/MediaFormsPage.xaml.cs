using Plugin.MediaManager;
using Xamarin.Forms;

namespace MediaForms
{
    public partial class MediaFormsPage : ContentPage
    {
        public MediaFormsPage()
        {
            InitializeComponent();
			CrossMediaManager.Current.PlayingChanged += (sender, e) =>
			{
				ProgressBar.Progress = e.Progress;
				Duration.Text = "" + e.Duration.TotalSeconds.ToString() + " seconds";
			};
        }

		void PlayClicked(object sender, System.EventArgs e)
		{
			CrossMediaManager.Current.PlayPause();
		}

		void PauseClicked(object sender, System.EventArgs e)
		{
			CrossMediaManager.Current.Pause();
		}

		void StopClicked(object sender, System.EventArgs e)
		{
			CrossMediaManager.Current.Stop();
		}
    }
}
