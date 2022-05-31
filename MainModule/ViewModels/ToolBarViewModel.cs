using FileBrowser.Events;
using MainModule.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MainModule.ViewModels
{
    public class ToolBarViewModel : BindableBase
    {
        /// <summary>
        /// FileViewModel посылает сообщение об изменении выбранного файла/каталога. Проверяется их путь и на основании этого определяется доступность кнопок.
        /// </summary>
        #region СВОЙСТВА

        private readonly IDialogService _dialogService;

        private string _messageReceived;
        public string MessageReceived
        {
            get { return _messageReceived; }
            set { SetProperty(ref _messageReceived, value); }
        }

        private bool _canAddFolder; // доступность кнопки Добавить папку
        public bool CanAddFolder
        {
            get { return _canAddFolder; }
            set { SetProperty(ref _canAddFolder, value); }
        }

        private bool _canDeleteItem;
        public bool CanDeleteItem  // доступность кнопки Удалить папку/файл
        {
            get { return _canDeleteItem; }
            set { SetProperty(ref _canDeleteItem, value); }
        }
        public ImageSource DeleteItemImage { get; set; }
        public ImageSource AddFolderImage { get; set; }
        #endregion

        #region КОМАНДЫ И ОБРАБОТЧИКИ
        #region ДОБАВИТЬ
        private DelegateCommand _addFolder; 
        public DelegateCommand AddFolder =>
            _addFolder ?? (_addFolder = new DelegateCommand(ExecuteAddFolder, CanExecuteAddFolder).ObservesProperty(() => CanAddFolder));

        void ExecuteAddFolder()
        {
            ShowDialog();
        }

        bool CanExecuteAddFolder()
        {
            return CanAddFolder;
        }
        void ChangeCanExecuteAddFolder(FileInfoModel fileInfoModel)
        {
            if (fileInfoModel == null || fileInfoModel.FullPath == null)
            {
                CanAddFolder = false;
                return;
            }
            CanAddFolder = true;
        }
        #endregion

        #region УДАЛИТЬ
        private DelegateCommand _deleteItem;
        public DelegateCommand DeleteItem =>
            _deleteItem ?? (_deleteItem = new DelegateCommand(ExecuteDeleteItem, CanExecuteDeleteItem).ObservesProperty(() => CanDeleteItem));

        void ExecuteDeleteItem()
        {
            
        }

        bool CanExecuteDeleteItem()
        {
            return CanDeleteItem;
        }

        void ChangeCanExecuteDeleteItem(FileInfoModel fileInfoModel)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(fileInfoModel.FullPath);

            if (fileInfoModel == null || fileInfoModel.FullPath == null || fileInfoModel.Type == Helpers.FileType.Back)
            {
                CanDeleteItem = false;
                return;
            }
            CanDeleteItem = true;
        }
        #endregion
        #endregion
        public ToolBarViewModel(IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _canDeleteItem = false;
            _canAddFolder = false;
            DeleteItemImage = new BitmapImage(new Uri("..\\Icons\\DeleteItem.png", UriKind.Relative));
            AddFolderImage = new BitmapImage(new Uri("..\\Icons\\AddFolder.png", UriKind.Relative));
            eventAggregator.GetEvent<ListViewSelectionChanged>().Subscribe(ChangeCanExecuteAddFolder);  // диск или файл/каталог изменился -> пришло от << FileViewModel >>
            eventAggregator.GetEvent<ListViewSelectionChanged>().Subscribe(ChangeCanExecuteDeleteItem);  // диск или файл/каталог изменился -> пришло от << FileViewModel >>
            _dialogService = dialogService;
        }
        private void ShowDialog()
        {
            _dialogService.ShowMessageDialog("Hello from ViewAViewModel", r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    MessageReceived = r.Parameters.GetValue<string>("myParam");
                }
                else
                {
                    MessageReceived = "Not closed by user";
                }
            });
        }
    }
}