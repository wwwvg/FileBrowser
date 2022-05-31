using MainModule.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.ViewModels.Content
{
    public class HexViewModel : BindableBase
    {
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
            if (navigationContext.Parameters.ContainsKey("FileInfoModel"))
            {
                _fileInfoModel = navigationContext.Parameters.GetValue<FileInfoModel>("FileInfoModel");
                SetText();
            }
            //SetTextAsync();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        void SetText()
        {
            Text = "";
            using (StreamReader reader = new StreamReader(_fileInfoModel.FullPath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    Text += line + "\n";
                }
            }
        }

        async Task SetTextAsync()
        {
            // асинхронное чтение
            using (StreamReader reader = new StreamReader(_fileInfoModel.FullPath))
            {
                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    Text += line + "\n";
                }
            }
        }
        public HexViewModel()
        {

        }
    }
}
