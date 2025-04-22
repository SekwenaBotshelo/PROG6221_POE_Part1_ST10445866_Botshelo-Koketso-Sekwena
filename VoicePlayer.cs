using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace POE_Part1_Chatbot
{
    /// <summary>
    /// Handles audio playback using NAudio library.
    /// Manages WAV file loading and streaming with error handling.
    /// </summary>
    internal class VoicePlayer
    {
        /// <summary>
        /// Asynchronously plays a WAV greeting audio file.
        /// </summary>
        /// <param name="filePath">Relative path to the WAV file</param>
        /// <returns>Task for async playback control</returns>
        public static async Task PlayGreeting(string filePath)
        {
            try
            {
                // Verify file exists before attempting playback
                if (!File.Exists(filePath))
                {
                    ConsoleUI.PrintError("⚠️ Audio file not found");
                    return;
                }

                // NAudio objects (auto-disposed via 'using' blocks)
                using (var audioFile = new AudioFileReader(filePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Play();

                    // Non-blocking wait for playback completion
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(100); // Reduce CPU usage
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle NAudio-specific exceptions (e.g., corrupt WAV files)
                ConsoleUI.PrintError($"Audio Error: {ex.Message}");
            }
        }
    }
}