## MediaManager - Cross platform media plugin for Xamarin and Windows
* Designed to be simple and easy to use
* Stand alone for easy integration with existing projects and frameworks
* Native plackback of media file
* Native notifications and remote controls

### Setup & Usage
* Available on NuGet: https://www.nuget.org/packages/Plugin.MediaManager/ [![NuGet](https://img.shields.io/nuget/v/Plugin.MediaManager.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.MediaManager/)
* Install into each project that utilizes MediaManager

### Build Status: [![Build status](https://ci.appveyor.com/api/projects/status/c9c6recwcu7k0s15?svg=true)](https://ci.appveyor.com/project/martijn00/xamarinmediamanager)

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

You must request `AccessWifiState`, `Internet`, `MediaContentControl` and `WakeLock` permissions


#### Contributors
* [martijn00](https://github.com/martijn00)
* [modplug](https://github.com/modplug)
* [jmartine2](https://github.com/jmartine2)
* [SimonSimCity](https://github.com/SimonSimCity)
