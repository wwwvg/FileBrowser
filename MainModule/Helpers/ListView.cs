using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MainModule.Helpers
{
    public class ListView : System.Windows.Controls.ListView
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ListViewItem();
        }
    }
}
