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
        private string _userName;
        private Dictionary<string, string> _userPreferences = new Dictionary<string, string>();
        private bool _awaitingTipConfirmation = false;

        // Memory and recall fields
        private Stack<string> _conversationHistory = new Stack<string>();
        private string _currentTopic = "";
        private bool _userExpressedConfusion = false;
        private const int MAX_HISTORY = 5;
        private Random _random = new Random();

        /// Starts the main chat loop after initialization.
        public override void StartSession()
        {
            InitializeUser();
            ShowMenu();
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
                _currentTopic = topic;
                ConsoleUI.PrintBorder($"I'll remember you're interested in {topic}. " +
                                    ResponseManager.GetTopicImportance(topic));
            }
        }

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

        private string SanitizeInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '?' || c == '\'')
                {
                    sb.Append(c);
                }
            }

            string sanitized = sb.ToString().Trim();
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

                UpdateConversationContext(input);

                if (CheckExitConditions(input)) break;
                if (HandleTopicUpdates(input)) continue;

                DisplayUserInput(input);

                string response = GenerateContextAwareResponse(input);
                DisplayResponse(response, input);  // Pass input as parameter

                HandleTipConfirmation(input);  // Pass input as parameter
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleUI.TypeLine("Goodbye! Stay safe online.");
            Console.ResetColor();
        }

        private void UpdateConversationContext(string input)
        {
            // Maintain conversation history
            if (_conversationHistory.Count >= MAX_HISTORY)
            {
                _conversationHistory.Pop();
            }
            _conversationHistory.Push(input);

            // Detect user confusion or requests for more info
            _userExpressedConfusion = input.Contains("don't understand") ||
                                    input.Contains("explain more") ||
                                    input.Contains("clarify") ||
                                    input.Contains("confused");

            // Detect topic mentions
            foreach (var topic in ResponseManager.GetAllTopics())
            {
                if (input.Contains(topic))
                {
                    _currentTopic = topic;
                    break;
                }
            }
        }

        private bool CheckExitConditions(string input)
        {
            List<string> exitKeywords = new List<string> { "exit", "quit", "bye", "goodbye", "leave", "end", "stop" };
            if (exitKeywords.Any(keyword => input.Contains(keyword)))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n--> {_userName} has left the chat.");
                Console.ResetColor();
                return true;
            }
            return false;
        }

        private bool HandleTopicUpdates(string input)
        {
            if (input.Contains("my favorite topic is"))
            {
                string[] parts = input.Split(new[] { "my favorite topic is" }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1)
                {
                    UpdateUserTopic(parts[1].Trim());
                    return true;
                }
            }

            if (input.StartsWith("i'm interested in") || input.StartsWith("i am interested in"))
            {
                string newTopic = input.Replace("i'm interested in", "")
                                     .Replace("i am interested in", "")
                                     .Trim();
                if (!string.IsNullOrEmpty(newTopic))
                {
                    UpdateUserTopic(newTopic);
                    return true;
                }
            }

            if (input.Contains("don't have a favorite topic") ||
                input.Contains("not sure what my favorite topic is") ||
                input.Contains("still exploring") ||
                input.Contains("i don't know my favorite topic"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                ConsoleUI.PrintBorder("No problem! You can use me to research and learn what might become your favorite topic.");
                Console.ResetColor();
                return true;
            }

            return false;
        }

        private void UpdateUserTopic(string newTopic)
        {
            _userPreferences["interest"] = newTopic;
            _currentTopic = newTopic;
            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleUI.PrintBorder($"Thanks! I've updated your favorite topic to '{newTopic}'. " +
                                ResponseManager.GetTopicImportance(newTopic));
            Console.ResetColor();
        }

        private void DisplayUserInput(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n--> {_userName}");
            ConsoleUI.PrintBorder(input);
            Console.ResetColor();
        }

        private string GenerateContextAwareResponse(string input)
        {
            string baseResponse = ResponseManager.GetResponse(input, _userName);

            if (_userExpressedConfusion && !string.IsNullOrEmpty(_currentTopic))
            {
                return $"{baseResponse}\n\nLet me explain more about {_currentTopic}. " +
                      ResponseManager.GetDetailedExplanation(_currentTopic);
            }

            if (!string.IsNullOrEmpty(_currentTopic) &&
                (input.Contains("more") || input.Contains("what else")))
            {
                return $"{baseResponse}\n\nSince we're discussing {_currentTopic}, " +
                      ResponseManager.GetRelatedInformation(_currentTopic);
            }

            if (!string.IsNullOrEmpty(_currentTopic) &&
                _random.Next(4) == 0) // 25% chance to personalize
            {
                return PersonalizeResponse(baseResponse, _currentTopic);
            }

            return baseResponse;
        }

        private string PersonalizeResponse(string response, string topic)
        {
            string[] personalizations =
            {
                $"As someone interested in {topic}, you might find this relevant: ",
                $"Since we've been discussing {topic}, you should know: ",
                $"This relates to your interest in {topic}: "
            };
            return personalizations[_random.Next(personalizations.Length)] + response;
        }

        private void DisplayResponse(string response, string input)
        {
            if (!_userPreferences.ContainsKey("interest"))
            {
                response += "\n(You can tell me your favorite topic anytime by saying: 'My favorite topic is phishing'.)";
            }

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

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n--> Bot");
            ConsoleUI.PrintBorder(response);
            Console.ResetColor();
        }

        private void HandleTipConfirmation(string input)
        {
            if (!_awaitingTipConfirmation) return;

            ConsoleUI.PromptInput();
            string confirmInput = Console.ReadLine()?.ToLower().Trim();

            string[] affirmatives = { "yes", "yes please", "sure", "ok", "okay", "yep", "yeah", "alright" };
            string[] negatives = { "no", "nope", "not now", "later", "maybe later" };

            if (affirmatives.Any(a => confirmInput.Contains(a)))
            {
                string tipsResponse;

                if (!string.IsNullOrEmpty(_currentTopic))
                {
                    tipsResponse = $"About {_currentTopic}, here's a tip: " +
                                 ResponseManager.GetRandomTip(_currentTopic + " tips");
                }
                else if (_userPreferences.TryGetValue("interest", out string favTopic))
                {
                    tipsResponse = ResponseManager.GetResponse($"{favTopic} tips", _userName);
                }
                else
                {
                    tipsResponse = "Here's a general security tip: " +
                                 ResponseManager.GetRandomTip("password tips");
                }

                DisplayResponse(tipsResponse, input);
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

                string noProblemResponse = noProblemResponses[_random.Next(noProblemResponses.Length)];
                DisplayResponse(noProblemResponse, input);
            }
            _awaitingTipConfirmation = false;
        }
    }
}