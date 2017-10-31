using System.Windows;
using StyleFileCreator.App.ViewModel;
using StyleFileCreator.App.Model;
//using Microsoft.Practices.ServiceLocation;
using CommonServiceLocator;

namespace StyleFileCreator.App
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class MainWizard : Window
    {
        readonly MainViewModel _mainViewModel;

        public MainWizard()
        {
            InitializeComponent();
            _mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
            _mainViewModel.RequestClose += this.OnViewModelRequestClose;


            ////Closing += (s, e) => ViewModelLocator.Cleanup();           
        }

        /// <summary>
        /// Returns the cup of coffee ordered by the user, 
        /// or null if the user cancelled the order.
        /// </summary>
        public FormularData Result
        {
            get { return _mainViewModel.FormData; }
        }

        void OnViewModelRequestClose(object sender, FeedbackEventArgs e)
        {
            //base.DialogResult = this.Result != null;
            if (e.ResetForm == true)
            {
                ViewModelLocator.Reset();
            }
            if (e.ShouldClose == true) 
            { 
                base.Close(); 
            }
        }
    }
}
