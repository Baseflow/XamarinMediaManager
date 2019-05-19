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
|Windows 10 UWP|Yes|10+|MediaElement|
|Windows WPF|No|

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

Optionally provide the `Activity` on Android.

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

### Retrieve metadata for media

Depending on the platform and the media item metadata will be extracted from ID3 data in the file.

```csharp
CrossMediaManager.Current.MediaQueue.Current.Title;
CrossMediaManager.Current.MediaQueue.Current.AlbumArt;
CrossMediaManager.Current.MediaQueue.Current.*
```

### Add Video Player to the UI

For android we need a videoview
```csharp
playerView = view.FindViewById<VideoView>(Resource.Id.exoplayerview_activity_video);
```

For iOS we need to add a VideoView either in code, or in a Xib or Storyboard.
```csharp
var playerView = new VideoView();
View.AddSubview(playerView);
```

Then for both android and iOS we have to add the player view to the mediaplayer
```csharp
CrossMediaManager.Current.MediaPlayer.SetPlayerView(playerView);
```

### Control the player 

```csharp
await CrossMediaManager.Current.Play();
await CrossMediaManager.Current.Pause();
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
```

### Retrieve information

```csharp
CrossMediaManager.Current.MediaPlayer.State == MediaPlayerState.Playing;
```

### Hook into events

```csharp
event StateChangedEventHandler StateChanged;
event PlayingChangedEventHandler PlayingChanged;
event BufferingChangedEventHandler BufferingChanged;
event MediaItemFinishedEventHandler MediaItemFinished;
event MediaItemChangedEventHandler MediaItemChanged;
event MediaItemFailedEventHandler MediaItemFailed;
event PositionChangedEventHandler PositionChanged;
```

## Xamarin.Forms

```csharp
await CrossMediaManager.Current.*
```

## Platform specific features

|Feature|Android|iOS, Mac, tvOS|UWP|Tizen|
| ------------------- | :-----------: | :------------------: | :------------------: |:------------------: |
|Audio|x|x|x|x|
|Video|x|x|x|x|
|Queue|x|x|x|x|
|Notifications|x|x|x|x|
|Volume|x|x|x|x|
|Media Extraction|x|x|x|x|
|HLS|x|x|||
|DASH|x||||
|SmoothStreaming|x||||
|ChromeCast|x||||
|Airplay||x|||
|Xamarin.Forms|x|x|x||

You can also directly access the native media player if you need it!
```csharp
//Access ExoPlayer on Android
CrossMediaManager.Android.NativeMediaPlayer.Player.VideoDecoderCounters
CrossMediaManager.Android.*
//iOS, MacOS or tvOS
CrossMediaManager.Apple.*
//UWP
CrossMediaManager.Windows.*
//Tizen
CrossMediaManager.Tizen.*
```

## Building the source code

* On Windows you need Visual Studio 2019 with the latest Xamarin, .NET Core and UWP installed.
* On Visual Studio for Mac 2019 multi-target is not supported. Therefor you need to compile from command line on a Mac. Simple go to the folder where the source code is and run: `msbuild MediaManager.sln /t:rebuild /p:Configuration=Release`

## **IMPORTANT**
**Android:**

* You must request `AccessWifiState`, `Internet`, `ForegroundService` and `WakeLock` permissions
* Your app must target Android SDK v28 or higher

**iOS:**

* In order for the audio to contiunue to play in the background you have to add the 'Audio, Airplay and Picture in Picture Background mode' and 'Background fetch' to your Info.plist


* If you want to enable RemoteControl features, you will have to override `UIApplication.RemoteControlReceived(UIEvent)` and forward the event to the `MediaManagerImplementation.MediaRemoteControl.RemoteControlReceived(UIEvent)` method. See the sample application for more details.
* If you are playing audio from a http resource you have to take care of [ATS](https://developer.xamarin.com/guides/ios/platform_features/introduction_to_ios9/ats/).
* If you want to display a artwork/cover that is embedded into an MP3 file, make sure that you use ID3 v2.3 (not v2.4).

**Tizen:**

* You must request `http://tizen.org/privilege/internet`, `http://tizen.org/privilege/mediastorage`, and `http://tizen.org/privilege/externalstorage` privileges
