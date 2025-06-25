using System;
using System.Collections.Generic;
using System.Linq;

namespace POE_Part1_Chatbot.Core
{
    internal static class ResponseManager
    {
        #region Configuration
        private static readonly Random _random = new();
        private static string _lastTopic = "";
        private static string _lastSentiment = "";
        private const int MaxHistory = 5;
        private static readonly Queue<string> _conversationHistory = new(MaxHistory);
        #endregion

        #region Knowledge Bases
        private static readonly Dictionary<string, string> _definitions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "purpose", "The purpose of this chatbot is to raise awareness about online safety and help you recognize potential cyber threats." },
            { "cyber security awareness", "Understanding online risks such as malware, phishing, hacking, and scams, as well as how to safeguard your devices, personal data, and yourself." },
            { "phishing", "Fraudulent attempts to obtain sensitive information by disguising as trustworthy entities." },
            { "password safety", "Using strong, unique passwords combined with other security measures to prevent unauthorized access to accounts and sensitive data." },
            { "safe browsing", "Practicing safe habits while using the internet to protect against online threats like malware and malicious websites." },
            { "password", "A secret string of characters used to verify identity and secure access to systems and data." },
            { "scam", "A dishonest scheme conducted online aimed at tricking individuals into giving away personal information or money." },
            { "privacy", "The right and practice of protecting personal and sensitive information from unauthorized access or disclosure." }
        };

        private static readonly Dictionary<string, List<string>> _tips = new(StringComparer.OrdinalIgnoreCase)
        {
            { "phishing", new List<string> {
                "Always verify sender email addresses",
                "Never enter credentials from email links",
                "Use multi-factor authentication"
            }},
            { "privacy", new List<string> {
                "Review app permissions monthly",
                "Use unique passwords for each account",
                "Enable privacy settings on social media"
            }},
            { "password", new List<string> {
                "Use at least 12 characters",
                "Combine letters, numbers and symbols",
                "Avoid using personal information"
            }}
        };

        private static readonly Dictionary<string, SentimentProfile> _sentimentProfiles = new()
        {
            {
                "frustrated",
                new SentimentProfile(
                    new[] { "angry", "frustrated", "annoyed", "pissed" },
                    new[] { "I hear your frustration about {0}. Let's break this down.",
                          "This is understandably aggravating. For {0}, try these steps:" },
                    ConsoleColor.Yellow,
                    GetSimplifiedExplanation)
            },
            {
                "anxious",
                new SentimentProfile(
                    new[] { "worried", "nervous", "scared", "anxious" },
                    new[] { "I understand your concern about {0}. These protections help:",
                          "Your caution about {0} is wise. Here's how to stay safe:" },
                    ConsoleColor.Cyan,
                    GetReassuringExplanation)
            },
            {
                "confused",
                new SentimentProfile(
                    new[] { "confused", "don't understand", "what does", "how does" },
                    new[] { "Let me clarify {0} simply:",
                          "{0} can be tricky. Here's the key concept:" },
                    ConsoleColor.Magenta,
                    GetSimplifiedExplanation)
            },
            {
                "positive",
                new SentimentProfile(
                    new[] { "thanks", "appreciate", "helpful", "good" },
                    new[] { "Glad I could help with {0}! Would you like more details?",
                          "Happy to assist with {0}! Here's something extra:" },
                    ConsoleColor.Green,
                    GetDetailedExplanation)
            }
        };
        #endregion

        #region Public Interface
        public static string GetResponse(string input, string userName)
        {
            UpdateConversationHistory(input);

            string topic = DetectTopic(input);
            string sentiment = DetectSentiment(input);

            if (!string.IsNullOrEmpty(topic))
                _lastTopic = topic;
            if (!string.IsNullOrEmpty(sentiment))
                _lastSentiment = sentiment;

            return BuildResponse(input, topic, sentiment);
        }

        public static string GetRandomTip(string topic)
        {
            return _tips.TryGetValue(topic, out var topicTips) && topicTips.Count > 0
                ? topicTips[_random.Next(topicTips.Count)]
                : "Here's a general security tip: Update your software regularly.";
        }

        public static List<string> GetAllTopics() => _definitions.Keys.Concat(_tips.Keys).Distinct().ToList();
        public static ConsoleColor GetSentimentColor(string sentiment) =>
            _sentimentProfiles.TryGetValue(sentiment, out var profile) ? profile.Color : ConsoleColor.White;
        public static bool IsAffirmative(string input) =>
            new[] { "yes", "y", "please", "sure" }.Any(x => input?.Contains(x) ?? false);

        public static string GetDetailedExplanation(string topic)
        {
            var explanations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "phishing", "Technical Details:\n• Attack vectors: Email (75%), SMS (20%), Voice (5%)\n• Common lures: Fake invoices, account alerts\n• Protection: SPF/DKIM validation, AI filters" },
                { "privacy", "In-Depth View:\n• Data types: PII, behavioral, derived\n• Regulations: GDPR, CCPA compliance\n• Tools: Encryption, VPNs, tokenization" }
            };
            return explanations.TryGetValue(topic, out string explanation)
                ? explanation
                : $"Detailed {topic} information:\n• Technical aspects\n• Best practices\n• Advanced protections";
        }

        public static string GetTopicImportance(string topic)
        {
            var importanceLevels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "phishing", "★★★★★ (Critical)" },
                { "password", "★★★★★ (Critical)" },
                { "privacy", "★★★★ (High)" },
                { "scam", "★★★★ (High)" },
                { "safe browsing", "★★★ (Medium)" },
                { "cyber security awareness", "★★★ (Medium)" }
            };

            return importanceLevels.TryGetValue(topic, out var importance)
                ? importance
                : "★★ (Important)";
        }
        #endregion

        #region Core Logic
        private static string BuildResponse(string input, string topic, string sentiment)
        {
            // Handle follow-up questions first
            if (IsFollowUpQuestion(input) && !string.IsNullOrEmpty(_lastTopic))
            {
                return _sentimentProfiles.TryGetValue(_lastSentiment, out var profile)
                    ? profile.ExplanationProvider(_lastTopic)
                    : GetDetailedExplanation(_lastTopic);
            }

            // Handle sentiment responses
            if (!string.IsNullOrEmpty(sentiment) && !string.IsNullOrEmpty(topic))
            {
                if (_sentimentProfiles.TryGetValue(sentiment, out var profile))
                {
                    string response = profile.Responses[_random.Next(profile.Responses.Length)];
                    return string.Format(response, topic) + "\n\n" + GetTopicResponse(topic);
                }
            }

            // Standard topic response
            if (!string.IsNullOrEmpty(topic))
                return GetTopicResponse(topic);

            // Fallback
            return GetDefaultResponse();
        }

        private static string GetTopicResponse(string topic)
        {
            string definition = _definitions.TryGetValue(topic, out string def)
                ? def
                : $"Let me explain {topic}...";

            string tip = _random.Next(3) == 0  // 33% chance to include tip
                ? $"\n\nQuick Tip: {GetRandomTip(topic)}"
                : "";

            return definition + tip;
        }
        #endregion

        #region Explanation Providers
        public static string GetSimplifiedExplanation(string topic)
        {
            var explanations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "phishing", "1. Fake messages try to trick you\n2. Never share passwords via links\n3. When unsure, contact the company directly" },
                { "privacy", "1. Control who sees your info\n2. Use strong unique passwords\n3. Check app permissions regularly" }
            };
            return explanations.TryGetValue(topic, out string explanation)
                ? explanation
                : $"Simple {topic} steps:\n1. Basic protection\n2. Regular checks\n3. Verified tools";
        }

        public static string GetReassuringExplanation(string topic)
        {
            var explanations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "phishing", "You're being smart by learning! Remember:\n✓ Most attacks are avoidable\n✓ Companies never ask for passwords via email\n✓ You can always verify suspicious messages" },
                { "privacy", "Good news! You can significantly improve your privacy by:\n✓ Updating settings monthly\n✓ Using a password manager\n✓ Being selective with app permissions" }
            };
            return explanations.TryGetValue(topic, out string explanation)
                ? explanation
                : $"You're taking the right steps with {topic}!\n✓ Basic protections\n✓ Regular maintenance\n✓ Staying informed";
        }
        #endregion

        #region Helper Methods
        private static void UpdateConversationHistory(string input)
        {
            if (_conversationHistory.Count >= MaxHistory)
                _conversationHistory.Dequeue();
            _conversationHistory.Enqueue(input);
        }

        public static string DetectTopic(string input) =>
            _definitions.Keys.FirstOrDefault(t => input.Contains(t));

        public static string DetectSentiment(string input) =>
            _sentimentProfiles.FirstOrDefault(p => p.Value.Triggers.Any(t => input.Contains(t))).Key;

        private static bool IsFollowUpQuestion(string input) =>
            input.Contains("?") || input.Contains("explain") || input.Contains("more");

        private static string GetDefaultResponse()
        {
            string[] defaults = {
                "Could you clarify your cybersecurity question?",
                "I specialize in topics like phishing and privacy. Ask away!",
                "Let me help with online safety. What would you like to know?"
            };
            return defaults[_random.Next(defaults.Length)];
        }
        #endregion

        private record SentimentProfile(
            string[] Triggers,
            string[] Responses,
            ConsoleColor Color,
            Func<string, string> ExplanationProvider);
    }
}