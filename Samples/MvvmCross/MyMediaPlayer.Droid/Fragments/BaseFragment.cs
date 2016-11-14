using System;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using MyMediaPlayer.Core.ViewModels;

namespace MyMediaPlayer.Droid.Fragments
{
    public abstract class BaseFragment : MvxFragment
    {
        protected abstract int FragmentId { get; }
        protected virtual string Title => (ViewModel as BaseViewModel)?.Title;

        public Toolbar _toolbar;

        public MvxCachingFragmentCompatActivity ParentActivity
        {
            get
            {
                return ((MvxCachingFragmentCompatActivity)Activity);
            }
        }

        protected BaseFragment()
        {
            this.RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(FragmentId, null);

            _toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
            if (_toolbar != null)
            {
                ParentActivity.SetSupportActionBar(_toolbar);
                if (!string.IsNullOrEmpty(Title))
                    ParentActivity.SupportActionBar.Title = Title;
                //ParentActivity.SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }

            return view;
        }
    }

    public abstract class BaseFragment<TViewModel> : BaseFragment where TViewModel : class, IMvxViewModel
    {
        public new TViewModel ViewModel
        {
            get { return (TViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }
    }
}
