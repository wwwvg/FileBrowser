using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using MainModule.Views;
using MainModule.ViewModels;
using MainModule.Views.Content;

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
            _regionManager.RegisterViewWithRegion("ErrorRegion", typeof(ErrorView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ImageView>();
            containerRegistry.RegisterForNavigation<HexView>();
            containerRegistry.RegisterForNavigation<TextView>();
        }
    }
}
