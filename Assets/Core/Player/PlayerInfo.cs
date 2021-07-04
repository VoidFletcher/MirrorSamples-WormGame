using Core.Scoring;
using Mirror;
using UnityEngine;

namespace Core.Player
{
    public class PlayerInfo : NetworkBehaviour
    {
        [SyncVar]
        public string playerName;

        [SyncVar]
        public int score;
        
        [SyncVar]
        public float baseSpeed;
        [SyncVar]
        public float speedModifier = 1f;
        [SyncVar]
        public float collectionRange = 3f;
        [SyncVar]
        public float turningAngleRadians = 240f;

        public bool HasAuthority() => hasAuthority;
        public bool IsServer() => _isServer;

        [Header("Script References")]
        public PlayerMovement playerMovement;
        public CameraMovement cameraMovement;
        public PlayerBody playerBody;
        public GameObject cameraContainer;
        private bool _isServer;

        public override void OnStartServer()
        {
            base.OnStartServer();
            _isServer = true;
        }
    }
}
