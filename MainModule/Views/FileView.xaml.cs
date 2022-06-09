using System.Windows.Controls;

namespace MainModule.Views
{
    /// <summary>
    /// Interaction logic for FileView
    /// </summary>
    public partial class FileView : UserControl
    {
        public FileView()
        {
            InitializeComponent();
        }

        private void ListView_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //if (e.Source is ListView)
            //    return;
            //e.Handled = true;
        }
    }
}
