using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POE_Part1_Chatbot.Core;


namespace POE_Part1_Chatbot
{
    /// <summary>
    /// Core chatbot logic - manages user interaction loop.
    /// Coordinates UI, input processing, and response generation.
    /// </summary>
    internal class Chatbot
    {
        private string _userName; // Stores authenticated user's name

        /// <summary>
        /// Starts the main chat loop after initialization.
        /// </summary>
        public void Start()
        {
            InitializeUser();
            RunChatLoop();
        }

        /// <summary>
        /// Collects user information and displays welcome message.
        /// </summary>
        private void InitializeUser()
        {
            ConsoleUI.PrintDivider();
            Console.Write("Please enter your name: ");
            _userName = Console.ReadLine()?.Trim();
            ConsoleUI.TypeLine($"\nWelcome, {_userName}! I'm here to help you stay cyber-safe.");
        }

        /// <summary>
        /// Continuous loop for processing user input until 'exit' command.
        /// </summary>
        private void RunChatLoop()
        {
            string input;
            while (true)
            {
                ConsoleUI.PromptInput(); // Show '>' prompt
                input = Console.ReadLine()?.ToLower().Trim();

                // Exit condition
                if (input == "exit") break;

                // Validate input
                if (string.IsNullOrWhiteSpace(input))
                {
                    ConsoleUI.PrintError("Please enter a valid message.");
                    continue;
                }

                // Generate and display response
                string response = ResponseManager.GetResponse(input, _userName);
                ConsoleUI.TypeLine(response); // Animated typing effect
            }

            ConsoleUI.TypeLine("Goodbye! Stay safe online.");


        }
    }
}
