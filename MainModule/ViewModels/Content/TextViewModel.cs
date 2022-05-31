using MainModule.Models;
using Prism.Commands;
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
        private FileInfoModel _fileInfoModel;
        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("FileInfoModel"))
                return true;
            return false;
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


        public TextViewModel()
        {

        }
    }
}
