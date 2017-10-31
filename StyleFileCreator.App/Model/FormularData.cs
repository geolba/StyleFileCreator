using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyleFileCreator.App.Utils;
using System.ComponentModel.DataAnnotations;

namespace StyleFileCreator.App.Model
{
    public class FormularData : ObservableModel
    {
        #region Fields       
       
        string _table;
        string _connectionString;
        string _category;
        string _exportFolder;
        string _exportFilename;
        bool _truncateName;
        Nullable<GeometryType> _geometryType;
        Dictionary<string, string> _columnDictionary;

        #endregion // Fields

        #region constructor 

        public FormularData()
        {
            this._columnDictionary = new Dictionary<string, string>();
            this._truncateName = false;
            this._exportFilename = "myStyleFile";
            this._category = "Inspire";
        }

        #endregion

        [Required(ErrorMessage = "Table is required!")]
        public string Table
        {
            get { return _table; }
            set
            {
                if (value == _table)
                {
                    return;
                }
                _table = value;               
                base.RaisePropertyChanged("Table");
            }
        }
        
        [Required(ErrorMessage = "Connection string is required!")]
        [DoesExistFileName(ErrorMessage = "Uh oh, no existing database file")]
        public string ConnString
        {
            get { return _connectionString; }
            set
            {
                if (value == _connectionString)
                {
                    return;
                }
                _connectionString = value;
                base.RaisePropertyChanged("ConnString");
            }
        }

        public string Category
        {
            get { return _category; }
            set
            {
                if (value == _category)
                {
                    return;
                }
                _category = value;
                base.RaisePropertyChanged("Category");
            }
        }

        public string ExportFolder
        {
            get 
            { 
                return _exportFolder; 
            }
            set
            {
                if (value == _exportFolder)
                {
                    return;
                }
                _exportFolder = value;
                base.RaisePropertyChanged("ExportFolder");
            }
        }

        public string ExportFilename
        {
            get 
            { 
                return _exportFilename; 
            }
            set
            {
                if (value == _exportFilename)
                {
                    return;
                }
                _exportFilename = value;
                base.RaisePropertyChanged("ExportFilename");
            }
        }

        public bool TruncateName
        {
            get 
            { 
                return _truncateName; 
            }
            set
            {
                if (value == _truncateName)
                {
                    return;
                }
                _truncateName = value;
                base.RaisePropertyChanged("TruncateName");
            }
        }

        public Nullable<GeometryType> GeometryType
        {
            get { return _geometryType; }
            set
            {
                if (value == _geometryType)
                    return;

                _geometryType = value;
                base.RaisePropertyChanged("GeometryType");
                //this.CalculatePrice();
            }
        }

        public Dictionary <string, string> ColumnDictionary 
        { 
            get { return _columnDictionary; }
            set
            {
                if (value == _columnDictionary)
                {
                    return;
                }
                _columnDictionary = value;
                //base.RaisePropertyChanged("ColumnDictionary");
            }
        }
        
        private string _id;
        public string IdColumn
        {
            get { return _id; }
            set
            {
                if (value == _id)
                {
                    return;
                }
                _id = value;
                this.CalculateColumns();
                this.RaisePropertyChanged("IdColumn");                             
            }
        }

        private string _name;
        public string NameColumn
        {
            get { return _name; }
            set
            {
                if (value == _name)
                {
                    return;
                }
                _name = value;
                this.CalculateColumns();
                this.RaisePropertyChanged("NameColumn");                              
            }
        }

        private string _hexwert;
        public string HexColumn
        {
            get { return _hexwert; }
            set
            {
                if (value == _hexwert)
                {
                    return;
                }
                _hexwert = value;
                this.CalculateColumns();
                this.RaisePropertyChanged("HexColumn");               
            }
        }

        //Tag is optional:
        private string _tag;
        public string TagColumn
        {
            get { return _tag; }
            set
            {
                if (value == _tag)
                {
                    return;
                }
                _tag = value;
                this.CalculateColumns();
                this.RaisePropertyChanged("TagColumn");               
            }
        }

        public bool HasValidColumns()
        {
            if (_columnDictionary.Count > 0)
            {
                var duplicateValues = this._columnDictionary.GroupBy(x => x.Value).Where(x => x.Count() > 1);
                if (duplicateValues.Count()== 0)
                {
                    return true;
                }
                //return true;
            }
            return false;
        }

        //Tag is optional:
        private bool _hasValidData;
        public bool HasValidData
        {
            get { return _hasValidData; }
            set
            {
                if (value == _hasValidData)
                {
                    return;
                }
                _hasValidData = value;
                base.RaisePropertyChanged("HasValidData");
            }
        }
            
        #region Private Helpers

        private void CalculateColumns()
        {
            this.ColumnDictionary = new Dictionary<string, string>
			{
				{
					"idColumn",
					this._id
				},
				{
					"colorColumn",
					this._hexwert
				},
				{
					"nameColumn",
					this._name
				},
				{
					"tagColumn",
					this._tag
				}
			};
        }
          
        #endregion // Private Helpers

        public void Clear()
        {
            this.ConnString = string.Empty;
            this.Table = string.Empty;
            //this.Description = string.Empty;
        }




    }
}
