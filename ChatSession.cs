using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POE_Part1_Chatbot
{
    public abstract class ChatSession
    {
        public abstract void StartSession(); // Must be implemented by derived class
    }

}
