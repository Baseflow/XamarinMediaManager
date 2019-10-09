using System;
namespace MediaManager.Media
{
    public interface IImage
    {
        T ToNative<T>();
    }
}
