using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POE_Part1_Chatbot.Core;


namespace POE_Part1_Chatbot
{
    /// Core chatbot logic - manages user interaction loop.
    /// Coordinates UI, input processing, and response generation.
    internal class Chatbot : ChatSession
    {
        private string _userName; // Stores authenticated user's name

        /// Starts the main chat loop after initialization.
        public override void StartSession()
        {
            InitializeUser();
            ShowMenu(); // Show prompts before starting chat loop
            RunChatLoop();
        }

        /// Collects user information and displays welcome message.
        private void InitializeUser()
        {
            ConsoleUI.PrintDivider();
            Console.Write("Please enter your name: ");
            _userName = Console.ReadLine()?.Trim();
            ConsoleUI.TypeLine($"\nWelcome, {_userName}! I'm here to help you stay cyber-safe.");
        }

        /// Creating the Menu feature for the user
        private void ShowMenu()
        {
            ConsoleUI.PrintDivider();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("--> You can ask me about the following topics:");
            Console.ResetColor();
            Console.WriteLine("- How are you");
            Console.WriteLine("- What is your purpose");
            Console.WriteLine("- What can I ask you about");
            Console.WriteLine("- What tips can I ask about");
            Console.WriteLine("- What is cyber security awareness exactly");
            Console.WriteLine("- What is phishing");
            Console.WriteLine("- What is password safety");
            Console.WriteLine("- What is safe browsing");
            Console.WriteLine("- Type 'exit' to leave the chat");
            ConsoleUI.PrintDivider();
        }

        /// Continuous loop for processing user input until 'exit' command.
        private void RunChatLoop()
        {
            string input;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n--> {_userName}: ");
                Console.ResetColor();

                input = Console.ReadLine()?.ToLower().Trim();

                List<string> exitKeywords = new List<string> { "exit", "quit", "bye", "goodbye", "leave", "end", "stop" };
                if (exitKeywords.Any(keyword => input.Contains(keyword)))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n--> {_userName} has left the chat.");
                    Console.ResetColor();
                    break;
                }


                if (string.IsNullOrWhiteSpace(input))
                {
                    ConsoleUI.PrintError("Please enter a valid message.");
                    continue;
                }

                // Print user input with border
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n--> {_userName}");
                ConsoleUI.PrintBorder(input);
                Console.ResetColor();

                // Get bot response
                string response = ResponseManager.GetResponse(input, _userName);

                // Print bot response with border
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n--> Bot");
                ConsoleUI.PrintBorder(response);
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleUI.TypeLine("Goodbye! Stay safe online.");
            Console.ResetColor();
        }
    }
}
