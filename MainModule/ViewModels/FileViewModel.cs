using MainModule.Helpers;
using MainModule.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using FileBrowser.Events;
using MainModule.Events;

namespace MainModule.ViewModels
{
    public class FileViewModel : BindableBase
    {
        /// <summary>
        /// FileViewModel -> отображает ComboBox с выбранным диском и ListView со списком файлов и папок.
        /// </summary>
#region СВОЙСТВА

        private FileInfoModel _selectedFile;                // выбранный файл или каталог            FullPath | Name | Type | Size | TimeCreated
        public FileInfoModel SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        private ObservableCollection<FileInfoModel> _files = new(); // список файлов и каталогов
        public ObservableCollection<FileInfoModel> Files  // Файлы и каталоги         
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        IEventAggregator _eventAggregator;                  // IEventAggregator - предназначен для отправки сообщений
        #endregion

#region КОМАНДЫ  
       
    #region ВЫБОР ФАЙЛА ИЛИ КАТАЛОГА

        private DelegateCommand _selectedCommandListView;
        public DelegateCommand SelectedCommandListView =>
            _selectedCommandListView ?? (_selectedCommandListView = new DelegateCommand(ExecuteSelectedCommandListView));

        void ExecuteSelectedCommandListView()
        {
            if(SelectedFile != null) // посылаем событие изменения выбора файла или каталога -> подписчик ->  << StatusBarViewModel >>
                _eventAggregator.GetEvent<ListViewSelectionChanged>().Publish($"Путь: {SelectedFile.FullPath}         Размер: {SelectedFile.Size}         Дата и время изменения: {SelectedFile.TimeCreated}");
        }
    #endregion

#endregion

#region КОНСТРУКТОР

        public FileViewModel(IEventAggregator eventAggregator)                              
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<DriveChanged>().Subscribe(SetRootFoldersAndFiles);  // диск или файл/каталог изменился -> пришло от << DriveViewModel >>
        }
#endregion

        void SetRootFoldersAndFiles(DriveModel driveModel) // добавление папок и файлов
        {
            Files.Clear();
            try
            {
                DirectoryInfo dir = new DirectoryInfo($"{driveModel.Name}");
                DirectoryInfo[] directories = dir.GetDirectories();
                foreach (var item in directories)
                {
                    Files.Add(new FileInfoModel { FullPath = item.FullName, Name = $"[{item.Name}]", Size = "<Папка>", TimeCreated = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm") });
                }

                FileInfo[] files = dir.GetFiles();
                foreach (var item in files)
                {
                    Files.Add(new FileInfoModel { FullPath = item.FullName, Name = item.Name, Size = Bytes.SizeSuffix(item.Length), TimeCreated = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm") });
                }

            }
            catch (UnauthorizedAccessException)
            {
                // Code here will be hit if access is denied. You can also
                // leave this empty to ignore the error.
            }
        }
    }
}
