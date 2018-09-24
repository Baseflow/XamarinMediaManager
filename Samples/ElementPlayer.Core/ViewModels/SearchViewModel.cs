using System;
using System.Collections.Generic;
using System.Text;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace ElementPlayer.Core.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService) : base(logProvider, navigationService)
        {
        }

        public override string Title => "Search";
    }
}
