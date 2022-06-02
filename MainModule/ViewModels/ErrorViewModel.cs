using MainModule.Events;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Media.Imaging;

namespace MainModule.ViewModels
{
    public class ErrorViewModel : BindableBase 
    {
        private BitmapImage _image;
        public BitmapImage Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        public ErrorViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<Error>().Subscribe(SetMessage);
        }

        private void SetMessage(string message)
        {
            Message = message;
            if (message != "")
                Image = new BitmapImage(new Uri("..\\Icons\\warning.png", UriKind.Relative));
            else
                Image = null;
        }
    }
}
