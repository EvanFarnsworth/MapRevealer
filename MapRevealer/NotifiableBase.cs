using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MapRevealer
{
    public abstract class NotifiableBase : INotifyPropertyChanged, IDisposable
    {
        public NotifiableBase()
        {
            CreateCommands();
        }

        #region NotifyPropertyChanged

        /// <summary>
        ///     Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetAndNotify<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Try this out?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storage"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected static bool SetAndNotifyStatic<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        /// <summary>
        ///     Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers
        ///     that support <see cref="CallerMemberNameAttribute" />.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// A slightly different implementation of Static Property Changed, from here:
        /// https://stackoverflow.com/a/42111290/2444435
        /// </summary>
        public static event PropertyChangedEventHandler StaticPropertyChanged;

        protected static void NotifyStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RefreshAllProperties()
        {
            var props = this.GetType().GetProperties();
            foreach (var prop in props)
                OnPropertyChanged(prop.Name);
        }
        #endregion

        #region Commands
        protected virtual void CreateCommands() { }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
