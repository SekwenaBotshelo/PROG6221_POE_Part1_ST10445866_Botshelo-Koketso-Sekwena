using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POE_Part1_Chatbot.Graphics
{
    // <summary>
    /// Handles display of ASCII art and colored console output.
    /// Uses multi-line strings for complex art layouts.
    /// </summary>
    internal class AsciiArtRenderer
    {
        /// <summary>
        /// Displays the chatbot's ASCII logo with cyan coloring.
        /// Uses verbatim strings (@) for multi-line formatting.
        /// </summary>
        public static void DisplayLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"                            ,--.--------.    ,----.       _,---.                ,---.                               
    _..---.  ,--.-.  .-,--./==/,  -   , -\,-.--` , \  _.='.'-,  \ .--.-. .-.-..--.'  \      .-.,.---.   _,..---._   
  .' .'.-. \/==/- / /=/_ / \==\.-.  - ,-./==|-  _.-` /==.'-     //==/ -|/=/  |\==\-/\ \    /==/  `   \/==/,   -  \  
 /==/- '=' /\==\, \/=/. /   `--`\==\- \  |==|   `.-./==/ -   .-' |==| ,||=| -|/==/-|_\ |  |==|-, .=., |==|   _   _\ 
 |==|-,   '  \==\  \/ -/         \==\_ \/==/_ ,    /|==|_   /_,-.|==|- | =/  |\==\,   - \ |==|   '='  /==|  .=.   | 
 |==|  .=. \  |==|  ,_/          |==|- ||==|    .-' |==|  , \_.' )==|,  \/ - |/==/ -   ,| |==|- ,   .'|==|,|   | -| 
 /==/- '=' ,| \==\-, /           |==|, ||==|_  ,`-._\==\-  ,    (|==|-   ,   /==/-  /\ - \|==|_  . ,'.|==|  '='   / 
|==|   -   /  /==/._/            /==/ -//==/ ,     / /==/ _  ,  //==/ , _  .'\==\ _.\=\.-'/==/  /\ ,  )==|-,   _`/  
`-._`.___,'   `--`-`             `--`--``--`-----``  `--`------' `--`..---'   `--`        `--`-`--`--'`-.`.____.'   
");
            Console.ResetColor(); // Reset to default console colors
        }
    }
}
