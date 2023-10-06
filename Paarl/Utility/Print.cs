using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paarl.Utility
{
    internal static class Print
    {
        public static void WriteInit(string message)        => Write(ConsoleColor.Cyan, "Init", message);
        public static void WriteQuestion(string message)    => Write(ConsoleColor.Magenta, "User", message);
        public static void WriteInfo(string message)        => Write(ConsoleColor.Cyan, "Info", message);
        public static void WriteSuccess(string message)     => Write(ConsoleColor.Green, "Success", message);
        public static void WriteWarning(string message)     => Write(ConsoleColor.Red, "Warning", message);
        public static void WriteLog(string message)         => Write(ConsoleColor.Blue, "Log", message);

        /// <summary>
        /// Gets the padding from prefix to message
        /// </summary>
        /// <param name="prefix">Prefix text</param>
        /// <returns></returns>
        private static int GetPadding(string prefix)
        {
            return 12 - prefix.Length;
        }

        /// <summary>
        /// Writes to console
        /// </summary>
        /// <param name="color">Colour of the prefix</param>
        /// <param name="prefix">Prefix test</param>
        /// <param name="message">Message</param>
        public static void Write(ConsoleColor color, string prefix, string message) 
        {
            Console.ResetColor();

            Console.Write("[");
            Console.ForegroundColor = color;
            Console.Write(prefix);
            Console.ResetColor();
            Console.Write("] ".PadRight(GetPadding(prefix)));

            Console.WriteLine(message);
        }
    }
}
