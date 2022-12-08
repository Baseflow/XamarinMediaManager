using MediaManager;

namespace MauiPlayerSample
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //CrossMediaManager.Current.Play("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4");
        }
    }
}
