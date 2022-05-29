using Prism.Mvvm;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileBrowser.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "File browser";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        //private ImageSource _icon;
        //public ImageSource Icon
        //{
        //    get { return _icon; }
        //    set { SetProperty(ref _icon, value); }
        //}

        public MainWindowViewModel()
        {
            //Icon = new BitmapImage(new Uri("/FileBrowser;component/Icons/Browser.png", UriKind.Relative));
        }
    }
}
