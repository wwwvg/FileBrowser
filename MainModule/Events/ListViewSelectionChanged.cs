using MainModule.Models;
using Prism.Events;

namespace FileBrowser.Events
{
    public class ListViewSelectionChanged : PubSubEvent<FileInfoModel>
    {

    }
}
