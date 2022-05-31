using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.Services
{
    internal interface IAddDeleteFile
    {
        bool DeleteItem(string path);
        bool AddFolder(string path);
    }
}
