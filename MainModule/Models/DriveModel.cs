using System.Windows.Media;

namespace MainModule.Models
{
    /// <summary>
    /// Модель для выбора диска: информация о выбранном диске
    /// </summary>
    public class DriveModel
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public string FreeSpace { get; set; }
    }
}
