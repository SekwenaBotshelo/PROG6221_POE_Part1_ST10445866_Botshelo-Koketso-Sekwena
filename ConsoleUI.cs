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

        // Draws a border around a message (single-line version)
        public static void PrintBorder(string message, int maxLineLength = 60)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                message = char.ToUpper(message[0]) + message.Substring(1);
            }

            // Split message into lines manually if it contains '\n'
            string[] splitByNewline = message.Split('\n');
            List<string> allWrappedLines = new List<string>();

            foreach (string line in splitByNewline)
            {
                allWrappedLines.AddRange(WrapText(line.Trim(), maxLineLength));
            }

            int width = allWrappedLines.Max(line => line.Length) + 4;

            string top = "┌" + new string('─', width) + "┐";
            string bottom = "└" + new string('─', width) + "┘";

            Console.WriteLine(top);
            foreach (string line in allWrappedLines)
            {
                Console.WriteLine($"│  {line.PadRight(width - 4)}  │");
            }
            Console.WriteLine(bottom);
        }

        public static void PrintWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static List<string> WrapText(string text, int maxLineLength)
        {
            List<string> lines = new List<string>();
            string[] words = text.Split(' ');
            string line = "";

            foreach (string word in words)
            {
                if ((line + word).Length > maxLineLength)
                {
                    lines.Add(line.TrimEnd());
                    line = "";
                }
                line += word + " ";
            }

            if (!string.IsNullOrWhiteSpace(line))
            {
                lines.Add(line.TrimEnd());
            }

            return lines;
        }
    }
}
