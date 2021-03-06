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
    public class DeleteDialogViewModel : BindableBase, IDialogAware
    {
        #region СВОЙСТВА
        IEventAggregator _eventAggregator;
        public string Title => "File Browser";

        private string _textRecycleBin = "Переместить в корзину";
        public string TextRecycleBin
        {
            get { return _textRecycleBin; }
            set { SetProperty(ref _textRecycleBin, value); }
        }

        private bool _moveToRecycleBin = true;
        public bool MoveToRecycleBin
        {
            get { return _moveToRecycleBin; }
            set { SetProperty(ref _moveToRecycleBin, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        #endregion

        #region КОМАНДЫ УДАЛЕНИЯ И ОТМЕНЫ
        public DelegateCommand CancelCommand { get; }
        public DelegateCommand DeleteCommand { get; }

        public event Action<IDialogResult> RequestClose;

        private void CloseDialogAndDelete()
        {
            var buttonResult = ButtonResult.Yes;
            var parameters = new DialogParameters();
            parameters.Add("MoveToRecycleBin", MoveToRecycleBin);  // информация (bool) о том, что следует ли файл/каталог помещать в корзину или убить сразу
            try
            {
                RequestClose?.Invoke(new DialogResult(buttonResult, parameters)); // возврат из диалога с информацией о нажатой кнопке и доп. информации (см. выше)
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
            Message = $"Вы уверены, что хотите удалить выбранный элемент {parameters.GetValue<string>("message")}?";
        }
        #endregion

        #region КОНСТРУКТОР
        public DeleteDialogViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            CancelCommand = new DelegateCommand(CloseDialog);
            DeleteCommand = new DelegateCommand(CloseDialogAndDelete);
        }
        #endregion
    }
}
