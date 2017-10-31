using System;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using StyleFileCreator.App.Model;
using System.Collections.ObjectModel;
using StyleFileCreator.App.Utils;
//using Microsoft.Practices.ServiceLocation;
using StyleFileCreator.App.Resources;
using System.Collections.Generic;

namespace StyleFileCreator.App.ViewModel
{
    public class SettingsViewModel : ViewVM
    {
        private readonly IDataService _dataService;
        private string _errorString;

        public override string DisplayName
        {
            get 
            { 
                return Strings.PageDisplayName_Settings;// "Konfigurationen";
            }
        }

        ObservableCollection<string> _tables;
        public ObservableCollection<string> Tables
        {
            get { return _tables; }
            set { Set(ref _tables, value); }
        }   

        #region columns 

        ObservableCollection<string> _idColumns;
        public ObservableCollection<string> IdColumns
        {
            get { return this._idColumns; }
            set 
            { 
                //base.Set(ref _idColumns, value);
                base.Set<ObservableCollection<string>>(ref this._idColumns, value, false, "IdColumns");
            }
        }

        ObservableCollection<string> _hexColumns;
        public ObservableCollection<string> HexColumns
        {
            get { return this._hexColumns; }
            set 
            { 
                //base.Set(ref this._hexColumns, value);
                base.Set<ObservableCollection<string>>(ref this._hexColumns, value, false, "HexColumns");
            }
        }       

        ObservableCollection<string> _tagColumns;
        public ObservableCollection<string> TagColumns
        {
            get { return this._tagColumns; }
            set 
            { 
                //base.Set(ref this._tagColumns, value);
                base.Set<ObservableCollection<string>>(ref this._tagColumns, value, false, "TagColumns");
            }
        }

        ObservableCollection<string> _nameColumns;
        public ObservableCollection<string> NameColumns
        {
            get { return this._nameColumns; }
            set 
            { 
                //base.Set(ref this._nameColumns, value);
                base.Set<ObservableCollection<string>>(ref this._nameColumns, value, false, "NameColumns");
            }
        }

        #endregion

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

        #region commands
        //public RelayCommand OpenMasterDataCommand { get; set; }

        public RelayCommand ReadAllTables { get; set; }
        public RelayCommand ReadAllColumns { get; set; }
        public RelayCommand TestDialogServiceCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }

        #endregion

        public SettingsViewModel(StyleFileCreator.App.Model.IDataService dataService, FormularData formData)
            : base(formData)
        {
            _dataService = dataService;

            Tables = new ObservableCollection<string>();
            IdColumns = new ObservableCollection<string>();
            HexColumns = new ObservableCollection<string>();
            TagColumns = new ObservableCollection<string>();
            NameColumns = new ObservableCollection<string>();
           

            ReadAllTables = new RelayCommand(new Action(GetTables));
            OpenFileCommand = new RelayCommand(new Action(OpenFile));
            ReadAllColumns = new RelayCommand(new Action(GetColumns));           
        }

        private void OpenFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location };
            dialog.Title = "Select Access Database";
            dialog.Filter = "MDB (*.Mdb)|*.mdb";                          
            // Show open file dialog box
            System.Nullable<bool> result = dialog.ShowDialog();
            // OK is pressed:
            if (result == true)
            {   
                base.FormData.ConnString = dialog.FileName;
            }
        }

        private void GetTables()
        {
            IdColumns.Clear();
            HexColumns.Clear();
            TagColumns.Clear();
            NameColumns.Clear();

            if (System.IO.File.Exists(base.FormData.ConnString) == true)
            {
                string dbConnectString =
                   "Provider=" + "Microsoft.Jet.OLEDB.4.0" + ";" +
                    "Data Source=" + base.FormData.ConnString + ";";               
                Tables.Clear();
                foreach (var item in _dataService.GetTables(dbConnectString))
                {
                    Tables.Add(item);
                }
                base.FormData.Table = Tables.FirstOrDefault<string>();
            }
        }

        private void GetColumns()
        {
            string dbConnectString =
               "Provider=" + "Microsoft.Jet.OLEDB.4.0" + ";" +
                "Data Source=" + base.FormData.ConnString + ";";
            IdColumns.Clear();
            HexColumns.Clear();
            TagColumns.Clear();
            NameColumns.Clear();
            IEnumerable<KeyValuePair<Type, string>> columns = this._dataService.GetColumnNames(dbConnectString, this.FormData.Table);
            Farbtabelle colorTable = new Farbtabelle();
            
            Type idType = colorTable.GetByParameterName("Id");
            List<string> idColumns = columns.Where(p => p.Key == idType).Select(kvp => kvp.Value).ToList();
            foreach (var columnName in idColumns)
            {
                IdColumns.Add(columnName);
            }
            base.FormData.IdColumn = IdColumns.FirstIfNotFound(p => p == "Objectid" || p == "ObjectId" || p == "Id");

            Type hexType = colorTable.GetByParameterName("Hexwert");
            List<string> hexColumns = columns.Where(p => p.Key == hexType).Select(kvp => kvp.Value).ToList();
            foreach (var columnName in hexColumns)
            {
                HexColumns.Add(columnName);
            }
            base.FormData.HexColumn = HexColumns.FirstIfNotFound(p => p == "HEXWert" || p == "HexWert");

            Type tagType = colorTable.GetByParameterName("Tag");
            List<string> tagColumns = columns.Where(p => p.Key == tagType).Select(kvp => kvp.Value).ToList();
            foreach (var columnName in tagColumns)
            {
                TagColumns.Add(columnName);
            }
            base.FormData.TagColumn = TagColumns.FirstIfNotFound(p => p == "codeLabel_EN");

            Type nameType = colorTable.GetByParameterName("Name");
            List<string> nameColumns = columns.Where(p => p.Key == nameType).Select(kvp => kvp.Value).ToList();
            foreach (var columnName in nameColumns)
            {
                NameColumns.Add(columnName);
            }
            base.FormData.NameColumn = NameColumns.FirstIfNotFound(p => p == "INSPIRE_URI");

            //Tables.Insert(0, " -- Select Category -- ");
        }
          
        internal override bool IsValid()
        {
            if (base.FormData.HasValidColumns() == false && base.FormData.Table != null && base.FormData.ConnString != null 
                && NameColumns.Count > 0)
            {
                this.ErrorString = "ungültige Spaltenauswahl";
            }
            else
            {
                this.ErrorString = string.Empty;
            }
            //return true;
            return base.FormData.Table != null && base.FormData.ConnString != null && base.FormData.HasValidColumns() == true;
        }

        internal override void Reset()
        {
            //this.SelectedPath = string.Empty;
            base.FormData.ConnString = string.Empty;
            this.Tables.Clear();
            //this.SelectedTable = string.Empty;geht automatisch mit clear zu null wenn View als CurrentView gestzt ist
            base.FormData.Table = string.Empty;

            this.IdColumns.Clear();
            this.HexColumns.Clear();
            this.TagColumns.Clear();
            this.NameColumns.Clear();
            //this.SelectedIdColumn = string.Empty;
            //this.SelectedHexColumn = string.Empty;
            //this.SelectedTagColumn = string.Empty;
            //this.SelectedNameColumn = string.Empty;
            base.FormData.IdColumn = string.Empty;
            base.FormData.HexColumn = string.Empty;
            base.FormData.TagColumn = string.Empty;
            this.FormData.NameColumn = string.Empty;
            
            base.FormData.TruncateName = false;
        }

        public override void Cleanup()
        {            
            //unregisters this instance from the messenger class
            base.Cleanup();
        }
    }
}
