using System;
using System.Collections.Generic;
using StyleFileCreator.App.Model;
using StyleFileCreator.App.Resources;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace StyleFileCreator.App.ViewModel
{
    public class WelcomeViewModel : ViewVM
    {
        #region Fields

        private readonly IDataService _dataService;
        private string _welcomeTitle = string.Empty;
        private RelayCommand _showAboutCommand;

        #endregion // Fields       

        #region commands
       
        public ICommand ShowAboutCommand
        {
            get
            {
                if (_showAboutCommand == null)
                    _showAboutCommand = new RelayCommand(OpenAbout);
                return _showAboutCommand;
            }
        }

        private void OpenAbout()
        {
            //Messenger.Default.Send<GalaSoft.MvvmLight.ViewModelBase>(ServiceLocator.Current.GetInstance<AboutViewModel>(), "showPopup");
            //AboutDialog aboutWindow = new AboutDialog();
            //aboutWindow.ShowDialog();
        }

        public RelayCommand SaveDataCommand { get; set; }      

        #endregion

        #region constructor

        public WelcomeViewModel(StyleFileCreator.App.Model.IDataService dataService) : base(null)
        {
            _dataService = dataService;

            _dataService.GetData(
              delegate(DataItem item, Exception error)
              {
                  if (error != null)
                  {
                      // Report error here
                      return;
                  }

                  WelcomeTitle = item.Name;
              });

            SaveDataCommand = new RelayCommand(new Action(SaveData));

        }

        //test for data input:
        private void SaveData()
        {
          var columnDictionary = new Dictionary<string, string>
			{
				{
					"idColumn",
					"Objectid"
				},
				{
					"colorColumn",
					"HEXWert"
				},
				{
					"nameColumn",
					"INSPIRE_URI"
				},
				{
					"tagColumn",
					"RGBWert"
				}
			};
                string dbConnectString =
                   "Provider=" + "Microsoft.Jet.OLEDB.4.0" + ";" +
                    //"Data Source=" + base.FormData.ConnString + ";";
                    "Data Source=" + "C:\\Users\\kaiarn\\Desktop\\WernerStyleFile\\inspire_age_2012_teststyle.mdb" + ";";
                DataService dataservice = new DataService();
                dataservice.SaveFalseValue(dbConnectString, "farbtabelle", columnDictionary);
                
           
        }

        #endregion // Constructor

        #region Properties
        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        //public const string WelcomeTitlePropertyName = "WelcomeTitle";       
        public string WelcomeTitle
        {
            get { return _welcomeTitle; }
            set 
            { 
                //base.Set(ref _welcomeTitle, value);
                base.Set<string>(ref this._welcomeTitle, value, false, "WelcomeTitle");
            }
            
        }

        public override string DisplayName
        {
            get 
            {
                return Strings.PageDisplayName_Welcome;  //"Welcome";
            }
        }

        #endregion // Properties

        internal override bool IsValid()
        {
            return true;
        }

        internal override void Reset()
        {
            //nothing to reset
        }
    }
}
