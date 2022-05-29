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
               
       

        private DriveModel _selectedDrive;                  // выбранный диск           Name | Icon(ImageSource) | FreeSpace
        public DriveModel SelectedDrive
        {
            get => _selectedDrive;
            set => SetProperty(ref _selectedDrive, value);
        }

        private List<DriveModel> _drives;                   // все диски
        public List<DriveModel> Drives 
        {
            get => _drives;
            set => SetProperty(ref _drives, value);
        }

        private string _freeSpace;                          // метка логического диска и количество доступной памяти на нём
        public string FreeSpace
        {
            get => _freeSpace;
            set => SetProperty(ref _freeSpace, value);
        }

        IEventAggregator _eventAggregator;                  // IEventAggregator - предназначен для отправки сообщений
        #endregion

#region КОМАНДЫ

    #region ВЫБОР ЛОГИЧЕСКОГО ДИСКА

        private DelegateCommand<DriveModel> _selectedCommandComboBox;     // DriveModel - передаваемый параметр, содержит картинку, имя диска, количество свободной памяти
        public DelegateCommand<DriveModel> SelectedCommandComboBox =>
            _selectedCommandComboBox ?? (_selectedCommandComboBox = new DelegateCommand<DriveModel>(SelectedDriveChanged));

        void SelectedDriveChanged(DriveModel selectedDrive)
        {
            SetVolumeLabelInfo(selectedDrive);
            SetRootFoldersAndFiles();
            _eventAggregator.GetEvent<ListViewSelectionChanged>().Publish("");  // посылаем событие смены диска -> подписчик ->  << StatusBarViewModel >>
        }

        void SetVolumeLabelInfo(DriveModel selectedDrive)
        {
            FreeSpace = $"{selectedDrive.FreeSpace}"; //информация о названии диска и свободной памяти
            
        }

        void SetRootFoldersAndFiles() // добавление папок и файлов
        {
            Files.Clear();
            try
            {
                DirectoryInfo dir = new DirectoryInfo($"{SelectedDrive.Name.Replace(" ", "")}:\\");
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
    #endregion

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
            DriveInfo[] drives = DriveInfo.GetDrives(); //получаем список дисков
            if (drives.Length == 0)
                return;

            _drives = new List<DriveModel>();

            foreach (var drive in drives) //добавляем их в ComboBox
            {
                _drives.Add(new DriveModel() { Name = $"{drive.Name.Replace(":\\", "")} ", 
                    Icon = new BitmapImage(new Uri("/MainModule;component/Icons/HardDrive.png", UriKind.Relative)),
                    FreeSpace = $"  [{drive.VolumeLabel}]  {Bytes.SizeSuffix(drive.TotalFreeSpace)} из {Bytes.SizeSuffix(drive.TotalSize)}  свободно"
                });              
            }
            SelectedDrive = _drives[0]; // выбираем диск C:\
            FreeSpace = $"  [{drives[0].VolumeLabel}]  {Bytes.SizeSuffix(drives[0].TotalFreeSpace)} из {Bytes.SizeSuffix(drives[0].TotalSize)} свободно"; // справа отображаем доступный объем памяти
            SetRootFoldersAndFiles(); // список заполняем папками и файлами 
        }
#endregion

        
    }
}
