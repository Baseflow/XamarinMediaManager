using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Android.App;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Internal;
using Google.Android.Material.TextField;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Commands;
using MvvmCross.Core;
using MvvmCross.IoC;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace ElementPlayer.Forms.Droid
{
    // This class is never actually executed, but when Xamarin linking is enabled it does how to ensure types and properties
    // are preserved in the deployed app
    public class LinkerPleaseInclude
    {
        public void Include(Button button)
        {
            button.Click += (s, e) => button.Text = $"{button.Text}";
        }

        public void Include(View view)
        {
            view.Click += (s, e) => view.ContentDescription = $"{view.ContentDescription}";
        }

        public void Include(TextView text)
        {
            text.AfterTextChanged += (sender, args) => text.Text = $"{text.Text}";
            text.Hint = $"{text.Hint}";
        }

        public void Include(CompoundButton cb)
        {
            cb.CheckedChange += (sender, args) => cb.Checked = !cb.Checked;
        }

        public void Include(SeekBar sb)
        {
            sb.ProgressChanged += (sender, args) => sb.Progress = sb.Progress + 1;
        }

        public void Include(RadioGroup radioGroup)
        {
            radioGroup.CheckedChange += (sender, args) => radioGroup.Check(args.CheckedId);
        }

        public void Include(RatingBar ratingBar)
        {
            ratingBar.RatingBarChange += (sender, args) => ratingBar.Rating = 0 + ratingBar.Rating;
        }

        public void Include(NumberPicker numberPicker)
        {
            numberPicker.ValueChanged += (sender, args) => numberPicker.Value = 0 + args.NewVal + numberPicker.MinValue + numberPicker.MaxValue;
            numberPicker.MinValue = numberPicker.MinValue + 1;
            numberPicker.MaxValue = numberPicker.MaxValue + 1;
            numberPicker.SetDisplayedValues(new string[] { "" });
        }

        public void Include(TextInputLayout textInputLayout)
        {
            textInputLayout.Hint = $"{textInputLayout.Hint}";
            textInputLayout.Error = $"{textInputLayout.Error}";
            textInputLayout.ErrorEnabled = true;
        }

        public void Include(TextInputEditText textInputEditText)
        {
            textInputEditText.Hint = $"{textInputEditText.Hint}";
            textInputEditText.Error = $"{textInputEditText.Error}";
            textInputEditText.SetAutofillHints("");
        }

        public void Include(Activity act)
        {
            act.Title = $"{act.Title}";
        }

        public void Include(ICommand command)
        {
            command.CanExecuteChanged += (s, e) => { if (command.CanExecute(null)) command.Execute(null); };
        }

        public void Include(INotifyCollectionChanged changed)
        {
            changed.CollectionChanged += (s, e) => { _ = $"{e.Action}{e.NewItems}{e.NewStartingIndex}{e.OldItems}{e.OldStartingIndex}"; };
        }

        public void Include(INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += (sender, e) => { _ = e.PropertyName; };
        }

        public void Include(MvxPropertyInjector injector)
        {
            _ = new MvxPropertyInjector();
        }

        public void Include(MvxTaskBasedBindingContext context)
        {
            context.Dispose();
            var context2 = new MvxTaskBasedBindingContext();
            context2.Dispose();
        }

        public void Include(MvxViewModelViewTypeFinder viewModelViewTypeFinder)
        {
            _ = new MvxViewModelViewTypeFinder(null, null);
        }

        public void Include(MvxNavigationService service, IMvxViewModelLoader loader)
        {
            _ = new MvxNavigationService(null, loader);
            _ = new MvxAppStart<MvxNullViewModel>(null, null);
        }

        public void Include(ConsoleColor color)
        {
            Console.Write("");
            Console.WriteLine("");
            _ = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public void Include(MvvmCross.Plugin.Json.Plugin plugin)
        {
            plugin.Load();
        }

        public void Include(AndroidX.AppCompat.Widget.AlertDialogLayout alertDialog)
        {
            _ = new AndroidX.AppCompat.Widget.AlertDialogLayout(Application.Context);
        }

        /*public void Include(AndroidX.ConstraintLayout.Widget.ConstraintLayout constraintLayout)
        {
            _ = new AndroidX.ConstraintLayout.Widget.ConstraintLayout(Application.Context);
        }*/

        public void Include(AndroidX.AppCompat.Widget.FitWindowsLinearLayout fitWindowsLinearLayout)
        {
            _ = new AndroidX.AppCompat.Widget.FitWindowsLinearLayout(Application.Context);
        }

        public void Include(BaselineLayout baselineLayout) => _ = new BaselineLayout(Application.Context);

        public void IncludeMvvmcross64()
        {
            _ = new MvxSettings();
            _ = new MvxStringToTypeParser(); //??
            //_ = new MvxPluginManager(null); //should not be required
            _ = new MvxViewModelLoader(null);
            _ = new MvxNavigationService(null, null);
            _ = new MvxViewModelByNameLookup();

            _ = new MvxViewModelViewTypeFinder(null, null);
            _ = new MvxViewModelViewLookupBuilder();
            _ = new MvxCommandCollectionBuilder();
            _ = new MvxStringDictionaryNavigationSerializer();
            _ = new MvxChildViewModelCache();
            _ = new MvxWeakCommandHelper();
        }
    }
}