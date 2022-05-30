using MainModule.Events;
using MainModule.Helpers;
using MainModule.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace MainModule.ViewModels
{
    public class DriveViewModel : BindableBase
    {
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

#region ВЫБОР ЛОГИЧЕСКОГО ДИСКА

        private DelegateCommand<DriveModel> _selectedDriveCommand;     // DriveModel - передаваемый параметр, содержит картинку, имя диска, количество свободной памяти
        public DelegateCommand<DriveModel> SelectedDriveCommand =>
            _selectedDriveCommand ?? (_selectedDriveCommand = new DelegateCommand<DriveModel>(DriveSelected));

        void DriveSelected(DriveModel selectedDrive)
        {
            SelectedDrive = selectedDrive;
            SetVolumeLabelInfo(selectedDrive);
            _eventAggregator.GetEvent<DriveChanged>().Publish(selectedDrive.Name);  // посылаем событие смены диска -> подписчик ->  << FileViewModel >>
        }

        void SetVolumeLabelInfo(DriveModel selectedDrive)
        {
            FreeSpace = $"{selectedDrive.FreeSpace}"; //информация о названии диска и свободной памяти
        }
#endregion

        IEventAggregator _eventAggregator;                  
        public DriveViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            DriveInfo[] drives = DriveInfo.GetDrives(); //получаем список дисков
            if (drives.Length == 0)
                return;

            _drives = new List<DriveModel>();

            foreach (var drive in drives) //добавляем их в ComboBox
            {
                _drives.Add(new DriveModel()
                {
                    Name = $"{drive.Name} ",
                    Icon = new BitmapImage(new Uri("..\\Icons\\HardDrive.png", UriKind.Relative)),
                    FreeSpace = $"  [{drive.VolumeLabel}]  {Bytes.SizeSuffix(drive.TotalFreeSpace)} из {Bytes.SizeSuffix(drive.TotalSize)}  свободно"
                });
            }
            DriveSelected(_drives[0]); // выбираем диск C:\
            FreeSpace = $"  [{drives[0].VolumeLabel}]  {Bytes.SizeSuffix(drives[0].TotalFreeSpace)} из {Bytes.SizeSuffix(drives[0].TotalSize)} свободно"; // справа отображаем доступный объем памяти
            //_eventAggregator.GetEvent<DriveChanged>().Publish(_drives[0].Name);  // посылаем событие смены диска -> подписчик ->  << FileViewModel >>
        }
    }
}
