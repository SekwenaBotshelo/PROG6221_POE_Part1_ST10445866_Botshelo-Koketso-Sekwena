using System;
using POE_Part1_Chatbot.Graphics;

namespace POE_Part1_Chatbot
{
    internal class Program
    {
        
        /// Main entry point for the chatbot application.
        /// Uses async/await for non-blocking audio playback.
        /// </summary>
        static async Task Main(string[] args)
        {
            try
            {
                // Initialize audio file path (relative to executable)
                string filePath = Path.Combine("Assets", "welcome.wav");

                // Play greeting audio if file exists (fail silently if not)
                if (File.Exists(filePath))
                {
                    await VoicePlayer.PlayGreeting(filePath); // Async audio playback
                }
                else
                {
                    Console.WriteLine("Welcome audio not found. Starting silently...");
                }

                // Display ASCII art logo
                AsciiArtRenderer.DisplayLogo();

                // Initialize and start the chatbot
                Chatbot bot = new Chatbot();
                bot.Start();
            }
            catch (Exception ex)
            {
                // Global error handler for uncaught exceptions
                ConsoleUI.PrintError($"Fatal error: {ex.Message}");
                #if DEBUG

                Console.WriteLine($"DEBUG: {ex.StackTrace}"); // Show stack trace in debug mode
                #endif
            }
            Console.ReadKey();
        }
    }
}
