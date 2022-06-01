using System.Windows;

namespace MainModule.Helpers
{
    public class ListViewItem : System.Windows.Controls.ListViewItem
    {
        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);

            if (IsSelected)
            {
                Focus();
            }
        }
    }
}
