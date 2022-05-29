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
        #region СВОЙСТВА
        private ObservableCollection<FileInformation> _files = new();
        public ObservableCollection<FileInformation> Files
        {
            get { return _files; }
            set { SetProperty(ref _files, value); }
        }

        private DriveModel _selectedDrive; // выбранный диск
        public DriveModel SelectedDrive
        {
            get => _selectedDrive;
            set => SetProperty(ref _selectedDrive, value);
        }

        private FileInformation _selectedFile;
        public FileInformation SelectedFile
        {
            get { return _selectedFile; }
            set { SetProperty(ref _selectedFile, value); }
        }

        private List<DriveModel> _drives; // все диски
        public List<DriveModel> Drives 
        {
            get => _drives;
            set => SetProperty(ref _drives, value);
        }

        private string _freeSpace; // volume label и количество доступной памяти на диске
        public string FreeSpace
        {
            get => _freeSpace;
            set => SetProperty(ref _freeSpace, value);
        }

        IEventAggregator _eventAggregator;
        #endregion

#region КОМАНДЫ

    #region ComboBoxSelection
        private DelegateCommand<DriveModel> _selectedCommandComboBox;     // DriveModel - передаваемый параметр, содержит картинку, имя диска, количество свободной памяти
        public DelegateCommand<DriveModel> SelectedCommandComboBox =>
            _selectedCommandComboBox ?? (_selectedCommandComboBox = new DelegateCommand<DriveModel>(SelectedDriveChanged));

        void SelectedDriveChanged(DriveModel selectedDrive)
        {
            SetVolumeLabelInfo(selectedDrive);
            SetRootFoldersAndFiles();
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
                    Files.Add(new FileInformation { FullPath = item.FullName, Name = $"[{item.Name}]", Size = "<Папка>", TimeCreated = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm") });
                }

                FileInfo[] files = dir.GetFiles();
                foreach (var item in files)
                {
                    Files.Add(new FileInformation { FullPath = item.FullName, Name = item.Name, Size = Bytes.SizeSuffix(item.Length), TimeCreated = item.LastWriteTime.ToString("dd/MM/yyyy  hh:mm") });
                }

            }
            catch (UnauthorizedAccessException)
            {
                // Code here will be hit if access is denied. You can also
                // leave this empty to ignore the error.
            }
        }
    #endregion

    #region ListViewSelection

        private DelegateCommand _selectedCommandListView;
        public DelegateCommand SelectedCommandListView =>
            _selectedCommandListView ?? (_selectedCommandListView = new DelegateCommand(ExecuteSelectedCommandListView));

        void ExecuteSelectedCommandListView()
        {
            if(SelectedFile != null)
                _eventAggregator.GetEvent<ListViewSelectionChanged>().Publish($"{SelectedFile.FullPath}         Размер: {SelectedFile.Size}         Дата и время изменения: {SelectedFile.TimeCreated}");
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
                    FreeSpace = $"  [{drive.VolumeLabel}]  {Bytes.SizeSuffix(drive.TotalFreeSpace)} из {Bytes.SizeSuffix(drive.TotalSize)}"
                });              
            }
            SelectedDrive = _drives[0];
            FreeSpace = $"  [{drives[0].VolumeLabel}]  {Bytes.SizeSuffix(drives[0].TotalFreeSpace)} из {Bytes.SizeSuffix(drives[0].TotalSize)}";
            SetRootFoldersAndFiles();
        }
#endregion

        
    }
}
