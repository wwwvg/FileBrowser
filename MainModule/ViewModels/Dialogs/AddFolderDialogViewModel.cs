using MainModule.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PrismDemo.Dialogs
{
    public class AddFolderDialogViewModel : BindableBase, IDialogAware
    {
        IEventAggregator _eventAggregator;  // для отправки сообщений об ошибках
        public string Title => "File Browser";

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _newFolderName;
        public string NewFolderName
        {
            get { return _newFolderName; }
            set { SetProperty(ref _newFolderName, value); }
        }

        public DelegateCommand CancelCommand { get; }
        public DelegateCommand AddCommand { get; }

        public event Action<IDialogResult> RequestClose;

        private void CloseDialogAndCreateFolder()
        {
            var buttonResult = ButtonResult.Yes;
            var parameters = new DialogParameters();
            parameters.Add("NewFolderName", NewFolderName);

            try
            {
                RequestClose?.Invoke(new DialogResult(buttonResult, parameters));
            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<Error>().Publish(ex.Message);
            }
        }

        private void CloseDialog()
        {
            var buttonResult = ButtonResult.Cancel;
            try
            {
                RequestClose?.Invoke(new DialogResult(buttonResult));
            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<Error>().Publish(ex.Message);
            }

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
            Message = $"Создать новый каталог (папку):";
        }

        public AddFolderDialogViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            CancelCommand = new DelegateCommand(CloseDialog);
            AddCommand = new DelegateCommand(CloseDialogAndCreateFolder);
        }
    }
}
