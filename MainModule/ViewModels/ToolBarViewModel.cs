using FileBrowser.Events;
using MainModule.Events;
using MainModule.Models;
using MainModule.ViewModels.Dialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.VisualBasic.FileIO;

namespace MainModule.ViewModels
{
    public class ToolBarViewModel : BindableBase
    {
        /// <summary>
        /// FileViewModel посылает сообщение об изменении выбранного файла/каталога. Проверяется их путь и на основании этого определяется доступность кнопок.
        /// </summary>
        #region СВОЙСТВА

        private FileInfoModel _fileInfoModel;

        private readonly IDialogService _dialogService;

        private string _currentDirectory;

        private IEventAggregator _eventAggregator;

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
        public ImageSource RefreshImage { get; set; }
        #endregion

        #region КОМАНДЫ И ОБРАБОТЧИКИ
        #region ДОБАВИТЬ
        private DelegateCommand _addFolder; 
        public DelegateCommand AddFolder =>
            _addFolder ?? (_addFolder = new DelegateCommand(ExecuteAddFolder, CanExecuteAddFolder).ObservesProperty(() => CanAddFolder));

        void ExecuteAddFolder()
        {
            ShowAddFolderDialog();
        }

        bool CanExecuteAddFolder()
        {
            return CanAddFolder;
        }
        void ChangeCanExecuteAddFolder(string currentDirectory)
        {
            if (currentDirectory == null || currentDirectory == string.Empty)
            {
                CanAddFolder = false;
                return;
            }
            _currentDirectory = currentDirectory;
            CanAddFolder = true;
        }
        void ShowAddFolderDialog()
        {
            _dialogService.ShowAddFolderDialog(_currentDirectory, r =>
            {
                if (r.Result == ButtonResult.Yes)
                {
                    try
                    {
                        string newFolderName = r.Parameters.GetValue<string>("NewFolderName");
                        newFolderName = Path.Combine(_currentDirectory, newFolderName);
                        FileSystem.CreateDirectory(newFolderName);
                        _eventAggregator.GetEvent<RefreshRequested>().Publish();   //==========> сообщение FileViewModel на обновление списка файлов
                    }
                    catch (Exception ex)
                    {
                        _eventAggregator.GetEvent<Error>().Publish(ex.Message);
                    }
                }
            });
        }
        #endregion

        #region УДАЛИТЬ
        private DelegateCommand _deleteItem;
        public DelegateCommand DeleteItem =>
            _deleteItem ?? (_deleteItem = new DelegateCommand(ExecuteDeleteItem, CanExecuteDeleteItem).ObservesProperty(() => CanDeleteItem));

        void ExecuteDeleteItem()
        {
            ShowDeleteDialog();
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

            _fileInfoModel = fileInfoModel;
            CanDeleteItem = true;
        }
        #endregion

        #region ОБНОВИТЬ
        private DelegateCommand _refresh;
        public DelegateCommand Refresh =>
            _refresh ?? (_refresh = new DelegateCommand(ExecuteRefresh));

        void ExecuteRefresh()
        {
            _eventAggregator.GetEvent<RefreshRequested>().Publish();
        }
        #endregion
        #endregion
        private void ShowDeleteDialog()
        {
            _dialogService.ShowDeleteDialog(_fileInfoModel?.FullPath, r =>
            {
                if (r.Result == ButtonResult.Yes)
                {
                    try
                    {
                        bool moveToRecycleBin = r.Parameters.GetValue<bool>("MoveToRecycleBin");
                        if(moveToRecycleBin)
                        {
                            if (_fileInfoModel.Type == Helpers.FileType.Folder)
                                FileSystem.DeleteDirectory(_fileInfoModel.FullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                            if (_fileInfoModel.Type == Helpers.FileType.Text || _fileInfoModel.Type == Helpers.FileType.Bin || _fileInfoModel.Type == Helpers.FileType.Image)
                                FileSystem.DeleteFile(_fileInfoModel.FullPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                        }
                        else
                        {
                            if (_fileInfoModel.Type == Helpers.FileType.Folder)
                                Directory.Delete(_fileInfoModel.FullPath, true);
                            if (_fileInfoModel.Type == Helpers.FileType.Text || _fileInfoModel.Type == Helpers.FileType.Bin || _fileInfoModel.Type == Helpers.FileType.Image)
                                File.Delete(_fileInfoModel.FullPath);
                        }
                        _eventAggregator.GetEvent<RefreshRequested>().Publish();   //==========> сообщение FileViewModel на обновление списка файлов
                    }
                    catch (Exception ex)
                    {
                        _eventAggregator.GetEvent<Error>().Publish(ex.Message);
                    }
                }
            });
        }
        public ToolBarViewModel(IEventAggregator eventAggregator, IDialogService dialogService)
        {
            _eventAggregator = eventAggregator;
            _canDeleteItem = false; // кнопки не доступны сначала
            _canAddFolder = true; // кнопки не доступны сначала
            DeleteItemImage = new BitmapImage(new Uri("..\\Icons\\DeleteItem.png", UriKind.Relative));
            AddFolderImage = new BitmapImage(new Uri("..\\Icons\\AddFolder.png", UriKind.Relative));
            RefreshImage = new BitmapImage(new Uri("..\\Icons\\Refresh.png", UriKind.Relative));
            eventAggregator.GetEvent<CurrentDirectoryChanged>().Subscribe(ChangeCanExecuteAddFolder);  // диск или файл/каталог изменился -> пришло от << FileViewModel >>
            eventAggregator.GetEvent<ListViewSelectionChanged>().Subscribe(ChangeCanExecuteDeleteItem);  // диск или файл/каталог изменился -> пришло от << FileViewModel >>
            _dialogService = dialogService;
        }
    }
}