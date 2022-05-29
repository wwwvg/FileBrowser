using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.Models
{
    /// <summary>
    /// Модель для списка файлов/папок: информация о выбранном файле/папки
    /// </summary>
    public class FileInfoModel
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string TimeCreated { get; set; }
    }
}
