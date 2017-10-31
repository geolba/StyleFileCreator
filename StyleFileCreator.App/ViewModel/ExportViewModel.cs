using System;
using GalaSoft.MvvmLight.Command;
using StyleFileCreator.App.Model;
using StyleFileCreator.App.Resources;
using WPFFolderBrowser;

namespace StyleFileCreator.App.ViewModel
{

    public class ExportViewModel : ViewVM
    {
        #region fields

        //private string _selectedFolder;
        //private string _fileName;
        #endregion

        #region properties

        public override string DisplayName
        {
            get
            {
                return Strings.PageDisplayName_Export;  //"Export Settings";
            }
        }

        //public string FileName
        //{
        //    get { return _fileName; }
        //    set
        //    {
        //        base.Set<string>(ref this._fileName, value, false, "FileName");
        //        base.FormData.ExportFilename = _fileName;
        //    }
        //} 

        //public string SelectedFolder
        //{
        //    get { return _selectedFolder; }
        //    private set
        //    {
        //        base.Set<string>(ref this._selectedFolder, value, false, "SelectedFolder");               
        //        base.FormData.ExportFolder = _selectedFolder;
        //    }
        //}

        public RelayCommand OpenFileCommand { get; set; }

        #endregion

        public ExportViewModel(StyleFileCreator.App.Model.IDataService dataService, FormularData formData)
            : base(formData)
        {
            this.OpenFileCommand = new RelayCommand(new Action(OpenFolder));            
        }

        private void OpenFolder()
        {
            WPFFolderBrowserDialog dlgDirestorySelector = new WPFFolderBrowserDialog();
            dlgDirestorySelector.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if (dlgDirestorySelector.ShowDialog() == true)
            {                
                base.FormData.ExportFolder = dlgDirestorySelector.FileName;
            }
        }
        internal override void Reset()
        {
            base.FormData.ExportFolder = string.Empty;
            base.FormData.ExportFilename = "myStyleFile";
        }

        public override void Cleanup()
        {  
            //unregisters this instance from the messenger class
            base.Cleanup();
        }

        internal override bool IsValid()
        {
            return !string.IsNullOrEmpty(base.FormData.ExportFolder) && !string.IsNullOrEmpty(base.FormData.ExportFilename);
        }
    }

}
