using Android.OS;
using Android.Views;
using AndroidX.AppCompat.Widget;
using ElementPlayer.Core.ViewModels;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Platforms.Android.Views.Fragments;
using MvvmCross.ViewModels;

namespace ElementPlayer.Android.Fragments
{
    public abstract class BaseFragment : MvxFragment
    {
        private Toolbar _toolbar;
        protected virtual string Title => (ViewModel as BaseViewModel)?.Title ?? "";

        protected abstract int FragmentId { get; }

        public MvxActivity ParentActivity
        {
            get
            {
                return (MvxActivity)Activity;
            }
        }

        protected BaseFragment()
        {
            RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(FragmentId, null);

            _toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
            if (_toolbar != null)
            {
                ParentActivity.SetSupportActionBar(_toolbar);
                ParentActivity.SupportActionBar.SetDisplayHomeAsUpEnabled(false);
                ParentActivity.SupportActionBar.SetHomeButtonEnabled(false);
                if (!string.IsNullOrEmpty(Title))
                    ParentActivity.SupportActionBar.Title = Title;
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
