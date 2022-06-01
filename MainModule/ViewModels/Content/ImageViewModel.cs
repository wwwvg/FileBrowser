using MainModule.Events;
using MainModule.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MainModule.ViewModels.Content
{
    public class ImageViewModel : BindableBase, INavigationAware
    {
        private FileInfoModel _fileInfoModel;

        private IEventAggregator _eventAggregator;

        private ImageSource _image;
        public ImageSource Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("FileInfoModel"))
            {
                _fileInfoModel = navigationContext.Parameters.GetValue<FileInfoModel>("FileInfoModel");
                SetImage();
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        void SetImage()
        {
            try
            {
                Image = new BitmapImage(new Uri(_fileInfoModel.FullPath, UriKind.Absolute));
            }
            catch(Exception ex)
            {
                _eventAggregator.GetEvent<Error>().Publish(ex.Message);
            }
        }

        public ImageViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
    }
}
