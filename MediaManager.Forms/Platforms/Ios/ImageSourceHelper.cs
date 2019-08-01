using System;
using System.Collections.Generic;
using System.Text;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;

namespace MediaManager.Forms.Platforms.Ios
{
    public static class ImageSourceHelper
    {
        public static ImageSource ToImageSource(this CGImage cgImage)
        {
            return ImageSource.FromStream(() => new UIImage(cgImage).AsPNG().AsStream());
        }
    }
}
