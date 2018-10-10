# MediaManager - Cross platform media plugin for Xamarin and Windows
* Designed to be simple and easy to use
* Native playback of media files from remote http(s), embedded and local sources
* Native media notifications and remote controls
* Queue and playback management by default
* Playback status (Playing, Buffering, Loading, Paused, Progress)
* Events for media handling to hook into

## Setup & Usage
* Available on NuGet: https://www.nuget.org/packages/Plugin.MediaManager/
* Install into each project that utilizes MediaManager
* More information on the [Xamarin Blog](https://blog.xamarin.com/play-audio-and-video-with-the-mediamanager-plugin-for-xamarin/ )

## Build Status: 
[![Build status](https://ci.appveyor.com/api/projects/status/c9c6recwcu7k0s15?svg=true)](https://ci.appveyor.com/project/martijn00/xamarinmediamanager)
![GitHub tag](https://img.shields.io/github/tag/martijn00/XamarinMediaManager.svg)
[![NuGet](https://img.shields.io/nuget/v/Plugin.MediaManager.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.MediaManager/)
[![MyGet](https://img.shields.io/myget/martijn00/v/Plugin.MediaManager.svg)](https://www.myget.org/F/martijn00/api/v3/index.json)

**Platform Support**

|Platform|Supported|Version|Native|
| ------------------- | :-----------: | :------------------: |:------------------: |
|Xamarin.iOS|Yes|iOS 10+|AVPlayer|
|Xamarin.Android|Yes|API 16+|ExoPlayer|
|Windows 10 UWP|Yes|10+|MediaElement|
|Windows WPF|No|
|.Net Standard|Yes|2.0+|MediaManager|
|Xamarin.Mac|Yes|3.0+|AVPlayer|
|Xamarin.tvOS|Yes|10.0+|AVPlayer|
|Tizen|Yes|4.0+|
|Xamarin.Forms|Yes|3.2+|MediaManager|

## Installation

Add the NuGet package to all the projects you want to use it in.

* In Visual Studio - Tools > NuGet Package Manager > Manage Packages for Solution
* Select the Browse tab, search for MediaManager
* Select Plugin.MediaManager
* Install into each project within your solution

## Usage

Call **MediaManager.Current** from any .Net library or Xamarin project to gain access to APIs.

### Initialize plugin

Make sure to call Init() on startup of your app. Optionally provide the `Activity` on Android.

```csharp
CrossMediaManager.Current.Init();
```

### Play a single media item

```csharp
await CrossMediaManager.Current.Play("http://www.montemagno.com/sample.mp3");
```

### Play multiple media items

```csharp
await CrossMediaManager.Current.
```

### Retrive metadata for media

```csharp
await CrossMediaManager.Current.
```

### Add Video Player to the UI

```csharp
await CrossMediaManager.Current.
```

## Xamarin.Forms

```csharp
await CrossMediaManager.Current.
```
## Platform specific features

|Feature|Android|iOS, Mac, tvOS|UWP|Tizen|
| ------------------- | :-----------: | :------------------: |:------------------: |
|HLS|x|||
|DASH|x|||
|SmoothStreaming|x|||
|ChromeCast|x|||
|Apple TV||x||

## **IMPORTANT**
**Android:**

* You must request `AccessWifiState`, `Internet`, `MediaContentControl` and `WakeLock` permissions

**iOS:**

* In order for the audio to contiunue to play in the background you have to add the Audio and Airplay Background mode to your Info.plist
* If you want to enable RemoteControl features, you will have to override `UIApplication.RemoteControlReceived(UIEvent)` and forward the event to the `MediaManagerImplementation.MediaRemoteControl.RemoteControlReceived(UIEvent)` method. See the sample application for more details.
* If you are playing audio from a http resource you have to take care of [ATS](https://developer.xamarin.com/guides/ios/platform_features/introduction_to_ios9/ats/).
* If you want to display a artwork/cover that is embedded into an MP3 file, make sure that you use ID3 v2.3 (not v2.4).

**Tizen:**

* You must request `http://tizen.org/privilege/internet`, `http://tizen.org/privilege/mediastorage`, and `http://tizen.org/privilege/externalstorage` privileges

## Contributors
* [martijn00](https://github.com/martijn00)
* [modplug](https://github.com/modplug)
* [jmartine2](https://github.com/jmartine2)
* [SimonSimCity](https://github.com/SimonSimCity)
* [fela98](https://github.com/fela98)
* [bubavanhalen](https://github.com/bubavanhalen)
* [rookiejava](https://github.com/rookiejava)
