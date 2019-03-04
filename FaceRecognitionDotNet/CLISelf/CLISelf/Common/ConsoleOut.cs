using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.CLISelf.Common
{
    class ConsoleOut : TextWriter
    {

        private ConsoleOut(TextWriter output)
        {
            _out = output;
        }

        private TextWriter _out;

        public override Encoding Encoding
        {
            get { return _out.Encoding; }
        }

        public override void Write(char value)
        {
            if (__outputEnabled)
                _out.Write(value);
        }

        private static ConsoleOut __instance;

        public static void Install()
        {
            if (__instance == null)
            {
                __instance = new ConsoleOut(Console.Out);
                Console.SetOut(__instance);
            }
        }

        private static bool __outputEnabled;

        public static bool OutputEnabled
        {
            get { return __outputEnabled; }
            set { __outputEnabled = value; }
        }

    }
}
