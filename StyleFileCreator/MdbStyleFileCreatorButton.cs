using System;
using System.Windows;
using ESRI.ArcGIS.Framework;
using System.Windows.Forms.Integration;//for enabling keyboard input
using StyleFileCreator.App;

namespace StyleFileCreator
{
    public class MdbStyleFileCreatorButton : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        #region fields:

        private IApplication m_application;

        #endregion

        #region constructors

        public MdbStyleFileCreatorButton():base()
        {
            m_application = ArcMap.Application;
        }

        #endregion

       

        protected override void OnClick()
        {
            try
            {
                //MainWindow wpfwindow = new MainWindow(m_application);
                App.ViewModel.ViewModelLocator.Application = m_application;
                MainWizard wpfwindow = new MainWizard();

                //Enable Keyboard Input:
                ElementHost.EnableModelessKeyboardInterop(wpfwindow);
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(wpfwindow);
                helper.Owner = (IntPtr)ArcMap.Application.hWnd;//winFormWindow.Handle.
                wpfwindow.Show();
                
                ArcMap.Application.CurrentTool = null;
            }
            catch (Exception ex)
            {
                //System.Diagnostics.Trace.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
          
        }

        protected override void OnUpdate()
        {
            base.Enabled = ArcMap.Application != null;
        }
    }

}
