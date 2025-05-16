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

        /// Draws a border around a message (single-line version)
        public static void PrintBorder(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                message = char.ToUpper(message[0]) + message.Substring(1);
            }

            int width = message.Length + 4; // Padding on both sides
            string top = "┌" + new string('─', width) + "┐";
            string middle = $"│  {message}  │";
            string bottom = "└" + new string('─', width) + "┘";

            Console.WriteLine(top);
            Console.WriteLine(middle);
            Console.WriteLine(bottom);
        }
    }
}
