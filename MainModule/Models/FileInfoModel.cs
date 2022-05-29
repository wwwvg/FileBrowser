using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.Models
{
    public class FileInfoModel
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string TimeCreated { get; set; }
    }
}
