using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MediaManager
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void SetProperty<T>(ref T storage, T value, Action<bool> action, [CallerMemberName] string propertyName = null)
        {
            if (action == null)
            {
                throw new ArgumentException($"{nameof(action)} should not be null", nameof(action));
            }

            action.Invoke(SetProperty(ref storage, value, propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, Action afterAction, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName))
            {
                afterAction?.Invoke();
                return true;
            }

            return false;
        }
    }
}
