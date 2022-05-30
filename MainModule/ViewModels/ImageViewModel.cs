using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MainModule.ViewModels
{
    public class ImageViewModel : BindableBase
    {
        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }
        public ImageViewModel()
        {
            Image = new BitmapImage(new Uri("..\\Icons\\Back.png", UriKind.Relative));
        }
    }
}
