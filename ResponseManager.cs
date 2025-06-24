using System;
using System.Collections.Generic;
using System.Linq;

namespace POE_Part1_Chatbot.Core
{
    /// Static response generator with predefined answers for cybersecurity topics.
    /// Now includes enhanced memory and recall capabilities.
    internal class ResponseManager
    {
        // Static random generator
        private static readonly Random _random = new Random();

        // Context tracker
        private static string _lastTopic = string.Empty;
        private static string _lastDetailedExplanation = string.Empty;

        // Definitions Dictionary
        private static readonly Dictionary<string, string> _definitions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "purpose", "The purpose of this chatbot is to raise awareness about online safety and help you recognize potential cyber threats." },
            { "cyber security awareness", "Understanding online risks such as malware, phishing, hacking, and scams, as well as how to safeguard your devices, personal data, and yourself." },
            { "phishing", "Phishing is the practice of obtaining personal information through fraudulent communications that appear to come from trustworthy sources." },
            { "password safety", "Using strong, unique passwords combined with other security measures to prevent unauthorized access to accounts and sensitive data." },
            { "safe browsing", "Practicing safe habits while using the internet to protect against online threats like malware and malicious websites." },
            { "password", "A secret string of characters used to verify identity and secure access to systems and data." },
            { "scam", "A dishonest scheme conducted online aimed at tricking individuals into giving away personal information or money." },
            { "privacy", "The right and practice of protecting personal and sensitive information from unauthorized access or disclosure." }
        };

        // Detailed explanations for each topic
        private static readonly Dictionary<string, string> _detailedExplanations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "privacy", "Privacy involves three key aspects:\n1. Controlling who can access your personal data\n2. Understanding how organizations use your information\n3. Managing your digital footprint across platforms\n\nGood privacy practices include reviewing app permissions, using privacy-focused browsers, and being mindful of what you share online." },
            { "phishing", "Phishing attacks typically involve:\n1. Fake communications appearing to be from trusted sources\n2. Urgent requests for personal information\n3. Links to fraudulent websites\n\nAdvanced phishing may use:\n- Spoofed email addresses\n- Fake login pages\n- Social engineering tactics" },
            { "password safety", "Creating strong passwords involves:\n1. Using 12+ characters with mixed cases\n2. Including numbers and special symbols\n3. Avoiding personal information\n4. Using unique passwords for each account\n\nConsider using a password manager to generate and store complex passwords securely." }
        };

        // Topic importance explanations
        private static readonly Dictionary<string, string> _topicImportance = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "privacy", "protecting your personal information helps prevent identity theft and maintains your digital reputation." },
            { "phishing", "recognizing phishing attempts can prevent financial losses and account compromises." },
            { "password safety", "strong passwords are your first line of defense against unauthorized access to your accounts." },
            { "safe browsing", "safe browsing habits protect you from malware and scams while maintaining your online security." }
        };

        // Related information for each topic
        private static readonly Dictionary<string, string> _relatedInformation = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "privacy", "You might also want to:\n1. Review social media privacy settings\n2. Check app permissions on your devices\n3. Consider using a VPN for public Wi-Fi\n4. Explore privacy-focused alternatives to common services" },
            { "phishing", "Related concerns include:\n1. Smishing (SMS phishing)\n2. Vishing (voice call phishing)\n3. Social media scams\n4. Business email compromise attacks" }
        };

        // Tips Dictionary (unchanged from original)
        private static readonly Dictionary<string, List<string>> _topicTips = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "phishing tips", new List<string> { /* ... existing tips ... */ } },
            { "password tips", new List<string> { /* ... existing tips ... */ } },
            { "safe browsing tips", new List<string> { /* ... existing tips ... */ } },
            { "scam tips", new List<string> { /* ... existing tips ... */ } },
            { "privacy tips", new List<string> { /* ... existing tips ... */ } }
        };

        // Sentiment analysis and empathetic responses
        private static readonly Dictionary<string, List<string>> _sentimentKeywords = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried", new List<string> { "worried", "anxious", "nervous", "concerned" } },
            { "frustrated", new List<string> { "frustrated", "angry", "annoyed", "upset" } },
            { "confused", new List<string> { "confused", "unsure", "don't understand", "puzzled" } },
            { "curious", new List<string> { "curious", "interested", "wondering", "tell me more" } }
        };

        private static readonly Dictionary<string, string> _empatheticResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "worried", "It's completely understandable to feel that way about {0}. Let me help clarify..." },
            { "frustrated", "I hear your frustration about {0}. Cybersecurity can be challenging..." },
            { "confused", "I'll explain {0} in simpler terms. The key points are..." },
            { "curious", "That's a great question about {0}! Here's what you should know..." }
        };

        // Default responses
        private static readonly List<string> _defaultResponses = new List<string>
        {
            "I'm not sure I understand. Could you rephrase that?",
            "I specialize in cybersecurity topics. Could you ask about things like phishing or password safety?",
            "Let me try to help - could you rephrase your question about online safety?"
        };

        // Public methods for the enhanced Chatbot class
        public static string GetDetailedExplanation(string topic)
        {
            if (_detailedExplanations.TryGetValue(topic, out string explanation))
            {
                _lastDetailedExplanation = explanation;
                return explanation;
            }
            return "Let me explain this in more detail...";
        }

        public static string GetTopicImportance(string topic)
        {
            return _topicImportance.TryGetValue(topic, out string importance) ?
                   importance : "this is a crucial cybersecurity topic.";
        }

        public static string GetRelatedInformation(string topic)
        {
            return _relatedInformation.TryGetValue(topic, out string info) ?
                   info : "Here's some additional information you might find useful...";
        }

        public static List<string> GetAllTopics()
        {
            return _definitions.Keys.Concat(_topicTips.Keys.Select(k => k.Replace(" tips", ""))).Distinct().ToList();
        }

        public static string GetRandomTip(string topic)
        {
            if (_topicTips.TryGetValue(topic, out var tips) && tips?.Count > 0)
            {
                return tips[_random.Next(tips.Count)];
            }
            return "I don't have tips on that specific topic right now.";
        }

        // Main response generation method (updated)
        public static string GetResponse(string input, string userName)
        {
            input = input.ToLower();
            string detectedSentiment = DetectSentiment(input);
            string detectedTopic = DetectTopic(input);

            // Handle sentiment responses
            if (!string.IsNullOrEmpty(detectedSentiment) && !string.IsNullOrEmpty(detectedTopic))
            {
                _lastTopic = detectedTopic;
                return string.Format(_empatheticResponses[detectedSentiment], detectedTopic) +
                       " " + GetDetailedExplanation(detectedTopic);
            }

            // Handle follow-up questions
            if (input.Contains("more") || input.Contains("explain") || input.Contains("what do you mean"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    return GetDetailedExplanation(_lastTopic);
                }
                return "Could you clarify what you'd like me to explain further?";
            }

            // Handle definition requests
            foreach (var keyword in _definitions.Keys)
            {
                if (input.Contains(keyword) || input.Contains("what is " + keyword) || input.Contains("define " + keyword))
                {
                    _lastTopic = keyword;
                    return _definitions[keyword];
                }
            }

            // Handle tip requests
            foreach (var topic in _topicTips.Keys)
            {
                if (input.Contains(topic))
                {
                    _lastTopic = topic.Replace(" tips", "");
                    return GetRandomTip(topic);
                }
            }

            // Default responses
            return _defaultResponses[_random.Next(_defaultResponses.Count)];
        }

        private static string DetectSentiment(string input)
        {
            foreach (var sentiment in _sentimentKeywords)
            {
                if (sentiment.Value.Any(keyword => input.Contains(keyword)))
                {
                    return sentiment.Key;
                }
            }
            return null;
        }

        private static string DetectTopic(string input)
        {
            foreach (var topic in GetAllTopics())
            {
                if (input.Contains(topic))
                {
                    return topic;
                }
            }
            return null;
        }
    }
}