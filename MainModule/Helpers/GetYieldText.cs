using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainModule.Helpers
{
    public class GetYieldText : IDisposable
    {
        /*
        public IEnumerable<string> AllLinesFromFile(string path)
        {
            LinkedList<string> Lines = new LinkedList<string>();
            StringBuilder text = new StringBuilder();
            string[] digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
            using (BinaryReader br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 32)))
            {
                br.BaseStream.Seek(0, SeekOrigin.Begin);
                byte[] inbuff = new byte[0];
                int b = 0;
                int count = 0;
                int countRow = 0;
                while ((inbuff = br.ReadBytes(16)).Length > 0)
                {
                    for (b = 0; b < inbuff.Length - 1; b++)
                    {
                        text.Append(digits[(inbuff[b] / 16) % 16] + digits[inbuff[b] % 16] + " ");
                        count++;
                    }
                    text.Append(digits[(inbuff[b] / 16) % 16] + digits[inbuff[b] % 16] + "\n");
                    //Lines.AddLast(text.ToString());
                    yield return text.ToString();
                    text.Clear();
                }
            }
        }
        */

        private BinaryReader _br;
        public string AllLinesFromFile(int linesPerRequest)
        {
            LinkedList<string> Lines = new LinkedList<string>();
            StringBuilder text = new StringBuilder();
            string[] digits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

            
            

            byte[] inbuff = new byte[0];
            int b = 0;
            int count = 0;
            //_br.BaseStream.Position = pos;
            //_br.BaseStream.Seek(_linesPerRequest * 32, SeekOrigin.Current);
            while ((inbuff = _br.ReadBytes(16)).Length > 0)
            {
                //_br.BaseStream.Seek(10, SeekOrigin.Begin);
                for (b = 0; b < inbuff.Length - 1; b++)
                {
                    text.Append(digits[(inbuff[b] / 16) % 16] + digits[inbuff[b] % 16] + " ");
                }
                text.Append(digits[(inbuff[b] / 16) % 16] + digits[inbuff[b] % 16] + "\n");
                if (count++ == linesPerRequest)
                    break;
            }

            return text.ToString();
        }

        public GetYieldText(string path)
        {
            _br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None, 1024));
        }

        public void Dispose()
        {
            _br.Dispose();
        }
    }
}
