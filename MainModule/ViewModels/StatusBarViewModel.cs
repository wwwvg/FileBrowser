using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Events;
using FileBrowser.Events;
using MainModule.Models;
using MainModule.Events;

namespace MainModule.ViewModels
{
    public class StatusBarViewModel : BindableBase
    {
        private string _about;
        public string About
        {
            get { return _about; }
            set { SetProperty(ref _about, value); }
        }
        void SetStatusBarText(FileInfoModel selectedFile)
        {
            About = $"Путь: {selectedFile.FullPath}         Размер: {selectedFile.Size}         Дата и время изменения: {selectedFile.TimeCreated}";
        }
        public StatusBarViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ListViewSelectionChanged>().Subscribe(SetStatusBarText);  // диск или файл/каталог изменился -> пришло от << FileViewModel >>
        }
    }
}

