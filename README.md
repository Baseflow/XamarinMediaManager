## Media Plugin for Xamarin and Windows

Cross platform plugin to play media from shared code.

### Setup
* Available on NuGet: https://www.nuget.org/packages/Xam.Plugin.MediaManager/
* Install into your PCL project and Client projects.

**Supports**
* Xamarin.iOS
* Xamarin.Android


### API Usage

Call **MediaManager.Current** from any project or PCL to gain access to APIs.

See Sample for usage.

### **IMPORTANT**
**Android:**

You must request `AccessWifiState`, `Internet`, `MediaContentControl` and `WakeLock` permissions


#### Contributors
* [martijn00](https://github.com/martijn00)
