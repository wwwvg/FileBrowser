using FileBrowser.Views;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using MainModule;
using PrismDemo.Dialogs;

namespace FileBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
           moduleCatalog.AddModule<MainModule.MainModule>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<DeleteDialog, DeleteDialogViewModel>();
            containerRegistry.RegisterDialog<AddFolderDialog, AddFolderDialogViewModel>();
        }
    }
}
