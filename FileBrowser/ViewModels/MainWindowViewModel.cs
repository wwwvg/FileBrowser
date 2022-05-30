using Prism.Mvvm;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileBrowser.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public string Title { get; set; } = "File browser";

        public MainWindowViewModel()
        {

        }
    }
}
