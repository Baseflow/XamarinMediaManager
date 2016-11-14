using System;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;

namespace MyMediaPlayer.Core.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        public virtual string Title { get; set; } = "";

        private bool _isLoading = false;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                SetProperty(ref _isLoading, value);
            }
        }

        private bool _isRefreshing;
        public virtual bool IsRefreshing
        {
            get { return _isRefreshing; }
            set
            {
                SetProperty(ref _isRefreshing, value);
            }
        }

        public MvxAsyncCommand ReloadCommand
        {
            get
            {
                return new MvxAsyncCommand(async () => await ReloadData());
            }
        }

        public virtual Task ReloadData()
        {
            return Task.FromResult(0);
        }
    }
}
