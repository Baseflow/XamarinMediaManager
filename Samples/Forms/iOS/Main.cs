using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Plugin.MediaManager;
using UIKit;

namespace MediaForms.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        public const string AppName = "MediaForms";

        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, AppName, "AppDelegate");
        }

    }
}
