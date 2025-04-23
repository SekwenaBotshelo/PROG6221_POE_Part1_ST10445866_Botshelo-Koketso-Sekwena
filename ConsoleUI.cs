using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POE_Part1_Chatbot
{
    /// Handles all console formatting and display logic.
    /// Implements visual effects like colored text and animated typing.
    internal class ConsoleUI
    {
        /// Prints a green horizontal divider for section separation
        public static void PrintDivider()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n===================================");
            Console.ResetColor();
        }

        /// Displays the input prompt with yellow coloring
        public static void PromptInput()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n> ");
            Console.ResetColor();
        }

        /// Shows error messages in red
        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor(); 
        }

        /// Simulates typing animation for chatbot responses
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
