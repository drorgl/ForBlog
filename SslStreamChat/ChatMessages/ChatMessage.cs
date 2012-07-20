using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatMessages
{
    /// <summary>
    /// Chat message container
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        public string Message { get; set; }
    }
}
