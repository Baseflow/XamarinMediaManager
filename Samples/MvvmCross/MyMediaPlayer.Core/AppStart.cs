using System;
using MvvmCross.Core.ViewModels;
using MyMediaPlayer.Core.ViewModels;

namespace MyMediaPlayer.Core
{
    public class AppStart : MvxNavigatingObject, IMvxAppStart
    {
        public void Start(object hint = null)
        {
            ShowViewModel<HomeViewModel>();
        }
    }
}
