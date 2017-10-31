using StyleFileCreator.App.Model;

namespace StyleFileCreator.App.ViewModel
{
    public abstract class ViewVM : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Fields   
  
        readonly FormularData _formData;
        private bool _isCurrentPage;

        #endregion // Fields

        #region Constructor

        protected ViewVM(FormularData formData)
        {
            this._formData = formData;           
        }

        #endregion // Constructor

        #region Properties     
  
        public FormularData FormData
        {
            get { return _formData; }
        } 

        public abstract string DisplayName { get; }

        public bool IsCurrentPage
        {
            get { return _isCurrentPage; }
            set
            {
                if (value == _isCurrentPage)
                {
                    return;
                }
                _isCurrentPage = value;
                this.RaisePropertyChanged("IsCurrentPage");
            }
        }

        #endregion

        #region abstract methods

        /// <summary>
        /// Returns true if the user has filled in this page properly
        /// and the wizard should allow the user to progress to the 
        /// next page in the workflow.
        /// </summary>
        internal abstract bool IsValid();

        internal abstract void Reset();

        #endregion // Methods



    }
}
