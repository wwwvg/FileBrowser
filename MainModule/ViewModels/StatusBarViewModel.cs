﻿using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Events;
using FileBrowser.Events;

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
        public StatusBarViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ListViewSelectionChanged>().Subscribe(SetStatusBarText);
        }

        void SetStatusBarText(string text)
        {
            About = text;
        }
    }
}
