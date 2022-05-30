using MainModule.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MainModule.Models
{
    /// <summary>
    /// Модель для списка файлов/папок: информация о выбранном файле/папки
    /// </summary>
    public class FileInfoModel
    {
        public ImageSource Icon { get; set; }
        public string FullPath { get; set; }
        public string Name { get; set; }
        public FileType Type { get; set; }
        public string Size { get; set; }
        public string TimeCreated { get; set; }
    }
}
