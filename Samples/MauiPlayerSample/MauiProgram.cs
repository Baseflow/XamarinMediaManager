using MediaManager.Forms;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace MauiPlayerSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                }).
                UseMauiCompatibility()
                        .ConfigureMauiHandlers((handlers) => {
#if ANDROID
                            handlers.AddCompatibilityRenderer(typeof(VideoView), typeof(MediaManager.Forms.Platforms.Android.VideoViewRenderer));
#endif

#if IOS
                            handlers.AddCompatibilityRenderer(typeof(VideoView), typeof(MediaManager.Forms.Platforms.iOS.VideoViewRenderer));
#endif
                        });

            return builder.Build();
        }
    }
}
