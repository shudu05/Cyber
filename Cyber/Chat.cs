
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Recording
{
    internal class Chat
    {
        private readonly Random random = new Random();

        private string name;
        private string currentTopic = "";

        private readonly Dictionary<string, string[]> tips =
            new Dictionary<string, string[]>
        {
            {
                "passwords",
                new string[]
                {
                    "Use a long, unique password for every account.",
                    "Never share your passwords with other people.",
                    "Use a password manager to help protect your passwords."
                }
            },
            {
                "phishing",
                new string[]
                {
                    "Check who sent an email before clicking links.",
                    "Never share personal information through suspicious links.",
                    "Look for spelling mistakes and unusual requests in emails."
                }
            },
            {
                "safe browsing",
                new string[]
                {
                    "Avoid suspicious websites and downloads.",
                    "Check a website address before signing in.",
                    "Keep your browser updated."
                }
            },
            {
                "privacy",
                new string[]
                {
                    "Avoid sharing your home address online.",
                    "Review your social media privacy settings.",
                    "Think carefully before posting personal information."
                }
            },
            {
                "scams",
                new string[]
                {
                    "Be careful of messages promising prizes or easy money.",
                    "Never send money because of a suspicious message.",
                    "Verify a request before sharing information or making payments."
                }
            },
            {
                "malware",
                new string[]
                {
                    "Only download files from trusted sources.",
                    "Keep your device and security software updated.",
                    "Avoid opening unexpected attachments."
                }
            }
        };

        public Chat(string userName)
        {
            name = string.IsNullOrWhiteSpace(userName)
                ? "Guest"
                : userName.Trim();
        }

        public string GetResponse(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return "Please type a question so I can help you.";
            }

            // Convert message to lowercase and remove punctuation.
            question = Regex.Replace(
                question.Trim().ToLowerInvariant(),
                @"[^\w\s]",
                " ");

            question = Regex.Replace(question, @"\s+", " ").Trim();

            // Greetings - check whole words instead of substrings.
            if (IsGreeting(question))
            {
                return "Hello, " + name +
                    "! Welcome back. What would you like to learn about today?";
            }

            // Ask how the bot is doing.
            if (question.Contains("how are you"))
            {
                return "I'm doing great, thank you for asking! " +
                    "How can I help you with cybersecurity today?";
            }

            // Remember the user's name.
            if (question.StartsWith("my name is "))
            {
                string newName = question.Substring(11).Trim();

                if (newName.Length > 0)
                {
                    name = newName;

                    return "Nice to meet you, " + name +
                        "! I'll use your name during our conversation.";
                }

                return "Please tell me your name after saying my name is.";
            }

            // Goodbye.
            if (ContainsAny(question,
                "goodbye", "bye", "see you later"))
            {
                return "Goodbye, " + name +
                    "! Remember to stay safe online.";
            }

            // Thank you.
            if (ContainsAny(question, "thank you", "thanks"))
            {
                return "You're welcome, " + name +
                    "! I'm happy to help you learn about cybersecurity.";
            }

            // Help command.
            if (ContainsAny(question, "help", "what can you do"))
            {
                return "I can help you learn about passwords, phishing, " +
                    "safe browsing, privacy, scams and malware. " +
                    "You can also ask for another tip or request more information.";
            }

            // Identify the topic before checking emotions.
            string foundTopic = FindTopic(question);

            // Recognise worried emotions.
            if (ContainsAny(question,
                "worried", "scared", "afraid", "anxious", "nervous"))
            {
                if (foundTopic != "")
                {
                    currentTopic = foundTopic;

                    return "It's understandable to feel concerned about " +
                        foundTopic + ". Let's take it one step at a time. " +
                        GetTip(foundTopic);
                }

                if (currentTopic != "")
                {
                    return "I understand that this can feel worrying. " +
                        "Let's work through it together. " +
                        GetTip(currentTopic);
                }

                return "I understand that online safety can feel worrying. " +
                    "You can ask me about passwords, phishing, privacy or scams.";
            }

            // Recognise frustration.
            if (ContainsAny(question,
                "frustrated", "annoyed", "confused", "difficult"))
            {
                if (foundTopic != "")
                {
                    currentTopic = foundTopic;
                }

                if (currentTopic != "")
                {
                    return "I understand that " + currentTopic +
                        " can seem confusing. Let's simplify it. " +
                        GetTip(currentTopic);
                }

                return "I understand this can be frustrating. " +
                    "Take your time. We can work through it one step at a time.";
            }

            // Recognise curiosity.
            if (ContainsAny(question,
                "curious", "interested", "excited"))
            {
                if (foundTopic != "")
                {
                    currentTopic = foundTopic;

                    return "It's great that you're interested in " +
                        foundTopic + "! Here's a tip: " + GetTip(foundTopic);
                }

                return "It's great that you're interested in learning! " +
                    "Which cybersecurity topic would you like to explore?";
            }

            // Follow-up: another tip.
            if (ContainsAny(question,
                "another tip", "more tips", "give me another",
                "different tip", "one more tip"))
            {
                if (currentTopic != "")
                {
                    return "Here's another tip about " + currentTopic +
                        ": " + GetTip(currentTopic);
                }

                return "Choose a topic first: passwords, phishing, " +
                    "safe browsing, privacy, scams or malware.";
            }

            // Follow-up: explain more.
            if (ContainsAny(question,
                "explain more", "tell me more", "more information",
                "elaborate", "explain further"))
            {
                if (currentTopic != "")
                {
                    return "Let's explore " + currentTopic +
                        " a little more. " + GetTip(currentTopic);
                }

                return "Which topic would you like me to explain? " +
                    "Try passwords, phishing, safe browsing, privacy, scams or malware.";
            }

            // Purpose and identity.
            if (ContainsAny(question,
                "your purpose", "what do you do",
                "who are you", "your name"))
            {
                return "I'm your Cybersecurity Awareness Bot! " +
                    "My purpose is to help you understand online risks " +
                    "and learn how to protect yourself.";
            }

            // Definition of cybersecurity.
            if (ContainsAny(question,
                "what is cybersecurity", "define cybersecurity",
                "meaning of cybersecurity"))
            {
                return "Cybersecurity means protecting devices, networks " +
                    "and information from digital attacks, damage or unauthorised access.";
            }

            // Respond to recognised cybersecurity topics.
            if (foundTopic != "")
            {
                currentTopic = foundTopic;

                return "Let's talk about " + foundTopic + ": " +
                    GetTip(foundTopic);
            }

            // Unknown messages.
            return "I'm not sure I understand that yet, " + name + ". " +
                "Try asking me about passwords, phishing, safe browsing, " +
                "privacy, scams or malware.";
        }

        // Check greetings without matching parts of other words.
        private bool IsGreeting(string message)
        {
            return message == "hi" ||
                   message == "hello" ||
                   message == "hey" ||
                   message.StartsWith("hi ") ||
                   message.StartsWith("hello ") ||
                   message.StartsWith("hey ") ||
                   message.StartsWith("good morning") ||
                   message.StartsWith("good afternoon") ||
                   message.StartsWith("good evening");
        }

        // Check whether a message contains any supplied phrases.
        private bool ContainsAny(string message, params string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                if (Regex.IsMatch(
                    message,
                    @"(?<!\w)" + Regex.Escape(phrase) + @"(?!\w)"))
                {
                    return true;
                }
            }

            return false;
        }

        // Identify the topic from different keywords and phrases.
        private string FindTopic(string question)
        {
            if (ContainsAny(question,
                "password", "passcode", "login security",
                "strong password", "account password"))
            {
                return "passwords";
            }

            if (ContainsAny(question,
                "phishing", "suspicious email", "fake email",
                "email scam", "fake message"))
            {
                return "phishing";
            }

            if (ContainsAny(question,
                "browsing", "website", "browser",
                "online shopping", "safe website"))
            {
                return "safe browsing";
            }

            if (ContainsAny(question,
                "privacy", "personal information",
                "private information", "data protection"))
            {
                return "privacy";
            }

            if (ContainsAny(question,
                "scam", "fraud", "fake prize",
                "online fraud", "scammers"))
            {
                return "scams";
            }

            if (ContainsAny(question,
                "malware", "virus", "computer infection",
                "malicious software"))
            {
                return "malware";
            }

            return "";
        }

        // Return a random tip for the selected topic.
        private string GetTip(string topic)
        {
            if (!tips.ContainsKey(topic))
            {
                return "Please choose a cybersecurity topic I recognise.";
            }

            string[] topicTips = tips[topic];

            int number = random.Next(topicTips.Length);

            return topicTips[number];
        }
    }
}
