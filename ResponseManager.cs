using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POE_Part1_Chatbot.Core
{
    /// Static response generator with predefined answers for cybersecurity topics.
    /// Implements simple intent recognition via keyword matching.
    internal class ResponseManager
    {
        // ✅ Keyword-based fallback response dictionary
        private static readonly Dictionary<string, string> _keywordResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "purpose", "The purpose of this chatbot is to raise awareness about online safety and help you recognize potential cyber threats." },

            { "cyber security awareness", "It can be described as understanding online risks such as malware, phishing, hacking, and scams, as well as how to safeguard your devices, personal data, and yourself, is known as cybersecurity awareness. To prevent falling victim to cybercrime, it entails developing safe online practices, identifying possible threats, and maintaining vigilance." },

            { "phishing", "Phishing is a type of cyberattack in which hackers use phoney emails, texts, or websites to pose as reputable organisations, such banks or businesses, in an attempt to fool victims into disclosing personal information like passwords or bank account information. Verifying senders, avoiding unknown links, and utilising robust security measures like two-factor authentication are the best ways to keep safe when dealing with what are typically urgent or questionable requests." },

            { "password safety", "Password safety entails coming up with strong, one-of-a-kind passwords and keeping them safe to prevent hackers away from your personal data. Typically, a secure password is lengthy, consists of a combination of letters, numbers, and symbols, and stays away from information that could be guessed, such as names or dates of birth. For added security, you should turn on two-factor authentication and refrain from using the same password across accounts." },

            { "password", "Password safety entails coming up with strong, one-of-a-kind passwords and keeping them safe to prevent hackers away from your personal data. Typically, a secure password is lengthy, consists of a combination of letters, numbers, and symbols, and stays away from information that could be guessed, such as names or dates of birth. For added security, you should turn on two-factor authentication and refrain from using the same password across accounts.." },

            { "safe browsing", "Being cautious when using the internet to prevent viruses, scams, and cyberattacks is known as \\\"safe browsing.\\\" Checking for a secure connection (look for \\\"https://\\\" and a padlock icon), avoiding dubious links or pop-ups, keeping your browser and antivirus software up to date, and not disclosing personal information on unfamiliar or untrusted websites are all part of it..." },
            
            { "scam", "If something feels too good to be true online, it probably is. Never share personal info with unknown contacts." },

            { "privacy", "Always review app permissions and adjust privacy settings on your accounts to limit data sharing." }
        };

        /// Maps user input to appropriate responses using flexible keyword matching.

        public static string GetResponse(string input, string userName)
        {
            if (string.IsNullOrWhiteSpace(input))
                return $"Sorry {userName}, I didn’t catch that. Could you please say that again?";

            // Greeting
            if (input.Contains("hello") || input.Contains("hi") || input.Contains("hey"))
            {
                return $"Hello {userName}! How can I help you stay safe online today?";
            }

            // Exit keyword check (just in case someone types "exit" as input)
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) || input.Contains("goodbye"))
            {
                return $"Goodbye {userName}, and remember—stay cyber safe!";
            }

            // Search for any matching keyword in the dictionary
            foreach (var keyword in _keywordResponses.Keys)
            {
                if (input.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return _keywordResponses[keyword];
                }
            }

            // Fallback/default response
            return $"Sorry {userName}, I didn’t quite understand that. Try asking about phishing, password safety, or safe browsing.";
        }
    }
}
