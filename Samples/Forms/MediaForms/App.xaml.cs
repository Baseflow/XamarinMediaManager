using Plugin.MediaManager.Forms;
using Xamarin.Forms;

namespace MediaForms
{
    public partial class App : Application
    {
        public App()
        {
            // Make sure it doesn't get stripped away by the linker
            var workaround = typeof(VideoView);
            InitializeComponent();
            //MainPage = new MediaFormsPage();

            MainPage = new NavigationPage(new HomePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
