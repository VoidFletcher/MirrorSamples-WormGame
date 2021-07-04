using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Mirror;
using UnityEngine;

namespace Core.Network
{
    public class ConnectionHandler : NetworkBehaviour
    {
        public LogLevel logLevel = LogLevel.Info;
        public SyncList<NetworkIdentity> players = new SyncList<NetworkIdentity>();
        public PlayerInfo myPlayer;

        public enum LogLevel
        {
            Verbose = 3,
            Info = 2,
            Warning = 1,
            Error = 0
        }
        public override void OnStartServer()
        {
            base.OnStartServer();
            AdvancedNetworkManager.instance.onServerConnect.AddListener(PlayerConnected);
            AdvancedNetworkManager.instance.onServerAddPlayer.AddListener(PlayerInitialized);
            AdvancedNetworkManager.instance.onServerPlayerDisconnect.AddListener(PlayerDisconnected);
        }

        [Server]
        private void PlayerConnected(NetworkConnection connection)
        {
            
            
        }

        [Server]
        private void PlayerInitialized(NetworkConnection connection)
        {
            if (logLevel <= LogLevel.Verbose) Debug.Log($"[Connection Handler] New client initialized: {connection.address}");
            foreach (var playerInfo in players.Select(player => player.GetComponent<PlayerInfo>()))
            {
                playerInfo.playerBody.UpdateLocations();
                playerInfo.playerBody.TargetSynchronize(connection);
            }
            
            players.Add(connection.identity);
            TargetSetLocalPlayer(connection);
        }

        private void PlayerDisconnected(NetworkConnection connection)
        {
            players.Remove(connection.identity);
        }

        [TargetRpc]
        private void TargetSetLocalPlayer(NetworkConnection connection)
        {
            myPlayer = connection.identity.GetComponent<PlayerInfo>();
        }
    }
}
