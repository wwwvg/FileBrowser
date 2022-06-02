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
using System.Windows.Media;
using System.Windows.Input;
using Prism.Regions;

namespace MainModule.ViewModels
{
    public class FileViewModel : BindableBase
    {
        /// <summary>
        /// FileViewModel -> отображает ComboBox с выбранным диском и ListView со списком файлов и папок.
        /// </summary>
#region СВОЙСТВА

        private FileInfoModel _selectedFile;    // выбранный файл или каталог            Icon(ImageSource) | Type(enum) | FullPath | Name | Type | Size | TimeCreated
        public FileInfoModel SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { SetProperty(ref _selectedIndex, value);}
        }

        private Stack<int> _previousSelectedIndexes = new(); 

        private ObservableCollection<FileInfoModel> _files = new(); // список файлов и каталогов
        public ObservableCollection<FileInfoModel> Files  // Файлы и каталоги         
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        private KeyEventArgs _args;
        public KeyEventArgs Args
        {
            get { return _args; }
            set { SetProperty(ref _args, value); }
        }

        IEventAggregator _eventAggregator;                  // IEventAggregator - предназначен для отправки сообщений
        IRegionManager _regionManager;
        #endregion

        #region КОМАНДЫ  

        #region ВЫБОР ФАЙЛА ИЛИ КАТАЛОГА

        private DelegateCommand _selectedCommandListView;
        public DelegateCommand SelectedCommandListView =>
            _selectedCommandListView ?? (_selectedCommandListView = new DelegateCommand(ExecuteSelectedCommandListView));

        void ExecuteSelectedCommandListView()
        {
            if(SelectedFile != null) // посылаем событие изменения выбора файла или каталога -> подписчик ->  << StatusBarViewModel >>
                _eventAggregator.GetEvent<ListViewSelectionChanged>().Publish(SelectedFile);
        }
        #endregion

        #region ДВОЙНОЙ ЩЕЛЧОК

        DelegateCommand _doubleClicked;
        public DelegateCommand DoubleClicked =>
            _doubleClicked ?? (_doubleClicked = new DelegateCommand(ExecuteDoubleClicked));

        void ExecuteDoubleClicked()
        {
            if (SelectedFile == null) 
                return;

            var parameters = new NavigationParameters();
            parameters.Add("FileInfoModel", SelectedFile);

            switch (SelectedFile.Type)
            {
                case FileType.Back:
                    SetFoldersAndFiles(SelectedFile.FullPath);
                    SelectedIndex = _previousSelectedIndexes.Pop();
                    break;
                case FileType.Folder:
                    _previousSelectedIndexes.Push(SelectedIndex);
                    SetFoldersAndFiles(SelectedFile.FullPath);
                    SelectedIndex = 0;
                    break;
                case FileType.Image:
                    _regionManager.RequestNavigate("ContentRegion", "ImageView", Callback, parameters);
                    break;
                case FileType.Text:
                    _regionManager.RequestNavigate("ContentRegion", "TextView", Callback, parameters);
                    break;
                case FileType.Bin:
                    _regionManager.RequestNavigate("ContentRegion", "HexView", Callback, parameters);
                    break;
                default:
                    break;
            }

        }

        private void Callback(NavigationResult result)
        {
            if (result.Error != null)
            {
                int i = 0;
                //handle error
            }
        }
        #endregion

        #endregion

        void SetFoldersAndFiles(string path) // добавление папок и файлов
        {
            Files.Clear();
            DirectoryInfo dir = new DirectoryInfo($"{path}");
            try
            { 
                DirectoryInfo[] directories = dir.GetDirectories();
                if (dir.Parent != null)
                {
                    Files.Add(new FileInfoModel { Icon = IconForFile.GetIconForFile(FileType.Back), Type = FileType.Back, Name = "[..]", FullPath = dir.Parent.FullName });                
                }

                foreach (var item in directories)
                {
                    if (item.Attributes == FileAttributes.Hidden || item.Attributes == FileAttributes.System)
                        continue;
                    Files.Add(new FileInfoModel { Icon = IconForFile.GetIconForFile(FileType.Folder), Type = FileType.Folder, FullPath = item.FullName, Name = $"[{item.Name}]", Size = "<Папка>", TimeCreated = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm") });
                }

                FileInfo[] files = dir.GetFiles();
                foreach (var item in files)
                {
                    FileType type;
                    if(item.Extension ==".png" || item.Extension == ".bmp" || item.Extension == ".jpg" || item.Extension == ".gif")
                        type = FileType.Image;
                    else if(item.Extension ==".txt" || item.Extension == ".cfg" || item.Extension == ".ini" || item.Extension == ".log" || item.Extension == ".csv" || item.Extension == ".xml")
                        type = FileType.Text;
                    else
                        type = FileType.Bin;

                    ImageSource imageSource = IconForFile.GetIconForFile(type);

                    Files.Add(new FileInfoModel { Icon = imageSource, Type = type, FullPath = item.FullName, Name = item.Name, Size = Bytes.SizeSuffix(item.Length), TimeCreated = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm") });
                }
            }
            catch (UnauthorizedAccessException ex) // некотрые системные папки недоступны, но если запустить программу с админскими привилегиями то все ОК.
            {
                Files.Add(new FileInfoModel { Icon = IconForFile.GetIconForFile(FileType.Back), Type = FileType.Back, Name = "[..]", FullPath = dir.Parent.FullName });
                _eventAggregator.GetEvent<Error>().Publish(ex.Message);
            } 
        }


        #region КОНСТРУКТОР
        public FileViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<DriveChanged>().Subscribe(SetFoldersAndFiles);  // диск или файл/каталог изменился -> пришло от << DriveViewModel >>        
        }
        #endregion
    }
}
