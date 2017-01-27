## MediaManager - Cross platform media plugin for Xamarin and Windows
* Designed to be simple and easy to use
* Stand alone for easy integration with existing projects and frameworks
* Native plackback of media file
* Native notifications and remote controls

### Setup & Usage
* Available on NuGet: https://www.nuget.org/packages/Plugin.MediaManager/
* Install into each project that utilizes MediaManager

### Build Status: 
[![Build status](https://ci.appveyor.com/api/projects/status/c9c6recwcu7k0s15?svg=true)](https://ci.appveyor.com/project/martijn00/xamarinmediamanager)
![GitHub tag](https://img.shields.io/github/tag/martijn00/Plugin.MediaManager.svg)
[![NuGet](https://img.shields.io/nuget/v/Plugin.MediaManager.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.MediaManager/)
[![MyGet](https://img.shields.io/myget/martijn00/v/Plugin.MediaManager.svg)](https://www.myget.org/F/Plugin.MediaManager/api/v2)

**Platform Support**

|Platform|Supported|Version|
| ------------------- | :-----------: | :------------------: |
|Xamarin.iOS|Yes|iOS 7+|
|Xamarin.Android|Yes|API 9+|
|Windows Phone Silverlight|No||
|Windows Phone RT|No||
|Windows Store RT|No||
|Windows 10 UWP|Yes|10+|
|Windows WPF|No|
|.Net Framework|Yes|4.5|
|.Net Standard|Future|
|Xamarin.Mac|Yes|3.0+|
|Xamarin.tvOS|Yes|10.0+|

### Example Usage

### Add the NuGet package to your PCL 
* In Visual Studio - Tools > NuGet Package Manager > Manage Packages for Solution
* Select the Browse tab, search for MediaManager
* Select Plugin.MediaManager
* Install into each project within your solution

Call **MediaManager.Current** from any .Net library or Xamarin project to gain access to APIs.

```csharp
await CrossMediaManager.Current.Play("http://www.montemagno.com/sample.mp3");
```

See Sample for more details.

### **IMPORTANT**
**Android:**

* You must request `AccessWifiState`, `Internet`, `MediaContentControl` and `WakeLock` permissions

**iOS:**

* In order for the audio to contiunue to play in the background you have to add the Audio and Airplay Background mode to your Info.plist
* If you want to enable RemoteControl features, you will have to override `UIApplication.RemoteControlReceived(UIEvent)` and forward the event to the `MediaManagerImplementation.MediaRemoteControl.RemoteCoontrolReceived(UIEvent)` method. See the sample application for more details.
* If you are playing audio from a http resource you have to take care of [ATS](https://developer.xamarin.com/guides/ios/platform_features/introduction_to_ios9/ats/).
* If you want to display a artwork/cover that is embedded into an MP3 file, make sure that you use ID3 v2.3 (not v2.4).

#### Contributors
* [martijn00](https://github.com/martijn00)
* [modplug](https://github.com/modplug)
* [jmartine2](https://github.com/jmartine2)
* [SimonSimCity](https://github.com/SimonSimCity)
