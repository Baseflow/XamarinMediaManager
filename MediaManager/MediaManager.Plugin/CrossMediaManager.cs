using MediaManager.Plugin.Abstractions;
using System;

namespace MediaManager.Plugin
{
  /// <summary>
  /// Cross platform MediaManager implemenations
  /// </summary>
  public class CrossMediaManager
  {
    static Lazy<IMediaManager> Implementation = new Lazy<IMediaManager>(() => CreateMediaManager(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IMediaManager Current
    {
      get
      {
        var ret = Implementation.Value;
        if (ret == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return ret;
      }
    }

    static IMediaManager CreateMediaManager()
    {
#if PORTABLE
        return null;
#else
        return new MediaManagerImplementation();
#endif
    }

    internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
