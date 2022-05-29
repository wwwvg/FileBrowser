using Prism.Mvvm;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MainModule.ViewModels
{
    public class ToolBarViewModel : BindableBase
    {
        public ImageSource DeleteItemImage { get; set; }
        public ImageSource AddFolderImage { get; set; }
        public ToolBarViewModel()
        {
            DeleteItemImage = new BitmapImage(new Uri("/MainModule;component/Icons/DeleteItem.png", UriKind.Relative));
            AddFolderImage = new BitmapImage(new Uri("/MainModule;component/Icons/AddFolder.png", UriKind.Relative));
        }
    }
}
