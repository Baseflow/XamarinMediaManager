---------------------------------
MediaManager for Xamarin.Forms
---------------------------------

1. Android

Call from MainActivity: CrossMediaManager.Current.Init(this);

2. Other platforms

Call from AppDelegate, etc: CrossMediaManager.Current.Init();

3. Add a VideoView to your Page

<mm:VideoView VerticalOptions="FillAndExpand" Source="Some url here" />

4. Or play media from code

await CrossMediaManager.Current.Play("Some url here");



Star on Github if this project helps you: https://github.com/martijn00/XamarinMediaManager