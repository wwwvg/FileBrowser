using Prism.Mvvm;

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

        public MainWindowViewModel()
        {

        }
    }
}
