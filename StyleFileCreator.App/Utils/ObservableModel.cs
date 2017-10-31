using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Collections;

namespace StyleFileCreator.App.Utils
{
   
    //public interface INotifyDataErrorInfo
    //    {
    //        bool HasErrors { get; }
    //        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    //        IEnumerable GetErrors(string propertyName);
    //    }

    public class ObservableModel : INotifyPropertyChanged, IDataErrorInfo, INotifyDataErrorInfo
    {
        #region Fields

        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        
        //public readonly Dictionary<string, string[]> _errors = new Dictionary<string, string[]>();
        private ConcurrentDictionary<string, List<string>> _validationErrors = new ConcurrentDictionary<string, List<string>>();
       // private readonly Dictionary<string, List<string>>
       //_validationErrors = new Dictionary<string, List<string>>();


        #endregion

        protected T GetValue<T>(System.Linq.Expressions.Expression<Func<T>> propertySelector)
        {
            string propertyName = GetPropertyName(propertySelector);
            return GetValue<T>(propertyName);
        }

        protected T GetValue<T>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }
            object value;
            if (!_values.TryGetValue(propertyName, out value))
            {
                value = default(T);
                _values.Add(propertyName, value);
            }
            return (T)value;
        }

        protected void SetValue<T>(System.Linq.Expressions.Expression<Func<T>> propertySelector, T value)
        {
            string propertyName = GetPropertyName(propertySelector);
            SetValue<T>(propertyName, value);
        }

        protected void SetValue<T>(string propertyName, T value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }
            _values[propertyName] = value;
            this.RaisePropertyChanged(propertyName);
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value))
            {
                return false;
            }         
            storage = value;
            this.RaisePropertyChanged(propertyName);          
            return true;
        }

        public virtual bool IsValid()
        {
            return GetValidationErrors() == string.Empty;
        }

       

        #region Privates

        private string GetPropertyName(System.Linq.Expressions.LambdaExpression expression)
        {
            var memberExpression = expression.Body as System.Linq.Expressions.MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException();
            }

            return memberExpression.Member.Name;
        }

        protected virtual string GetValidationErrors()
        {
            var vc = new ValidationContext(this, null, null);
            var vResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(this, vc, vResults, true))
            {
                return vResults.Aggregate("", (current, ve) => current + (ve.ErrorMessage + Environment.NewLine));
            }

            return "";
        }

        protected virtual string GetValidationErrors(string columnName)
        {
            var vc = new ValidationContext(this, null, null);
            var vResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(this, vc, vResults, true))
            {
                string error = "";
                foreach (var ve in vResults)
                {
                    if (ve.MemberNames.Contains(columnName, StringComparer.CurrentCultureIgnoreCase))
                    {
                        error += ve.ErrorMessage + Environment.NewLine;
                    }

                }
                return error;
            }
            return "";
        }
        
        #endregion

        #region interface INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        //public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                eventHandler(this, e);
            }
            ValidateAsync(propertyName);
        }      
       

        #endregion


        #region Notify data error

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void RaiseErrorsChanged(string propertyName)
        {
            EventHandler<DataErrorsChangedEventArgs> eventHandler = this.ErrorsChanged;
            if (eventHandler != null)
            {
                var e = new DataErrorsChangedEventArgs(propertyName);
                this.ErrorsChanged(this, e);
            }
        }
        //
        // Summary:
        //     Gets the validation errors for a specified property or for the entire entity.    
        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errorsForName;
            _validationErrors.TryGetValue(propertyName, out errorsForName);
            return errorsForName;
        }

        public bool HasErrors
        {
            get
            {
               return _validationErrors.Any(kv => kv.Value != null && kv.Value.Count > 0); 
            }
        }

        public Task ValidateAsync(string propertyName)
        {
            return Task.Run(() => Validate(propertyName));
        }

        private object _lock = new object();
        public void Validate(string propertyName)
        {
            lock (_lock)
            {



               

                ValidationContext validationContext = new ValidationContext(this, null, null);
                List<ValidationResult> validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                //string error = "";
                //foreach (var ve in validationResults)
                //{
                //    if (ve.MemberNames.Contains(propertyName, StringComparer.CurrentCultureIgnoreCase))
                //    {
                //        error += ve.ErrorMessage + Environment.NewLine;
                //    }

                //}



                //alle
                foreach (var kv in _validationErrors.ToList())
                {
                    if (validationResults.All(r => r.MemberNames.All(m => m != kv.Key)))
                    {
                        List<string> outLi;
                        _validationErrors.TryRemove(kv.Key, out outLi);
                        RaiseErrorsChanged(kv.Key);
                    }
                }

                var q = from r in validationResults
                        from m in r.MemberNames
                        where r.MemberNames.Contains(propertyName, StringComparer.CurrentCultureIgnoreCase)
                        group r by m into g
                        select g;

                foreach (var prop in q)
                {
                    var messages = prop.Select(r => r.ErrorMessage).ToList();

                    if (_validationErrors.ContainsKey(prop.Key))
                    {
                        List<string> outLi;
                        _validationErrors.TryRemove(prop.Key, out outLi);
                    }
                    _validationErrors.TryAdd(prop.Key, messages);
                    RaiseErrorsChanged(prop.Key);
                }
            }
        }

        #endregion


        #region interface IDataErrorInfo

        public string Error
        {
            get 
            {
                 return GetValidationErrors();            
            }
        }

        //public bool HasErrors
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //    ///// <summary>
        //    ///// Error calculation from IDataErrorInfo
        //    ///// Gets the first error for the given property
        //    ///// </summary>
        public string this[string columnName]
        {
            get 
            { 
                //throw new NotImplementedException();              
                return GetValidationErrors(columnName);
               
            }
        }

        #endregion





    }
}
