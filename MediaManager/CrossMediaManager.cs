using Plugin.MediaManager.Abstractions;
using System;
using System.Diagnostics;

namespace Plugin.MediaManager
{
  /// <summary>
  /// Cross platform MediaManager implemenations
  /// </summary>
  public class CrossMediaManager
  {
    static readonly Lazy<IMediaManager> Implementation = new Lazy<IMediaManager>(CreateMediaManager);

    /// <summary>
    /// Current settings to use
    /// </summary>
    public static IMediaManager Current
    {
      get
      {
      
        if (Implementation.Value == null)
        {
          throw NotImplementedInReferenceAssembly();
        }
        return Implementation.Value;
      }
    }

    static IMediaManager CreateMediaManager()
    {
#if PORTABLE
        Debug.WriteLine("PORTABLE Reached");
        return null;
#else
            Debug.WriteLine("Other reached");
            return new MediaManagerImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly()
    {
      return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
    }
  }
}
