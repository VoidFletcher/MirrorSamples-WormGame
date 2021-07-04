using System;
using UnityEngine;

namespace Core.Player
{
    public class DeathChecker : MonoBehaviour
    {
        public PlayerInfo playerInfo;
        public PlayerDeathHandler playerDeathHandler;

        private void Update()
        {
            if (!playerInfo.IsServer()) return;
            
            // The following logic will only execute on the server, since we want the server to handle checking for player
            // deaths.
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, transform.forward, out hit, 0.02f)) return;
            if (!hit.collider.GetComponent<DeathComponent>()) return;
            var hitPlayerInfo = hit.collider.GetComponentInParent<PlayerInfo>();
            if (hitPlayerInfo == null)
            {
                Debug.Log($"{playerInfo.playerName} hit a non-worm death object.");
                playerDeathHandler.Die();
                return;
            }
        
            if (hitPlayerInfo != playerInfo)
            {
                Debug.Log($"{playerInfo.playerName} hit a different worm.");
                playerDeathHandler.Die();
                return;
            }
        }
    }
}
