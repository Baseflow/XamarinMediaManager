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
        static Lazy<IMediaManager> implementation = new Lazy<IMediaManager>(() => CreateMediaManager(), System.Threading.LazyThreadSafetyMode.PublicationOnly);
        /// <summary>
        /// Gets if the plugin is supported on the current platform.
        /// </summary>
        public static bool IsSupported => implementation.Value == null ? false : true;

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static IMediaManager Current
        {
            get
            {
                var ret = implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        static IMediaManager CreateMediaManager()
        {
#if NETSTANDARD_20
            return null;
#else
            return new MediaManagerImplementation();
#endif
        }

        internal static Exception NotImplementedInReferenceAssembly() =>
			new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        
    }
}
