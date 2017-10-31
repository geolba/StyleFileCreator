using System;
using System.Threading.Tasks;
using System.Windows;

namespace StyleFileCreator.App.Utils
{
    public class DialogService : GalaSoft.MvvmLight.Views.IDialogService
    {
        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
 	        throw new NotImplementedException();
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
 	        throw new NotImplementedException();
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
 	        throw new NotImplementedException();
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            var tcs = new TaskCompletionSource<bool>();
            MessageBox.Show(message, title ?? string.Empty, MessageBoxButton.OK);

            if (afterHideCallback != null)
            {
                afterHideCallback();
            }

            tcs.SetResult(true);
            return tcs.Task;
        }

        public Task ShowMessage(string message, string title)
        {
            var tcs = new TaskCompletionSource<bool>();
            ShowMessage(message, title ?? string.Empty, null, null);
            tcs.SetResult(true);
            return tcs.Task;
        }

        public Task ShowMessageBox(string message, string title)
        {
 	        throw new NotImplementedException();
        }

        public bool? ShowOpenFileDialog()
        {
            //if (ownerViewModel == null)
            //    throw new ArgumentNullException(nameof(ownerViewModel));
            //if (settings == null)
            //    throw new ArgumentNullException(nameof(settings));

            //Logger.Write($"Title: {settings.Title}");

            var dialog = new Microsoft.Win32.OpenFileDialog();
            return dialog.ShowDialog();
        }

        


    }
}
