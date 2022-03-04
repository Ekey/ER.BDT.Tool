using System;
using System.IO;

namespace ER.Unpacker
{
    class Program
    {
        private static String m_Title = "ELDEN RING Binder Unpacker";

        static void Main(String[] args)
        {
            Console.Title = m_Title;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(m_Title);
            Console.WriteLine("(c) 2022 Ekey (h4x0r) / v{0}\n", Utils.iGetApplicationVersion());
            Console.ResetColor();

            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[Usage]");
                Console.WriteLine("    ER.Unpacker <m_File> <m_Directory>\n");
                Console.WriteLine("    m_File - Source of BHD file");
                Console.WriteLine("    m_Directory - Destination directory\n");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Examples]");
                Console.WriteLine("    ER.Unpacker E:\\Games\\ELDER RING\\Data0.bhd D:\\Unpacked");
                Console.ResetColor();
                return;
            }

            String m_BinderFile = args[0];
            String m_Output = Utils.iCheckArgumentsPath(args[1]);

            if (!File.Exists(m_BinderFile))
            {
                Utils.iSetError("[ERROR]: Input binder file -> " + m_BinderFile + " <- does not exist");
                return;
            }

            if (!File.Exists("oo2core_6_win64.dll"))
            {
                Utils.iSetError("[ERROR]: Unable to initialize oo2core_6_win64.dll module. Copy this library from game folder");
                return;
            }

            BinderUnpack.iDoIt(m_BinderFile, m_Output);
        }
    }
}