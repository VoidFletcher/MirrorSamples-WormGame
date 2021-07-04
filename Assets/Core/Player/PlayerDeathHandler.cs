using System;
using System.Linq;
using Core.Scoring;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Core.Player
{
    public class PlayerDeathHandler : NetworkBehaviour
    {
        public PlayerInfo playerInfo;
        
        /// <summary>
        /// The prefab that we instantiate and spawn for other worms to eat.
        /// </summary>
        public NetworkIdentity playerPointPartPrefab;
        
        [Server]
        public void Die()
        {
            for (int i = 0; i < playerInfo.playerBody.totalSegments; i++)
            {
                for (int j = 0; j < Mathf.RoundToInt(playerInfo.playerBody.dropsPerSegment / 2); j++)
                {
                    Debug.Log("Server instantiating piece.");
                    var piece = Instantiate(playerPointPartPrefab);
                    var point = piece.GetComponent<Point>();
                    point.value = Mathf.RoundToInt(playerInfo.playerBody.pointsPerSegment / playerInfo.playerBody.dropsPerSegment / 2);
                    var segmentPosition = playerInfo.playerBody.segments[i].transform.position;
                    piece.transform.position = new Vector3(Random.Range(segmentPosition.x + 0.5f, segmentPosition.x - 0.5f),
                        0,
                        Random.Range(segmentPosition.z + 0.5f, segmentPosition.z - 0.5f));
                    NetworkServer.Spawn(piece.gameObject);
                    point.spriteId = 0;
                }
            }

            playerInfo.score = 0;

            HandleDeath();
        }

        [ClientRpc]
        public void HandleDeath()
        {
            // for (int i = 0; i < playerInfo.playerBody.segments.Count; i++)
            // {
            //     if (i == 0) continue;
            //     var segment = playerInfo.playerBody.segments[i];
            //     Destroy(segment.gameObject);
            //     playerInfo.playerBody.segments.RemoveAt(i);
            // }

            var targets = FindObjectsOfType<NetworkStartPosition>();
            var targetLocation = targets[Random.Range(0, targets.Length)].transform;
            playerInfo.playerBody.head.SetPositionAndRotation(targetLocation.position, targetLocation.rotation);

            for (int i = 0; i < playerInfo.playerBody.segments.Count; i++)
            {
                if (i == 0) continue;
                var headTransform = playerInfo.playerBody.head;
                playerInfo.playerBody.segments[i].transform.SetPositionAndRotation(headTransform.position, headTransform.rotation);
            }
        }
    }
}
