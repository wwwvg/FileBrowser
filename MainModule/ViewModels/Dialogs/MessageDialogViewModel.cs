using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrismDemo.Dialogs
{
    public class MessageDialogViewModel : BindableBase, IDialogAware
    {
        public string Title => "Удалить";

        public ImageSource Icon;// => new BitmapImage(new Uri("..\\Icons\\Warning.png", UriKind.Relative));

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public DelegateCommand CancelCommand { get; }

        public event Action<IDialogResult> RequestClose;

        public MessageDialogViewModel()
        {
            CancelCommand = new DelegateCommand(CloseDialog);
            Uri iconUri = new Uri("pack://application:,,,Icons/Warning.png", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(iconUri);
        }

        private void CloseDialog()
        {
            var buttonResult = ButtonResult.Cancel;

            var parameters = new DialogParameters();
            parameters.Add("myParam", "The Dialog was closed by the user.");

            RequestClose?.Invoke(new DialogResult(buttonResult, parameters));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Message = parameters.GetValue<string>("message");
        }
    }
}
