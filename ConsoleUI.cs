using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POE_Part1_Chatbot
{
    /// <summary>
    /// Handles all console formatting and display logic.
    /// Implements visual effects like colored text and animated typing.
    /// </summary>
    internal class ConsoleUI
    {
        /// <summary>
        /// Prints a green horizontal divider for section separation
        /// </summary>
        public static void PrintDivider()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n===================================");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays the input prompt with yellow coloring
        /// </summary>
        public static void PromptInput()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n> ");
            Console.ResetColor();
        }

        /// <summary>
        /// Shows error messages in red
        /// </summary>
        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Simulates typing animation for chatbot responses
        /// </summary>
        public static void TypeLine(string text)
        {
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(25); // 25ms delay per character
            }
            Console.WriteLine();
        }
    }
}
