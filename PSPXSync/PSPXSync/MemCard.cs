using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSPXSync
{
    public class MemCard
    {
        public string Path;
        public DateTime Modified;

        public MemCard(string path)
        {
            if (path == null)
            {
                return;
            }

            if (!path.EndsWith("mcr") && !path.EndsWith("0.VMP"))
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    if (file.EndsWith("0.VMP"))
                    {
                        path = file;
                        break;
                    }
                }
            }

            this.Path = path;
            this.Modified = File.GetLastWriteTime(path);
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
