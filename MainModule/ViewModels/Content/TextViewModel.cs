using MainModule.Events;
using MainModule.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.ViewModels.Content
{
    public class TextViewModel : BindableBase, INavigationAware
    {
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private FileInfoModel _fileInfoModel;
        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            int i = 0;
            if (navigationContext.Parameters.ContainsKey("FileInfoModel"))
            {
                _fileInfoModel = navigationContext.Parameters.GetValue<FileInfoModel>("FileInfoModel");
                SetText();
            }
            else
                _eventAggregator.GetEvent<Error>().Publish("if (n!avigationContext.Parameters.ContainsKey(\"FileInfoModel\")");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        void SetText()
        {
            try
            {
                Text = "";
                using (StreamReader reader = new StreamReader(_fileInfoModel.FullPath, Encoding.UTF8))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Text += line + "\n";
                    }
                }
            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<Error>().Publish(ex.Message);
            }
        }

        async Task SetTextAsync()
        {
            string text = string.Empty;
            // асинхронное чтение
            using (StreamReader reader = new StreamReader(_fileInfoModel.FullPath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    text += line + "\n";
                }
            }
            Text = text;
        }


        public TextViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }
    }
}
