using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using MainModule.Views;
using MainModule.Services;
using MainModule.ViewModels;

namespace MainModule
{
    public class MainModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public MainModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("ToolBarRegion", typeof(ToolBarView));
            _regionManager.RegisterViewWithRegion("FileListRegion", typeof(FileView)); 
            _regionManager.RegisterViewWithRegion("StatusBarRegion", typeof(StatusBarView));
            _regionManager.RegisterViewWithRegion("DriveRegion", typeof(DriveView));
            _regionManager.RegisterViewWithRegion("ContentRegion", typeof(ImageView)); 
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<FileView>();
            containerRegistry.RegisterSingleton<IAddDeleteFile, AddDeleteFile>();
            containerRegistry.RegisterSingleton<FileViewModel>();
        }
    }
}
