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
using System.Windows;

namespace MainModule.ViewModels
{
    public class FileViewModel : BindableBase
    {
        /// <summary>
        /// FileViewModel -> отображает ComboBox с выбранным диском и ListView со списком файлов и папок.
        /// </summary>

        public FileViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<DriveChanged>().Subscribe(OnDiskChanged);  // диск или файл/каталог изменился -> пришло от << DriveViewModel >>
            _eventAggregator.GetEvent<RefreshRequested>().Subscribe(Refresh);  // нажата кнопка обновить на тулбаре -> пришло от << ToolBarViewModel >> 
        }

        #region СВОЙСТВА

        IEventAggregator _eventAggregator;

        IRegionManager _regionManager;

        private bool _isError = false; // для своевременного удаления сообщения об ошибке

        private FileInfoModel _selectedFile;    // выбранный файл или каталог            Icon(ImageSource) | Type(enum) | FullPath | Name | Type | Size | TimeCreated
        public FileInfoModel SelectedFile
        {
            get => _selectedFile;
            set => SetProperty(ref _selectedFile, value);
        }

        private string _currentDirectory;
        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set { SetProperty(ref _currentDirectory, value); }
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

        private string _nameOfSelectedItem;
        public string NameOfSelectedItem
        {
            get { return _nameOfSelectedItem; }
            set { SetProperty(ref _nameOfSelectedItem, value); }
        }
        #endregion

        #region КОМАНДЫ  

        #region ВЫБОР ФАЙЛА ИЛИ КАТАЛОГА

        private DelegateCommand _selectedCommandListView;
        public DelegateCommand SelectedCommandListView =>
            _selectedCommandListView ?? (_selectedCommandListView = new DelegateCommand(ExecuteSelectedCommandListView));

        private void ExecuteSelectedCommandListView()
        {
            if (SelectedFile != null) 
            {
                _eventAggregator.GetEvent<ListViewSelectionChanged>().Publish(SelectedFile);// посылаем событие изменения выбора файла или каталога ->
                                                                                            // подписчики ->  << StatusBarViewModel >> и << ToolBarViewModel >>
            }
            if(_isError == false)
                _eventAggregator.GetEvent<Error>().Publish(""); // очистить информацию об ошибке

            string s = NameOfSelectedItem;
            if (SelectedFile == null)
                return;

            var parameters = new NavigationParameters();
            parameters.Add("FileInfoModel", SelectedFile);

            switch (SelectedFile.Type)
            {
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
        #endregion

        #region ДВОЙНОЙ ЩЕЛЧОК ИЛИ ENTER

        DelegateCommand _doubleClicked;
        public DelegateCommand DoubleClicked =>
            _doubleClicked ?? (_doubleClicked = new DelegateCommand(ExecuteDoubleClicked));

        private void ExecuteDoubleClicked()
        {
            string s = NameOfSelectedItem;
            if (SelectedFile == null) 
                return;

            var parameters = new NavigationParameters();
            parameters.Add("FileInfoModel", SelectedFile);

            switch (SelectedFile.Type)
            {
                case FileType.Back:
                    SetFoldersAndFiles(SelectedFile.FullPath); // обновить список файлов
                    SelectedIndex = _previousSelectedIndexes.Pop(); // взять мз стека и установить индекс родительской папки
                    _eventAggregator.GetEvent<Error>().Publish(""); // очистить информацию об ошибке
                    break;
                case FileType.Folder:
                    _previousSelectedIndexes.Push(SelectedIndex);  // поместить в стек индекс папки, с которой осуществляется заход
                    SetFoldersAndFiles(SelectedFile.FullPath);  // обновить список файлов
                    SelectedIndex = 0;
                    break;
                default:
                    break;
            }
        }

        private void Callback(NavigationResult result)  // не используется
        {
            if (result.Error != null)
            {
                //handle error
            }
        }
        #endregion

        #endregion

        private void SetFoldersAndFiles(string path) // добавление папок и файлов
        {
            Files.Clear();
            DirectoryInfo dir = new DirectoryInfo($"{path}");
            try
            { 
                DirectoryInfo[] directories = dir.GetDirectories();
                CurrentDirectory = dir.FullName;
                _eventAggregator.GetEvent<CurrentDirectoryChanged>().Publish(CurrentDirectory); // посылаем событие изменения текущей директории
                if (dir.Parent != null)
                {
                    Files.Add(new FileInfoModel { Icon = IconForFile.GetIconForFile(FileType.Back), Type = FileType.Back, Name = "[..]", FullPath = dir.Parent.FullName });                
                }

                foreach (var item in directories)
                {
                    _isError = false;
                    if (item.Attributes == FileAttributes.Hidden || item.Attributes == FileAttributes.System)
                        continue;
                    Files.Add(new FileInfoModel { Icon = IconForFile.GetIconForFile(FileType.Folder), Type = FileType.Folder, FullPath = item.FullName, Name = $"[{item.Name}]", Size = "<Папка>", TimeCreated = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm") });
                }

                FileInfo[] files = dir.GetFiles();
                foreach (var item in files)
                {
                    _isError = false;
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

            catch (Exception ex) // некоторые системные папки и файлы недоступны, но если запустить программу с админскими привилегиями то все ОК.
            {
                _isError = true;
                Files.Add(new FileInfoModel { Icon = IconForFile.GetIconForFile(FileType.Back), Type = FileType.Back, Name = "[..]", FullPath = dir.Parent.FullName });
                _eventAggregator.GetEvent<Error>().Publish(ex.Message);     
            } 
        }

        private void OnDiskChanged(string path)
        {
            SetFoldersAndFiles(path);
            if (Files.Count != 0)
                SelectedIndex = 0;
        }

        private void Refresh()   // обновление списка файлов и каталогов
        {
            int selectedIndex = SelectedIndex;
            SetFoldersAndFiles(_currentDirectory);
            if (Files.Count > 0)
                SelectedIndex = 0;
            else if (selectedIndex < Files.Count)
            {
                SelectedIndex = selectedIndex;
            }
        }
    }
}
