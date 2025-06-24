using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POE_Part1_Chatbot.Core;

namespace POE_Part1_Chatbot
{
    /// Core chatbot logic - manages user interaction loop.
    /// Coordinates UI, input processing, and response generation.
    internal class Chatbot : ChatSession
    {
        private string _userName; // Stores authenticated user's name
        private Dictionary<string, string> _userPreferences = new Dictionary<string, string>(); // Stores chatbot's memory for the session
        private bool _awaitingTipConfirmation = false;

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
            _userPreferences["name"] = _userName;

            ConsoleUI.TypeLine($"\nWelcome, {_userName}! I'm here to help you stay cyber-safe.");

            Console.Write("What is your favorite cyber security topic (e.g., phishing, passwords, browsing)? ");
            string topic = Console.ReadLine()?.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(topic))
            {
                _userPreferences["interest"] = topic;
            }
        }

        // Creating the Menu feature for the user
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
            Console.WriteLine("- What is cyber security awareness");
            Console.WriteLine("- What is phishing");
            Console.WriteLine("- What is password safety");
            Console.WriteLine("- What is safe browsing");
            Console.WriteLine("- What is privacy");
            Console.WriteLine("- What is a scam");
            Console.WriteLine("- Tips about [topic]");
            Console.WriteLine("- Type 'exit' to leave the chat");
            ConsoleUI.PrintDivider();
        }

        // Input Validation
        private string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // Remove special characters that might cause issues
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '?' || c == '\'')
                {
                    sb.Append(c);
                }
            }

            string sanitized = sb.ToString().Trim();

            // Ensure we don't try to get substring longer than the string
            return sanitized.Length > 500 ? sanitized.Substring(0, 500) : sanitized;
        }

        private void RunChatLoop()
        {
            string input;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"\n--> {_userName}: ");
                Console.ResetColor();

                input = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    ConsoleUI.PrintError("Please enter a valid message.");
                    continue;
                }

                input = SanitizeInput(input);
                if (string.IsNullOrEmpty(input))
                {
                    ConsoleUI.PrintError("Please enter a valid message.");
                    continue;
                }

                // Exit conditions
                List<string> exitKeywords = new List<string> { "exit", "quit", "bye", "goodbye", "leave", "end", "stop" };
                if (exitKeywords.Any(keyword => input.Contains(keyword)))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n--> {_userName} has left the chat.");
                    Console.ResetColor();
                    break;
                }

                // Handle favorite topic updates
                if (input.Contains("my favorite topic is"))
                {
                    string[] parts = input.Split(new[] { "my favorite topic is" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        string newTopic = parts[1].Trim();
                        _userPreferences["interest"] = newTopic;

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        ConsoleUI.PrintBorder($"Thanks! I've updated your favorite topic to '{newTopic}'.");
                        Console.ResetColor();
                        continue;
                    }
                }

                // Handle other ways users express interest in topics
                if (input.StartsWith("i'm interested in") || input.StartsWith("i am interested in"))
                {
                    string newTopic = input.Replace("i'm interested in", "")
                                           .Replace("i am interested in", "")
                                           .Trim();
                    if (!string.IsNullOrEmpty(newTopic))
                    {
                        _userPreferences["interest"] = newTopic;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        ConsoleUI.PrintBorder($"Thanks for sharing! I've noted that you're interested in '{newTopic}'.");
                        Console.ResetColor();
                        continue;
                    }
                }

                // Handle uncertain topic input
                if (input.Contains("don't have a favorite topic") ||
                    input.Contains("not sure what my favorite topic is") ||
                    input.Contains("still exploring") ||
                    input.Contains("i don't know my favorite topic"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    ConsoleUI.PrintBorder("No problem! You can use me to research and learn what might become your favorite topic.");
                    Console.ResetColor();
                    continue;
                }

                // Print user input with border
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n--> {_userName}");
                ConsoleUI.PrintBorder(input);
                Console.ResetColor();

                // Generate base response
                string response = ResponseManager.GetResponse(input, _userName);

                // Add suggestions if favorite topic not yet set
                if (!_userPreferences.ContainsKey("interest"))
                {
                    response += "\n(You can tell me your favorite topic anytime by saying: 'My favorite topic is phishing'.)";
                }

                // Personalized enhancements if topic is known
                if (_userPreferences.TryGetValue("interest", out string favTopic))
                {
                    if (input.Contains("tip") || input.Contains("advice"))
                    {
                        response += $"\nBy the way, since you're interested in {favTopic}, would you like tips on that?";
                        _awaitingTipConfirmation = true;
                    }

                    if (input.Contains("who am I") || input.Contains("what's my name"))
                    {
                        response += $"\nYou're {_userName}, and your favorite cyber topic is {favTopic}.";
                    }
                }

                // Print bot response with border
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n--> Bot");
                ConsoleUI.PrintBorder(response);
                Console.ResetColor();

                // Handle tip confirmation if needed
                if (_awaitingTipConfirmation)
                {
                    ConsoleUI.PromptInput();
                    string confirmInput = Console.ReadLine()?.ToLower().Trim();

                    string[] affirmatives = { "yes", "yes please", "sure", "ok", "okay", "yep", "yeah", "alright" };
                    string[] negatives = { "no", "nope", "not now", "later", "maybe later" };

                    if (affirmatives.Any(a => confirmInput.Contains(a)))
                    {
                        Random tipRand = new Random();
                        string tipsResponse;

                        if (tipRand.Next(2) == 0) // 50% chance
                        {
                            tipsResponse = ResponseManager.GetResponse($"give me some {favTopic} tips", _userName);
                        }
                        else
                        {
                            tipsResponse = ResponseManager.GetResponse($"{favTopic} tips", _userName);
                        }

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"\n--> Bot");
                        ConsoleUI.PrintBorder(tipsResponse);
                        Console.ResetColor();
                    }
                    else if (negatives.Any(n => confirmInput.Contains(n)))
                    {
                        string[] noProblemResponses =
                        {
                            "Okay, let me know if you want tips later!",
                            "No problem! You can ask anytime.",
                            "Sure thing! I'm here when you're ready.",
                            "Understood! Just ask when you need tips.",
                            "Got it! The offer stands whenever you're interested."
                        };

                        Random responseRand = new Random();
                        string noProblemResponse = noProblemResponses[responseRand.Next(noProblemResponses.Length)];

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"\n--> Bot");
                        ConsoleUI.PrintBorder(noProblemResponse);
                        Console.ResetColor();
                    }
                    _awaitingTipConfirmation = false;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleUI.TypeLine("Goodbye! Stay safe online.");
            Console.ResetColor();
        }
    }
}