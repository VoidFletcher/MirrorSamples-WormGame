using System;
using Core.Player;
using UnityEngine;

namespace Core.Scoring
{
    public class PointCollector : MonoBehaviour
    {
        public PlayerInfo playerInfo;
        public CapsuleCollider collectionArea;

        public void OnTriggerEnter(Collider other)
        {
            if (!playerInfo.isServer) return;
            var pointComponent = other.GetComponent<Point>();
            if (pointComponent == null) return;
            pointComponent.Tractor(this);
            collectionArea.radius = playerInfo.collectionRange;
        }
    }
}
