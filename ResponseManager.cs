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
        // Context tracker to remember the last discussed topic
        private static string _lastTopic = string.Empty;

        //Definitions Dictionary
        private static readonly Dictionary<string, string> _definitions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "purpose", "The purpose of this chatbot is to raise awareness about online safety and help you recognize potential cyber threats." },
            { "cyber security awareness", "It can be described as understanding online risks such as malware, phishing, hacking, and scams, as well as how to safeguard your devices, personal data, and yourself, is known as cybersecurity awareness. To prevent falling victim to cybercrime, it entails developing safe online practices, identifying possible threats, and maintaining vigilance." },
            { "phishing", "Phishing is the practice of obtaining personal information, such as passwords, bank account details, or credit card numbers, from websites that pose as trustworthy in an effort to steal your money or identity." },
            { "password safety", "By employing strong, secure passwords in conjunction with other security policies and technologies, password safety refers to the procedures and safeguards put in place to prevent unauthorised access to user accounts and sensitive data. It is a crucial component of cybersecurity since passwords serve as the first layer of protection against data breaches and cyberattacks. " },
            { "safe browsing", "Taking safety measures to safeguard oneself from online dangers while using the internet is known as \"safe browsing.\" This involves identifying and avoiding malware, phishing scams, and harmful websites utilising tools and techniques. \r\n" },
            { "password", "A password is a secret string of characters used to verify a user's identity and secure access to systems and data. Strong passwords are essential to prevent unauthorized access. If you need to any tips to strengthen you password, you can ask me ;>\r\n" },
            { "scam", "A scam is a dishonest scheme or fraud, often conducted online, aimed at tricking individuals into giving away personal information or money. If you don't seem to fully grasp the concept, then you may ask me on tips to detect, prevent, and mitigate scams." },
            { "privacy", "Privacy in cybersecurity refers to the right and practice of protecting personal and sensitive information from unauthorized access, use, or disclosure. To mitigate any possible infringements upon your privacy, you may ask me on any possible tips that you may need on this topic." }
        };

        //Tips Dictionary
        private static readonly Dictionary<string, List<string>> _topicTips = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "phishing tips", new List<string>
                {
                    "Always hover over links to verify the URL before clicking.",
                    "Never download attachments from unknown or suspicious emails.",
                    "Phishing emails often create urgency—don’t rush to respond without verifying.",
                    "Enable two-factor authentication to protect your accounts.",
                    "Be wary of grammatical errors or unfamiliar greetings in emails.",
                    "Don’t enter personal info on forms linked in unexpected emails.",
                    "Watch for domain impersonation—‘paypa1.com’ instead of ‘paypal.com’.",
                    "Use spam filters to reduce phishing emails from reaching your inbox.",
                    "Report phishing emails to your IT department or provider.",
                    "Trust your instincts—if something feels off, investigate before acting."
                }
            },
            {
                "password tips", new List<string>
                {
                    "Use a unique password for each account you own.",
                    "Combine uppercase, lowercase, numbers, and symbols in your passwords.",
                    "Avoid using personal information like birthdays or names.",
                    "Update your passwords regularly and avoid reusing old ones.",
                    "Use a password manager to store and generate strong passwords securely.",
                    "Don’t write your passwords down on paper or unencrypted files.",
                    "Enable two-factor authentication for sensitive accounts.",
                    "Avoid auto-saving passwords on shared/public computers.",
                    "Change default passwords on routers and devices immediately.",
                    "Test your password strength using trusted security tools."
                }
            },
            {
                "safe browsing tips", new List<string>
                {
                    "Only enter personal information on websites that use HTTPS.",
                    "Avoid clicking pop-ups or advertisements from unknown sources.",
                    "Keep your browser and antivirus software up to date.",
                    "Do not download files from untrusted websites.",
                    "Log out of accounts after use, especially on public devices.",
                    "Use incognito mode for private searches and sessions.",
                    "Check the browser address bar for signs of phishing (e.g., misleading URLs).",
                    "Install reputable browser extensions that block trackers and malware.",
                    "Clear cookies and cache regularly to protect privacy.",
                    "Avoid using outdated plugins like Flash or Java in browsers."
                }
            },
            {
                "scam tips", new List<string>
                {
                    "If an offer sounds too good to be true, it probably is.",
                    "Never share personal or financial information over email or phone unless you're sure of the source.",
                    "Verify the identity of anyone asking for money or private data.",
                    "Scammers often impersonate trusted brands—double-check their website or call them directly.",
                    "Use reverse image search to check the authenticity of suspicious social media profiles or ads.",
                    "Be cautious of unsolicited prize notifications or job offers.",
                    "Read reviews before buying from unfamiliar online stores.",
                    "Never pay using gift cards or cryptocurrency to unknown parties.",
                    "Report scams to the South African Cybercrime Hub or relevant authority.",
                    "Educate others, especially elderly family, about common scam tactics."
                }
            },
            {
                "privacy tips", new List<string>
                {
                    "Review app permissions regularly and revoke access that isn’t necessary.",
                    "Use privacy-focused browsers and search engines to minimize tracking.",
                    "Adjust your social media privacy settings to control who sees your data.",
                    "Be mindful of what you share online, especially personal and location-based information.",
                    "Use VPNs on public Wi-Fi to encrypt your connection and protect your data.",
                    "Avoid using real names or contact details on public forums or gaming platforms.",
                    "Disable location tracking for apps that don’t need it.",
                    "Encrypt your personal files and sensitive data when stored digitally.",
                    "Opt-out of data-sharing options in apps and online services.",
                    "Check if your email or credentials have been exposed on websites like haveibeenpwned.com."
                }
            }
        };

        public static string GetResponse(string input, string userName)
        {
            input = input.Trim().ToLower();

            // --- Contextual follow-up handling ---
            if (input.Contains("more") ||
                input.Contains("explain") ||
                input.Contains("what do you mean") ||
                input.Contains("clarify"))
            {
                if (!string.IsNullOrEmpty(_lastTopic) && _definitions.ContainsKey(_lastTopic))
                {
                    return $"Sure! Here's more detail on {_lastTopic}: {_definitions[_lastTopic]}";
                }
                else
                {
                    return "Can you clarify what you'd like to know more about?";
                }
            }

            // Appreciation recognition
            if (input.Contains("thank you") || input.Contains("thanks") ||
                input.Contains("appreciate") || input.Contains("grateful") ||
                input.Contains("thankful") || input.Contains("learnt") || input.Contains("learned"))
            {
                return "You're welcome! I'm glad I could help. If you have more questions about cybersecurity, feel free to ask!";
            }

            // --- Basic questions ---
            switch (input)
            {
                case "how are you":
                    return "I'm just a bot, but I'm functioning perfectly! Thanks for asking.";

                case "what is your purpose":
                    _lastTopic = "purpose";
                    return _definitions["purpose"];

                case "what can i ask you about":
                    return "You can ask me for definitions of terms like phishing, password safety, or privacy, or request tips by typing something like 'phishing tips' or 'privacy tips'.";

                case "what tips can i ask about":
                    return "You can ask for tips on phishing, password safety, safe browsing, scams, or privacy. Just type a phrase like 'password tips'.";
            }

            // --- Tip Matching ---
            foreach (var topic in _topicTips.Keys)
            {
                if (input.Contains(topic))
                {
                    _lastTopic = topic.Replace(" tips", ""); // Update context without "tips"
                    var tips = _topicTips[topic];
                    Random rand = new Random();
                    int index = rand.Next(tips.Count);
                    return tips[index];
                }
            }

            // --- Definition Matching ---
            foreach (var keyword in _definitions.Keys)
            {
                if (input.Contains(keyword))
                {
                    _lastTopic = keyword;
                    return _definitions[keyword];
                }
            }

            // --- Default fallback ---
            return $"Sorry {userName}, I didn’t quite understand that. You can ask me about cybersecurity topics or request tips!";
        }
    }
}
