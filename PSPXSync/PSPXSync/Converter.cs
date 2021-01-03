using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSPXSync
{
    public class Converter
    {
        private static string _conversionProgram = "mcr2vmp.exe";

        public static void Run(string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C {_conversionProgram} \"{arguments}\"" ;
            process.StartInfo = startInfo;
            process.Start();

        }
    }
}
