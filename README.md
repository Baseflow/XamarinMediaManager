# MediaManager - Cross platform media plugin for Xamarin and Windows
* Designed to be simple and easy to use
* Native playback of media files from remote http(s), embedded and local sources
* Native media notifications and remote controls
* Queue and playback management by default
* Playback status (Playing, Buffering, Loading, Paused, Progress)
* Events for media handling to hook into

## Status: 
[![Build status](https://ci.appveyor.com/api/projects/status/be78iac0bl7q930i?svg=true)](https://ci.appveyor.com/project/martijn00/xamarinmediamanager)
![GitHub tag](https://img.shields.io/github/tag/martijn00/XamarinMediaManager.svg)
[![NuGet](https://img.shields.io/nuget/v/Plugin.MediaManager.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.MediaManager/)
[![MyGet](https://img.shields.io/myget/martijn00/v/Plugin.MediaManager.svg)](https://www.myget.org/F/martijn00/api/v3/index.json)

**Platform Support**

|Platform|Supported|Version|Player|
| ------------------- | :-----------: | :------------------: |:------------------: |
|.Net Standard|Yes|2.0+|MediaManager|
|Xamarin.Forms|Yes|3.2+|MediaManager|
|Xamarin.Android|Yes|API 16+|ExoPlayer|
|Xamarin.iOS|Yes|iOS 10+|AVPlayer|
|Xamarin.Mac|Yes|3.0+|AVPlayer|
|Xamarin.tvOS|Yes|10.0+|AVPlayer|
|Tizen|Yes|4.0+|MediaPlayer|
|Windows 10 UWP|Yes|10+|MediaPlayer|
|Windows WPF|Yes|4.7.1+|MediaPlayer|

## Installation

Add the [NuGet package](https://www.nuget.org/packages/Plugin.MediaManager/) to all the projects you want to use it in.

* In Visual Studio - Tools > NuGet Package Manager > Manage Packages for Solution
* Select the Browse tab, search for MediaManager
* Select Plugin.MediaManager
* Install into each project within your solution

More information on the [Xamarin Blog](https://blog.xamarin.com/play-audio-and-video-with-the-mediamanager-plugin-for-xamarin/ )

## Usage

Call **MediaManager.Current** from any .Net library or Xamarin project to gain access to APIs.

### **IMPORTANT:** Initialize plugin

Make sure to call Init() in all the native platforms on startup of your app.

```csharp
CrossMediaManager.Current.Init();
```

Optionally provide the `Activity` on Android. This will also be used to bind the Android `Service` and will be used as `Intent` to launch from a notification.

```csharp
public class MainActivity : AppCompatActivity
{
	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		SetContentView(Resource.Layout.main_activity);

		CrossMediaManager.Current.Init(this);
	}
}
```

### Play a single media item

```csharp
//Audio
await CrossMediaManager.Current.Play("https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3");
//Video
await CrossMediaManager.Current.Play("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4");
```

### Play multiple media items

```csharp
public IList<string> Mp3UrlList => new[]{
	"https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3",
	"https://ia800605.us.archive.org/32/items/Mp3Playlist_555/CelineDion-IfICould.mp3",
	"https://ia800605.us.archive.org/32/items/Mp3Playlist_555/Daughtry-Homeacoustic.mp3",
	"https://storage.googleapis.com/uamp/The_Kyoto_Connection_-_Wake_Up/01_-_Intro_-_The_Way_Of_Waking_Up_feat_Alan_Watts.mp3",
	"https://aphid.fireside.fm/d/1437767933/02d84890-e58d-43eb-ab4c-26bcc8524289/d9b38b7f-5ede-4ca7-a5d6-a18d5605aba1.mp3"
	};

await CrossMediaManager.Current.Play(Mp3UrlList);
```

### Other play possibilities

```csharp
Task Play(IMediaItem mediaItem);
Task<IMediaItem> Play(string uri);
Task Play(IEnumerable<IMediaItem> items);
Task<IEnumerable<IMediaItem>> Play(IEnumerable<string> items);
Task<IMediaItem> Play(FileInfo file);
Task<IEnumerable<IMediaItem>> Play(DirectoryInfo directoryInfo);
```

### Control the player 

```csharp
await CrossMediaManager.Current.Play();
await CrossMediaManager.Current.Pause();
await CrossMediaManager.Current.PlayPause();
await CrossMediaManager.Current.Stop();

await CrossMediaManager.Current.StepForward();
await CrossMediaManager.Current.StepBackward();

await CrossMediaManager.Current.SeekToStart();
await CrossMediaManager.Current.SeekTo(TimeSpan position);
```

### Control the Queue

```csharp
await CrossMediaManager.Current.PlayPrevious();
await CrossMediaManager.Current.PlayNext();
await CrossMediaManager.Current.PlayPreviousOrSeekToStart();
await CrossMediaManager.Current.PlayQueueItem(IMediaItem mediaItem);

void ToggleRepeat();
void ToggleShuffle();
```

### Retrieve and set information

```csharp
Dictionary<string, string> RequestHeaders { get; set; }
TimeSpan StepSize { get; set; }
MediaPlayerState State { get; }
TimeSpan Position { get; }
TimeSpan Duration { get; }
TimeSpan Buffered { get; }
float Speed { get; set; }
RepeatMode RepeatMode { get; set; }
ShuffleMode ShuffleMode { get; set; }
bool IsPlaying();
bool IsBuffering();
```

### Hook into events

```csharp
event StateChangedEventHandler StateChanged;
event PlayingChangedEventHandler PlayingChanged;
event BufferingChangedEventHandler BufferingChanged;
event PositionChangedEventHandler PositionChanged;
event MediaItemFinishedEventHandler MediaItemFinished;
event MediaItemChangedEventHandler MediaItemChanged;
event MediaItemFailedEventHandler MediaItemFailed;
```

### Retrieve metadata for media

Depending on the platform and the media item metadata will be extracted from ID3 data in the file.

```csharp
CrossMediaManager.Current.MediaQueue.Current.Title;
CrossMediaManager.Current.MediaQueue.Current.AlbumArt;
CrossMediaManager.Current.MediaQueue.Current.*
```

Since the metadata might not be available immediately you can subscribe for updates like this:

```csharp
var mediaItem = await CrossMediaManager.Current.Play("https://ia800806.us.archive.org/15/items/Mp3Playlist_555/AaronNeville-CrazyLove.mp3");
mediaItem.MetadataUpdated += (sender, args) => {
	var title = args.MediaItem.Title;
};
```

Alternatively you could also use the `PropertyChanged` event to see updates to the metadata.

### Add Video Player to the UI

**On Xamarin.Forms the video view will automatically be attached to the player.**

For android we need a `VideoView` in the axml layout.
```xml
<mediamanager.platforms.android.video.VideoView
	android:id="@+id/your_videoview"
	android:layout_width="match_parent"
	android:layout_height="300dp" />
```

Then find the view in code:
```csharp
playerView = view.FindViewById<VideoView>(Resource.Id.your_videoview);
```

For iOS, MacOS or tvOS we need to add a `VideoView` either in code, or in a Xib or Storyboard.
```csharp
var playerView = new VideoView();
View.AddSubview(playerView);
```

Then for all platforms we have to add the player view to the `MediaPlayer`
```csharp
CrossMediaManager.Current.MediaPlayer.SetPlayerView(playerView);
```

## Xamarin.Forms

Adding a `VideoView` to a Page in Forms is easy as this:

```xml
<mm:VideoView VerticalOptions="FillAndExpand" Source="http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4" />
```

Your Xamarin.Forms page could look like this:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage
    xmlns="http://xamarin.com/schemas/2014/forms"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:mm="clr-namespace:MediaManager.Forms;assembly=MediaManager.Forms"
    x:Class="YourClassName" >
    <ContentPage.Content>
        <StackLayout>
            <mm:VideoView VerticalOptions="FillAndExpand" Source="http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4" ShowControls="False" />
        </StackLayout>
    </ContentPage.Content>
</ContentPage>
```

You can even use the normal `Play(object)` method and not set source. When you navigate to the view that contains the `VideoView`, the player will automatically attach to the view.

If you want a Page that contains a player you can open the `VideoPage`.

```csharp
Navigation.PushAsync(new MediaManager.Forms.VideoPage());
```

### Play a non standard format like HLS, Dash or SS

MediaManager will try to make a guess which media type or format is used. Sometimes this will not be picked up or be wrong, but you can enforce it by setting it yourself like this:

```csharp
var item = await CrossMediaManager.Current.MediaExtractor.CreateMediaItem("https://devstreaming-cdn.apple.com/videos/streaming/examples/bipbop_16x9/bipbop_16x9_variant.m3u8");
item.MediaType = MediaType.Hls;

await CrossMediaManager.Current.Play(item);
```

By enforcing it there is still no guarantee that the native system actually is able to play the item. 

## Platform specific features

|Feature|Android|iOS, Mac, tvOS|UWP|Tizen|WPF
| ------------------- | :-----------: | :------------------: | :------------------: |:------------------: |:------------------: |
|Audio|x|x|x|x|x|
|Video|x|x|x|x|x|
|Queue|x|x|x|x|x|
|Notifications|x|x|x|x|x|
|Volume|x|x|x|x|x|
|Media Extraction|x|x|x|x|x|
|HLS|x|x|||
|DASH|x||||
|SmoothStreaming|x||||
|ChromeCast|x||||
|Airplay||x|||
|Xamarin.Forms|x|x|x||x|

You can also directly access the native platform implementation if you need it!
```csharp
//Android
CrossMediaManager.Android.*
//iOS, MacOS or tvOS
CrossMediaManager.Apple.*
//UWP
CrossMediaManager.Windows.*
//Tizen
CrossMediaManager.Tizen.*
//WPF
CrossMediaManager.Wpf.*
```

## **IMPORTANT**
**Android:**

* You must request `AccessWifiState`, `Internet`, `ForegroundService` and `WakeLock` permissions
* Your app must target Android SDK v28 or higher

**iOS:**

* In order for the audio to contiunue to play in the background you have to add the 'Audio, Airplay and Picture in Picture Background mode' and 'Background fetch' to your Info.plist

```xml
<key>UIBackgroundModes</key>
<array>
	<string>audio</string>
	<string>fetch</string>
</array>
```

* If you are playing audio from a http resource you have to take care of [ATS](https://developer.xamarin.com/guides/ios/platform_features/introduction_to_ios9/ats/). Optionally you can disable this for playing media. Add the following to your info.plist:

```xml
<key>NSAppTransportSecurity</key>
<dict>
	<key>NSAllowsArbitraryLoadsInMedia</key>
	<true/>
</dict>
```

If you want to disable more you could add: `NSAllowsLocalNetworking` or even `NSAllowsArbitraryLoads` to disable all checks.

* If you want to display a artwork/cover that is embedded into an MP3 file, make sure that you use ID3 v2.3 (not v2.4).

**UWP:**

* In the Package.appxmanifest under capabilities you need to select: "Background Media Playback", "Internet"
* Optionally add "Music Library" and "Videos Library" as well if you use that

**Tizen:**

* You must request `http://tizen.org/privilege/internet`, `http://tizen.org/privilege/mediastorage`, and `http://tizen.org/privilege/externalstorage` privileges

## Building the source code

* On Windows you need Visual Studio 2019 with the latest Xamarin, .NET Core and UWP installed.
* On Visual Studio for Mac 2019 multi-target is not supported. Therefor you need to compile from command line on a Mac. Simple go to the folder where the source code is and run: `msbuild MediaManager.sln /t:rebuild /p:Configuration=Release`
