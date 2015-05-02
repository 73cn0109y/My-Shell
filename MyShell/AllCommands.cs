using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace MyShell
{
    public class AllCommands
    {
        public AllCommands()
        {
            //List<Command> c = Global.Commands;
            new Command() { Name = "Visual Studio", isProgram = true, Run = () => { Process.Start(@"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe"); } };
        }
    }
}
