
[assembly: Xamarin.Forms.Internals.Preserve(AllMembers = true)]

#if NETSTANDARD
[assembly: Xamarin.Forms.XmlnsPrefix("http://baseflow.com/mediamanager", "mm")]
[assembly: Xamarin.Forms.XmlnsDefinition("http://baseflow.com/mediamanager", nameof(MediaManager.Forms))]
[assembly: Xamarin.Forms.XmlnsDefinition("http://baseflow.com/mediamanager", nameof(MediaManager.Forms.Xaml))]
#endif
