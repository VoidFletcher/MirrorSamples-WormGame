using Mirror;
using UnityEngine;

namespace Core.Network
{
    public static class ChatService
    {
        public struct ScoreMessage : NetworkMessage
        {
            public string playerName;
            public string playerMessage;
        }
        
        
    }
}
