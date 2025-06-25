using System;
using System.Collections.Generic;
using System.Linq;
using POE_Part1_Chatbot.Core;

namespace POE_Part1_Chatbot
{
    internal class Chatbot : ChatSession
    {
        #region State Fields
        private string _userName;
        private readonly Dictionary<string, string> _userPreferences = new();
        private bool _awaitingTipConfirmation = false;
        private readonly Random _random = new();

        // Enhanced conversation tracking
        private string _currentTopic = "";
        private string _currentSentiment = "";
        private const int MAX_HISTORY = 5;
        private readonly Queue<string> _conversationHistory = new(MAX_HISTORY);
        #endregion

        #region Core Session
        public override void StartSession()
        {
            InitializeUser();
            ShowMenu();
            RunChatLoop();
        }

        private void RunChatLoop()
        {
            string input;
            while (true)
            {
                input = GetUserInput();
                if (string.IsNullOrEmpty(input)) continue;

                UpdateConversationState(input);

                if (ShouldExit(input)) break;
                if (HandleSpecialCases(input)) continue;

                ProcessUserInput(input);
            }

            DisplayExitMessage();
        }
        #endregion

        #region Input Processing
        private string GetUserInput()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"\n--> {_userName}: ");
            Console.ResetColor();

            string input = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(input))
            {
                ConsoleUI.PrintError("Please enter a valid message.");
                return "";
            }

            return SanitizeInput(input);
        }

        private string SanitizeInput(string input)
        {
            var cleanInput = new string(input
                .Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '?' || c == '\'')
                .ToArray())
                .Trim();

            return cleanInput.Length > 500 ? cleanInput[..500] : cleanInput;
        }

        private void UpdateConversationState(string input)
        {
            // Maintain conversation history
            if (_conversationHistory.Count >= MAX_HISTORY)
                _conversationHistory.Dequeue();
            _conversationHistory.Enqueue(input);

            // Detect context
            _currentTopic = ResponseManager.DetectTopic(input) ?? _currentTopic;
            _currentSentiment = ResponseManager.DetectSentiment(input) ?? _currentSentiment;
        }
        #endregion

        #region Conversation Logic
        private bool ShouldExit(string input)
        {
            string[] exitKeywords = { "exit", "quit", "bye", "goodbye" };
            if (exitKeywords.Any(k => input.Contains(k)))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n--> {_userName} has left the chat.");
                Console.ResetColor();
                return true;
            }
            return false;
        }

        private bool HandleSpecialCases(string input)
        {
            return TryUpdateUserTopic(input) ||
                   HandleUncertainTopic(input);
        }

        private void ProcessUserInput(string input)
        {
            DisplayUserInput(input);
            string response = BuildResponse(input);
            DisplayResponse(response);
            HandleTipConfirmation(input);
        }
        #endregion

        #region Response Handling
        private string BuildResponse(string input)
        {
            string response = ResponseManager.GetResponse(input, _userName);

            // Add sentiment-aware enhancements
            if (ShouldEnhanceResponse(input))
            {
                response = EnhanceResponse(response, input);
            }

            // Add personalized touches
            if (ShouldPersonalizeResponse())
            {
                response = PersonalizeResponse(response);
            }

            return response;
        }

        private bool ShouldEnhanceResponse(string input) =>
            (input.Contains("?") || input.Contains("explain")) &&
            !string.IsNullOrEmpty(_currentTopic);

        private string EnhanceResponse(string response, string input)
        {
            if (input.Contains("simpl") || _currentSentiment == "confused")
                return $"{response}\n\n{ResponseManager.GetSimplifiedExplanation(_currentTopic)}";

            if (input.Contains("reassure") || _currentSentiment == "anxious")
                return $"{response}\n\n{ResponseManager.GetReassuringExplanation(_currentTopic)}";

            return $"{response}\n\n{ResponseManager.GetDetailedExplanation(_currentTopic)}";
        }

        private bool ShouldPersonalizeResponse() =>
            _userPreferences.TryGetValue("interest", out _) &&
            _random.Next(4) == 0; // 25% chance

        private string PersonalizeResponse(string response)
        {
            string topic = _userPreferences["interest"];
            string[] personalizations =
            {
                $"As someone interested in {topic}, consider this: ",
                $"Since you care about {topic}, note that: ",
                $"This relates to your interest in {topic}: "
            };
            return personalizations[_random.Next(personalizations.Length)] + response;
        }
        #endregion

        #region User Interaction
        private void DisplayUserInput(string input)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n--> {_userName}");
            ConsoleUI.PrintBorder(input);
            Console.ResetColor();
        }

        private void DisplayResponse(string response)
        {
            // Apply sentiment-based coloring
            Console.ForegroundColor = ResponseManager.GetSentimentColor(_currentSentiment);
            Console.WriteLine($"\n--> Bot");
            ConsoleUI.PrintBorder(response);
            Console.ResetColor();

            // Add suggestions if no topic set
            if (!_userPreferences.ContainsKey("interest"))
            {
                ConsoleUI.PrintBorder("(Try: 'My favorite topic is privacy')");
            }
        }

        private void HandleTipConfirmation(string input)
        {
            if (!_awaitingTipConfirmation) return;

            ConsoleUI.PromptInput();
            string confirmInput = Console.ReadLine()?.ToLower().Trim();

            if (ResponseManager.IsAffirmative(confirmInput))
            {
                string tips = GetContextualTips();
                DisplayResponse(tips);
            }
            _awaitingTipConfirmation = false;
        }

        private string GetContextualTips()
        {
            if (!string.IsNullOrEmpty(_currentTopic))
                return ResponseManager.GetRandomTip(_currentTopic);

            return _userPreferences.TryGetValue("interest", out string topic)
                ? ResponseManager.GetRandomTip(topic)
                : ResponseManager.GetRandomTip("security");
        }
        #endregion

        #region Topic Management
        private bool TryUpdateUserTopic(string input)
        {
            string newTopic = input.Contains("my favorite topic is")
                ? input.Split(new[] { "my favorite topic is" }, StringSplitOptions.RemoveEmptyEntries)
                      .LastOrDefault()?.Trim()
                : input.StartsWith("i'm interested in") || input.StartsWith("i am interested in")
                    ? input.Replace("i'm interested in", "")
                          .Replace("i am interested in", "")
                          .Trim()
                    : null;

            if (!string.IsNullOrEmpty(newTopic))
            {
                UpdateUserTopic(newTopic);
                return true;
            }
            return false;
        }

        private void UpdateUserTopic(string newTopic)
        {
            _userPreferences["interest"] = newTopic;
            _currentTopic = newTopic;
            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleUI.PrintBorder($"I'll remember you're interested in {newTopic}. " +
                                ResponseManager.GetTopicImportance(newTopic));
            Console.ResetColor();
        }

        private bool HandleUncertainTopic(string input)
        {
            if (input.Contains("don't know my favorite topic") ||
                input.Contains("still exploring"))
            {
                ConsoleUI.PrintBorder("No problem! Explore topics at your own pace.");
                return true;
            }
            return false;
        }
        #endregion

        #region Initialization
        private void InitializeUser()
        {
            ConsoleUI.PrintDivider();
            Console.Write("Please enter your name: ");
            _userName = Console.ReadLine()?.Trim();
            _userPreferences["name"] = _userName;

            ConsoleUI.TypeLine($"\nWelcome, {_userName}! I'm here to help you stay cyber-safe.");

            Console.Write("What is your favorite cyber security topic? ");
            string topic = Console.ReadLine()?.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(topic))
            {
                UpdateUserTopic(topic);
            }
        }

        public void ShowMenu()
        {
            var topics = ResponseManager.GetAllTopics();
            ConsoleUI.PrintBorder("Available Cybersecurity Topics:");

            foreach (var topic in topics)
            {
                string importance = ResponseManager.GetTopicImportance(topic);
                ConsoleUI.PrintWithColor($"• {topic.PadRight(25)} {importance}",
                                       ResponseManager.GetSentimentColor("positive"));
            }

            ConsoleUI.PrintBorder("\nYou can:");
            ConsoleUI.PrintWithColor("1. Ask about any topic", ConsoleColor.Cyan);
            ConsoleUI.PrintWithColor("2. Get a random security tip", ConsoleColor.Cyan);
            ConsoleUI.PrintWithColor("3. Exit", ConsoleColor.Cyan);
            ConsoleUI.PrintBorder("What would you like to do?");
        }

        private void DisplayExitMessage()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleUI.TypeLine("Stay safe online! Goodbye.");
            Console.ResetColor();
        }
        #endregion
    }
}