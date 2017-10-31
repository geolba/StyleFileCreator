using System;
using GalaSoft.MvvmLight.Command;
using StyleFileCreator.App.Model;
using System.Collections.ObjectModel;
using StyleFileCreator.App.Utils;
using GalaSoft.MvvmLight.Messaging;
using CommonServiceLocator;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using ESRI.ArcGIS.Framework; 

namespace StyleFileCreator.App.ViewModel
{
    public class FeedbackEventArgs : EventArgs
    {
        public FeedbackEventArgs(bool close, bool resetForm)
        {
            this.ShouldClose = close;
            this.ResetForm = resetForm;
        }
        public bool ShouldClose { get; set; }
        public bool ResetForm { get; set; }
        //public string Reason { get; set; }
    }
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        #region Fields
        private bool _isPopupVisible;
        private RelayCommand _closePopupCommand;
        

        RelayCommand _movePreviousCommand;
        RelayCommand _moveNextCommand;
        RelayCommand _cancelCommand;
        RelayCommand _closeCommand;
        private ReadOnlyCollection<ViewVM> _views;
        private FormularData _formData;
        private ViewVM _currentView;
        private GalaSoft.MvvmLight.ViewModelBase _currentPopupData;

        private readonly IDataService _dataService;
        private readonly DialogService _dialogService;
        private bool _isPrevButtonEnabled = false;
        private bool _isNextButtonEnabled = true;

        public event EventHandler<FeedbackEventArgs> RequestClose;

        #endregion // Fields

        /// <summary>
        /// Returns the cup of coffee ordered by the customer.
        /// If this returns null, the user cancelled the order.
        /// </summary>
        public FormularData FormData
        {
            get { return this._formData; }
        }
               
        public ViewVM CurrentView
        {
            get { return _currentView; }
            set 
            {                
                if (value == _currentView)
                    return;

                if (_currentView != null)
                {
                    _currentView.IsCurrentPage = false;
                }               
                Set(ref _currentView, value);
                if (_currentView != null)
                {
                    _currentView.IsCurrentPage = true;
                }
                this.RaisePropertyChanged("IsOnLastPage");
            }
        }

        public bool IsOnLastPage
        {
            get { return this.CurrentViewIndex == this.Views.Count - 1; }
        }

        public GalaSoft.MvvmLight.ViewModelBase CurrentPopupData
        {
            get { return _currentPopupData; }
            set
            {
                Set(ref _currentPopupData, value);              
                this.RaisePropertyChanged("CurrentPopupData");
            }
        }

        public bool IsPopupVisible
        {
            get { return this._isPopupVisible; }
            set
            {
                if (this._isPopupVisible == value)
                    return;
                this._isPopupVisible = value;
                RaisePropertyChanged("IsPopupVisible");
            }
        }

        /// <summary>
        /// Returns a read-only collection of all page ViewModels.
        /// </summary>
        public ReadOnlyCollection<ViewVM> Views
        {
            get
            {
                if (_views == null)
                {
                    this.CreateSubViews();
                }
                return _views;
            }
        }

        
                
        public bool IsPrevButtonEnabled
        {
            get { return _isPrevButtonEnabled; }
            set {  Set(ref _isPrevButtonEnabled, value); }
        }
               
        public bool IsNextButtonEnabled
        {
            get { return _isNextButtonEnabled; }
            set
            {
                //_isPrevButtonEnabled = value;
                //RaisePropertyChanged("IsPrevButtonEnabled");
                Set(ref _isNextButtonEnabled, value);
            }
        }

        public IApplication Application { get; set; }


        #region commands        
        public RelayCommand TestDialogServiceCommand { get; set; }

        #region CloseCommand

        /// <summary>
        /// Returns the command which, when executed, cancels the order 
        /// and causes the Wizard to be removed from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(() => this.Close());

                return _closeCommand;
            }
        }

        void Close()
        {
            //this._formData = null;
            //close the form true , but rnot eset the form data 
            this.RaiseRequestClose(new FeedbackEventArgs(true, false));
        }
       

        #endregion // CloseCommand

        #region CancelCommand resetting

        /// <summary>
        /// Returns the command which, when executed, cancels the order 
        /// and causes the Wizard to be removed from the user interface.
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(() => this.CancelOrder());
                }
                return _cancelCommand;
            }
        }

        void CancelOrder()
        {           
            //not close the form, but reset the form data = true
            this.RaiseRequestClose(new FeedbackEventArgs(false, true));
        }


        #endregion // CancelCommand

        #region MovePreviousCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the previous page in the workflow.
        /// </summary>
        public ICommand MovePreviousCommand
        {
            get
            {
                if (_movePreviousCommand == null)
                    _movePreviousCommand = new RelayCommand(
                        () => this.MoveToPreviousPage());

                return _movePreviousCommand;
            }
        }

        private bool CanMoveToPreviousPage
        {
            get 
            {               
                 bool canPreviousEnabled = 0 < this.CurrentViewIndex;
                 //this.IsPrevButtonEnabled = canPreviousEnabled;
                 return canPreviousEnabled;
            }
        }

        private void MoveToPreviousPage()
        {
            if (this.CanMoveToPreviousPage)
            {
                if (CurrentView is DataViewModel)
                {
                    this.CurrentView.Reset();
                }
                //set the new current view
                this.CurrentView = this.Views[this.CurrentViewIndex - 1];

                bool canPreviousEnabled = 0 < this.CurrentViewIndex;
                this.IsPrevButtonEnabled = canPreviousEnabled;

                bool canNextEnabled = this.CurrentView != null && this.CurrentView.IsValid();
                this.IsNextButtonEnabled = canNextEnabled;
            }
        }

        #endregion // MovePreviousCommand

        #region MoveNextCommand

        /// <summary>
        /// Returns the command which, when executed, causes the CurrentPage 
        /// property to reference the next page in the workflow.  If the user
        /// is viewing the last page in the workflow, this causes the Wizard
        /// to finish and be removed from the user interface.
        /// </summary>
        public ICommand MoveNextCommand
        {
            get
            {
                if (_moveNextCommand == null)
                    _moveNextCommand = new RelayCommand(
                        () => this.MoveToNextPage());

                return _moveNextCommand;
            }
        }

        private bool CanMoveToNextPage
        {
            get 
            {
                bool canNextEnabled = this.CurrentView != null && this.CurrentView.IsValid();
                return canNextEnabled;
            }
        }

        private void MoveToNextPage()
        {
            if (this.CanMoveToNextPage)
            {
                if (this.CurrentViewIndex < this.Views.Count - 1)
                {
                    this.CurrentView = this.Views[this.CurrentViewIndex + 1];

                    bool canPreviousEnabled = 0 < this.CurrentViewIndex;
                    this.IsPrevButtonEnabled = canPreviousEnabled;

                    bool canNextEnabled = this.CurrentView != null && this.CurrentView.IsValid();
                    this.IsNextButtonEnabled = canNextEnabled;
                }
                else //on last view, close the window
                {
                    //FeedbackEventArgs e = new FeedbackEventArgs(true);
                    //close the form = true, and reset the form data = true
                    this.RaiseRequestClose(new FeedbackEventArgs(true, true));
                }
                
            }
        }

        #endregion // MoveNextCommand

        #region ClosePopup

        public ICommand ClosePopupCommand
        {
            get
            {
                if (this._closePopupCommand == null)
                    this._closePopupCommand = new RelayCommand(ClosePopup);
                return this._closePopupCommand;
            }
        }

        private void ClosePopup()
        {
            this.IsPopupVisible = false;
            this.CurrentPopupData = null;
        }


        #endregion ClosePopupCommand

        private void ShowPopup(GalaSoft.MvvmLight.ViewModelBase content)
        {
            //ClosePopup();
            this.CurrentPopupData = content;
            this.IsPopupVisible = true;
        }


        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService, DialogService dialogService, FormularData formData)
        {
            //this.Application = WernerStyleFileCreator.ArcMap.Application

            _dataService = dataService;
            _dialogService = dialogService;
            _formData = formData;// new FormularData();
            _formData.PropertyChanged += OnPropertyChanged;
          

            //CurrentPage = ServiceLocator.Current.GetInstance<ReportViewModel>();
            CurrentView = this.Views[0];

            Messenger.Default.Register<GalaSoft.MvvmLight.ViewModelBase>(this, "showPopup", (action) => ShowPopup(action));
            Messenger.Default.Register<GalaSoft.MvvmLight.ViewModelBase>(this, "closePopup", (action) => ClosePopup());
        }

        void OnPropertyChanged(object sender,  PropertyChangedEventArgs args)
        {
            if (this.CurrentView != null)
            {
                bool canNextEnabled = this.CurrentView.IsValid();
                this.IsNextButtonEnabled = canNextEnabled;
            }
        }

        #region Events

        /// <summary>
        /// Raised when the wizard should be removed from the UI.
        /// </summary>
        void RaiseRequestClose(FeedbackEventArgs e)
        {
            EventHandler<FeedbackEventArgs> handler = this.RequestClose;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion // Events            


        #region Private Helpers

        private void CreateSubViews()
        {
           
            var views = new List<ViewVM>();
            var welcomeViewModel = ServiceLocator.Current.GetInstance<WelcomeViewModel>();
            views.Add(welcomeViewModel);           
            var settingsViewModel = ServiceLocator.Current.GetInstance<SettingsViewModel>();
            views.Add(settingsViewModel);
            var geometryTypeViewModel = ServiceLocator.Current.GetInstance<GeometryTypeViewModel>();
            views.Add(geometryTypeViewModel);
            var exportViewModel = ServiceLocator.Current.GetInstance<ExportViewModel>();
            views.Add(exportViewModel);
            var dataViewModel = ServiceLocator.Current.GetInstance<DataViewModel>();
            views.Add(dataViewModel);          

            _views = new ReadOnlyCollection<ViewVM>(views);
        }
        
        private int CurrentViewIndex
        {
            get
            {
                if (this.CurrentView == null)
                {
                  
                    return -1;
                }
                return this.Views.IndexOf(this.CurrentView);
            }
        }

        #endregion

        public void Reset()
        {
            this._formData.PropertyChanged -= OnPropertyChanged;
            this._formData.Clear();
            this._formData.PropertyChanged += OnPropertyChanged;

            // Clean up if needed (e.g. unsaved data):
            CurrentView = this.Views[0];
            bool canPreviousEnabled = 0 < this.CurrentViewIndex;
            this.IsPrevButtonEnabled = canPreviousEnabled;
            bool canNextEnabled = this.CurrentView != null && this.CurrentView.IsValid();
            this.IsNextButtonEnabled = canNextEnabled;
        }


        public override void Cleanup()
        {
            //unregisters this instance from the messenger class
            base.Cleanup();
        }
    }
}
