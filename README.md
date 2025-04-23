# POE_Part1_Chatbot

GitHub Repository Link:

https://github.com/SekwenaBotshelo/PROG6221_POE_Part1_ST10445866_Botshelo-Koketso-Sekwena.git 

Referancing:

1.) Andrew Troelsen, Phil Japikse. (2022). Pro C# 10 with .NET 6 - 
	Foundational Principles and Practices in Programming .
	Bhambersburg, PA, USA - West Chester, OH, USA: Apress.

2.) Farrell, J. (2022). JAVA Programming. Tenth ed. 200 Pier 4 Boulevard Boston,
	MA 02210 USA: Cengage 

3.) OpenAI. “ChatGPT.” Chatgpt.com, OpenAI, 2025, chatgpt.com.

Project Overview:

The chatbot asks for the user's name to personalise discussions, plays a welcome audio greeting using NAudio,
and displays an ASCII art logo for branding. Clear error messages inform the user if input is incorrect, and
responses are displayed with a typing animation. During runtime, stability is guaranteed by a global error handler.

Technical Highlights:

The C# code used to build this project is clear and modular. For convenience, logic is separated into several classes:

1.) Audio playback is handled by VoicePlayer.
2.) In AsciiArtRenderer, the logo is displayed.
3.) Animations and formatting are controlled by ConsoleUI.
4.) The chat loop is operated by a chatbot.
5.) Topic-based responses are produced using ResponseManager.

In Summary:

An instructional chatbot that mimics actual cybersecurity situations and provides useful advice to help people stay 
safe online. Fun design features like sound, graphics, and animations elevate it above a simple text Q&A.