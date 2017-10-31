using System;
using System.ComponentModel;

namespace StyleFileCreator.App.ViewModel
{
    /// <summary>
    /// Represents a value with a user-friendly name that can be selected by the user.
    /// </summary>
    /// <typeparam name="TValue">The type of value represented by the option.</typeparam>
    public class OptionViewModel<TValue> : INotifyPropertyChanged, IComparable<OptionViewModel<TValue>>
    {
        #region Fields

        const int UNSET_SORT_VALUE = Int32.MinValue;

        readonly string _displayName;
        bool _isSelected;
        readonly int _sortValue;
        readonly TValue _value;

        #endregion // Fields

        #region Constructor

        //public OptionViewModel(string displayName, TValue value)
        //    : this(displayName, value, UNSET_SORT_VALUE)
        //{
        //}

        public OptionViewModel(string displayName, TValue value, int sortValue)
        {
            _displayName = displayName;
            _value = value;
            _sortValue = sortValue;
        }

        #endregion // Constructor

        #region Properties

        /// <summary>
        /// Returns the user-friendly name of this option.
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
        }

        /// <summary>
        /// Gets/sets whether this option is in the selected state.
        /// When this property is set to a new value, this object's
        /// PropertyChanged event is raised.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;

                _isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Returns the value used to sort this option.
        /// The default sort value is Int32.MinValue.
        /// </summary>
        public int SortValue
        {
            get { return _sortValue; }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Returns the underlying value of this option.
        /// Note: this is a method, instead of a property,
        /// so that the UI cannot bind to it.
        /// </summary>
        internal TValue GetValue()
        {
            return _value;
        }

        #endregion // Methods

        #region IComparable<OptionViewModel<TValue>> Members

        public int CompareTo(OptionViewModel<TValue> other)
        {
            if (other == null)
                return -1;

            if (this.SortValue == UNSET_SORT_VALUE && other.SortValue == UNSET_SORT_VALUE)
            {
                return this.DisplayName.CompareTo(other.DisplayName);
            }
            else if (this.SortValue != UNSET_SORT_VALUE && other.SortValue != UNSET_SORT_VALUE)
            {
                return this.SortValue.CompareTo(other.SortValue);
            }
            else if (this.SortValue != UNSET_SORT_VALUE && other.SortValue == UNSET_SORT_VALUE)
            {
                return -1;
            }
            else
            {
                return +1;
            }
        }

        #endregion // IComparable<OptionViewModel<TValue>> Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
