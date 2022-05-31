using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.Services
{
    internal class AddDeleteFile : IAddDeleteFile
    {
        public bool AddFolder(string path)
        {
            return true;
        }

        public bool DeleteItem(string path)
        {
            return true;
        }
    }
}
