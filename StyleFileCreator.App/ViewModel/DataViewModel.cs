using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using StyleFileCreator.App.Model;
using StyleFileCreator.App.Resources;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Display;
using System.Threading;
using System.IO;
using StyleFileCreator.App.Utils;
using System.Windows.Input;

namespace StyleFileCreator.App.ViewModel
{
    
    public class DataViewModel : ViewVM
    {
        #region Fields

        private readonly IDataService _dataService;
        private string _errorString;
        private ObservableCollection<Farbtabelle> _colorTables;
        private CancellationTokenSource _cancellationTokenSource;
        private ProgressReporter _progressReporter;
        private int _currentProgress;
        private bool _startButtonEnabled = true;  
        private IStyleGallery _pStyleGallery;
        private bool _hasValidData = false; //Visibility.Hidden;
        private bool _hasInvalidData = false;
        private Visibility _throbberVisible = Visibility.Hidden;
        #endregion

        public int CurrentProgress
        {
            get { return this._currentProgress; }
            private set
            {
                if (this._currentProgress != value)
                {
                    this._currentProgress = value;
                    this.RaisePropertyChanged("CurrentProgress");
                }
            }
        }

        public bool StartButtonEnabled
        {
            get { return this._startButtonEnabled; }
            private set
            {
                if (this._startButtonEnabled != value)
                {
                    this._startButtonEnabled = value;
                    this.RaisePropertyChanged("StartButtonEnabled");
                }
            }
        }

        public string ErrorString
        {
            get { return _errorString; }
            set
            {
                if (!(value == this._errorString))
                {
                    this._errorString = value;
                    this.RaisePropertyChanged("ErrorString");
                }
            }
        }
               
        public bool HasValidData
        {
            get { return _hasValidData; }
            set
            {
                if (value != this._hasValidData)
                {
                    this._hasValidData = value;
                    this.RaisePropertyChanged("HasValidData");
                }
            }
        }
        
        public bool HasInvalidData
        {
            get { return _hasInvalidData; }
            set
            {
                if (value != this._hasInvalidData)
                {
                    this._hasInvalidData = value;
                    this.RaisePropertyChanged("HasInvalidData");
                }
            }
        }
            
        public Visibility ThrobberVisible
        {
            get { return _throbberVisible; }
            set
            {
                if (value != this._throbberVisible)
                {
                    this._throbberVisible = value;
                    this.RaisePropertyChanged("ThrobberVisible");
                }
            }
        }

        public ObservableCollection<Farbtabelle> ColorTables
        {
            get
            {
                return _colorTables;
            }
        }

        #region commands

        public ICommand GetDataCommand { get; set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        #endregion


        public DataViewModel(StyleFileCreator.App.Model.IDataService dataService, FormularData formData) : base(formData)
        {
            
            //_formData = formData;
            _dataService = dataService;
            _colorTables = new ObservableCollection<Farbtabelle>();
            
            this.GetDataCommand = new RelayCommand(async () => { await InitAsync(); });           
            this.StartCommand = new RelayCommand(new Action (this.StartBackgroundTask));
            this.ClearCommand = new RelayCommand(new Action (this.ClearUI));
            this.CancelCommand = new RelayCommand(new Action(this.CancelBackgroundTask));                       
        }

        private async Task InitAsync()
        {
            ThrobberVisible = Visibility.Visible; //synchronously set the busy indicator flag
            this.HasValidData = false;
           
            string dbConnectString =
             "Provider=" + "Microsoft.Jet.OLEDB.4.0" + ";" +
             "Data Source=" + base.FormData.ConnString + ";";
            string table = base.FormData.Table;
            Dictionary<string, string> dictionary = base.FormData.ColumnDictionary;

            //Task<List<Farbtabelle>> task = _dataService.GetAllColortablesAsync(dbConnectString, table, dictionary);
            //var colorTableList = await task;
            //task.ContinueWith(task => 
            //{
            //    var colorTableList = task.Result;
            //    foreach (var item in colorTableList)
            //    {
            //        ColorTables.Add(item);
            //    }
            //    RaisePropertyChanged("ColorTables");

            //    bool notValid = colorTableList.Any(p => p.IsValid() == false);
            //    base.FormData.HasValidData = !notValid;
            //    this.HasValidData = !notValid; // Visibility.Visible;
            //    this.HasInvalidData = notValid;

            //    if (notValid)
            //    {
            //        this.ErrorString = colorTableList.Where(colort => colort.IsValid() == false).Select(p => p.Error).FirstOrDefault();
            //    }
            //    ThrobberVisible = Visibility.Collapsed;
            //});

            Task<List<Farbtabelle>> task = Task.Run<List<Farbtabelle>>(() => _dataService.GetAllColortables(dbConnectString, table, dictionary));
            var colorTableList = await task;

            ColorTables.Clear();
            foreach (var item in colorTableList)
            {
                ColorTables.Add(item);
            }
            RaisePropertyChanged("ColorTables");

            bool flag = colorTableList.Any(p => p.IsValid() == false);
            base.FormData.HasValidData = !flag;
            this.HasValidData = !flag; // Visibility.Visible;
            this.HasInvalidData = flag;

            if (flag)
            {
                this.ErrorString = colorTableList.Where(colort => colort.IsValid() == false).Select(p => p.Error).FirstOrDefault<string>();
            }
            ThrobberVisible = Visibility.Collapsed;
        }
             
        private void StartBackgroundTask()
        {          
            // Update UI to reflect background task.
            this.TaskIsRunning();

            //config section from FormularData:
            string dbConnectString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + base.FormData.ConnString + ";";
            string table = base.FormData.Table;
            Dictionary<string, string> dictionary = base.FormData.ColumnDictionary;
            string category = base.FormData.Category; //"Inspire";
            GeometryType geometrySelection = base.FormData.GeometryType.Value;
            bool truncateName = base.FormData.TruncateName;
            //bool truncateName = this.chkTruncate.IsChecked.Value;

            string styleDirectoryName = FormData.ExportFolder; //Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string styleFileName = FormData.ExportFilename + ".style"; //"myStyleFile35" + ".style";
            string strStylefile = System.IO.Path.Combine(styleDirectoryName, styleFileName);            
            var application = ViewModelLocator.Application;
            var mxDoc = (IMxDocument)application.Document;
            this._pStyleGallery = mxDoc.StyleGallery;
            IStyleGalleryStorage pStyleGalleryStorage = (IStyleGalleryStorage) this._pStyleGallery;
            pStyleGalleryStorage.TargetFile = strStylefile;
            if (File.Exists(strStylefile))
            {
                //if creating new fill symbol:
                if (geometrySelection == GeometryType.Fill)
                {
                    IEnumStyleGalleryItem pEnumFillStyGallItems = this._pStyleGallery.get_Items("Fill Symbols", strStylefile, "");
                    pEnumFillStyGallItems.Reset();
                    var pStyleGalleryItem = pEnumFillStyGallItems.Next();
                    while (pStyleGalleryItem != null)
                    {
                        this._pStyleGallery.RemoveItem(pStyleGalleryItem);
                        pStyleGalleryItem = pEnumFillStyGallItems.Next();
                    }
                }
                else if (geometrySelection == GeometryType.Line)
                {
                    IEnumStyleGalleryItem pEnumLineStyGallItems = this._pStyleGallery.get_Items("Line Symbols", strStylefile, "");
                    pEnumLineStyGallItems.Reset();
                    var pStyleGalleryLineItem = pEnumLineStyGallItems.Next();
                    while (pStyleGalleryLineItem != null)
                    {
                        this._pStyleGallery.RemoveItem(pStyleGalleryLineItem);
                        pStyleGalleryLineItem = pEnumLineStyGallItems.Next();
                    }
                }
                else if (geometrySelection == GeometryType.Marker)
                {
                    IEnumStyleGalleryItem pEnumLineStyGallItems = this._pStyleGallery.get_Items("Marker Symbols", strStylefile, "");
                    pEnumLineStyGallItems.Reset();
                    var pStyleGalleryLineItem = pEnumLineStyGallItems.Next();
                    while (pStyleGalleryLineItem != null)
                    {
                        this._pStyleGallery.RemoveItem(pStyleGalleryLineItem);
                        pStyleGalleryLineItem = pEnumLineStyGallItems.Next();
                    }
                }
            }

            this._progressReporter = new ProgressReporter();
            this._cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = this._cancellationTokenSource.Token;

            Task<List<IStyleGalleryItem>> mainThreadDoWorkTask = new Task<List<IStyleGalleryItem>>(
                    () => DoBackgroundComputation(cancellationToken, dbConnectString, table, dictionary, category, geometrySelection, truncateName), cancellationToken, TaskCreationOptions.DenyChildAttach);

            //siehe: https://stackoverflow.com/questions/21520869/proper-way-of-handling-exception-in-task-continuewith 
            //MainThreadDoWorkTask.ContinueWith(t => FinalThreadDoWOrk(), CancellationToken.None, TaskContinuationOptions.None, uiThread);
            Task continueTask = mainThreadDoWorkTask.ContinueWith(delegate(Task<List<IStyleGalleryItem>> task)
            {
                this.UpdateUI(task);
            }, TaskScheduler.FromCurrentSynchronizationContext());
            mainThreadDoWorkTask.Start();

        }

        private void ClearUI()
        {
            this.ErrorString = string.Empty;
        }
               
        private void CancelBackgroundTask()
        {
            // Cancel the background task.
            if (this._cancellationTokenSource != null)
            {
                this._cancellationTokenSource.Cancel();
                this._cancellationTokenSource.Dispose();
            }
        }
        
        # region private helper methods

        private List<IStyleGalleryItem> DoBackgroundComputation(CancellationToken ct, string dbConnectString, string table, Dictionary<string, string> dictionary, string category, GeometryType geometrySelection, bool truncateName)
        {
            //now in StaThread
            //string staVerify = Thread.CurrentThread.GetApartmentState().ToString();
            List<IStyleGalleryItem> styleList = new List<IStyleGalleryItem>();
            //var colorTableList = this.repo.GetAllColortables(dbConnectString, table, dictionary);
            //var notValid = colorTableList.Any(p => p.IsValid() == false);
            //if (notValid == true)
            //{
            //    var errors = colorTableList.Where(colort => colort.IsValid() == false).Select(p => p.Error);
            //    //throw new AggregateException()
            //    throw new AggregateException(errors.Select((e) => new System.ComponentModel.DataAnnotations.ValidationException(e)));
            //}

            int previousRowPercent = 0;
            int i = 0;
            foreach (Farbtabelle colorTable in ColorTables)
            {
                int currentRowPercent = Convert.ToInt32(Math.Round(i * 100.0 / ColorTables.Count));

                //Check for cancellation
                if (ct.IsCancellationRequested)
                {
                    //return "canceled";
                    ct.ThrowIfCancellationRequested();
                }
                if (currentRowPercent > previousRowPercent)
                {
                    //start task on ui thread:
                    _progressReporter.ReportProgress(() =>
                    {
                        this.CurrentProgress = currentRowPercent;
                        var messageInfo = "Message: " + currentRowPercent + "%";
                        ErrorString = messageInfo;
                    });
                }
                previousRowPercent = currentRowPercent;

                var styleGalleryItem = colorTable.GetStyleGalleryItem(category, geometrySelection, truncateName);
                styleList.Add(styleGalleryItem);
                //Thread.Sleep(20);
                i++;
            }
            return styleList;// "success";
        }

        private void UpdateUI(Task<List<IStyleGalleryItem>> task)
        {
            // Update UI to reflect completion.
            this.CurrentProgress = 100;

            //// Display results.
            //if (task.Exception != null)
            //{
            //    TextBlockMessages1.Text = "Background task error: " + task.Exception.ToString();
            //}
            if (task.IsFaulted)
            {
                // faulted with exception
                Exception ex = task.Exception;
                while (ex is AggregateException && ex.InnerException != null)
                    ex = ex.InnerException;
                this.ErrorString = "Background task error: " + ex.Message;
            }
            else if (task.IsCanceled)
            {
                this.ErrorString = "Background task cancelled";
            }
            else
            {
                _pStyleGallery.AddItemList(task.Result);
                this.ErrorString = "Das Stylefile wurde erfolgreich angelegt!";
            }
            // Reset UI.
            this.TaskIsComplete();
        }       

        private void TaskIsRunning()
        {
            // Update UI to reflect background task.
            this.ErrorString = "";
            this.StartButtonEnabled = false;
            //this.clearButton.IsEnabled = false;
            //this.cancelButton.IsEnabled = true;
        }
        
        private void TaskIsComplete()
        {
            // Reset UI.
            this.CurrentProgress = 0;
            this.StartButtonEnabled = true;         
            //this.clearButton.IsEnabled = true;
            //this.cancelButton.IsEnabled = false;
        }      

        #endregion

        public override string DisplayName
        {
            get 
            { 
                return Strings.PageDisplayName_Data;// "Data"; 
            }
        }

        internal override bool IsValid()
        {
            return base.FormData.HasValidData == true;
        }

        internal override void Reset()
        {
            this.ColorTables.Clear();
            this.ErrorString = string.Empty;
            base.FormData.HasValidData = false;
            this.HasInvalidData = false;
            this.HasValidData = false;
        }

        public override void Cleanup()
        {           
            base.Cleanup();
        }
    }
}