using System;
using System.Collections.Generic;
using Core.Network;
using Core.Player;
using Mirror;
using TMPro;
using UnityEngine;

namespace Core.Scoring
{
    public class ScoreManager : MonoBehaviour
    {
        public ConnectionHandler connectionHandler;
        public TMP_Text mainScoreDisplay;

        public List<PlayerInfo> players = new List<PlayerInfo>();
        

        [Client]
        private void Update()
        {
            if (connectionHandler.myPlayer == null) return;
            mainScoreDisplay.text = connectionHandler.myPlayer.score.ToString();
        }
    }
}
