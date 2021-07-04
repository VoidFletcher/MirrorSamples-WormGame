using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Core.Player
{
    public class PlayerBody : NetworkBehaviour
    {
        public PlayerInfo playerInfo;
        public GameObject segmentPrefab;
        public Transform segmentParent;
        public Transform head;
        public float segmentDistance;
        public List<Segment> segments = new List<Segment>();

        [SyncVar] public bool pendingSynchronization = true;

        [SyncVar]
        public int startingSegments = 4;
        [SyncVar]
        public int totalSegments;
        [SyncVar] 
        public int pointsPerSegment;
        [SyncVar] 
        public int dropsPerSegment = 2;
        
        
        public readonly SyncDictionary<int, Vector3> segmentLocations = new SyncDictionary<int, Vector3>();
        public readonly SyncDictionary<int, Quaternion> segmentRotations = new SyncDictionary<int, Quaternion>();

        [Server]
        public void UpdateLocations()
        {
            pendingSynchronization = true;
            for (int i = 0; i < totalSegments; i++)
            {
                var segmentTransform = segments[i].transform;
                segmentLocations[i] = segmentTransform.position;
                segmentRotations[i] = segmentTransform.rotation;
            }
            pendingSynchronization = false;
        }

        
        private void Update()
        {
            if (playerInfo.IsServer())
            {
                totalSegments = startingSegments + Mathf.RoundToInt(playerInfo.score / pointsPerSegment);
            }

            if (segments.Count < totalSegments)
            {
                var segmentSpawn = Instantiate(segmentPrefab, segmentParent).GetComponent<Segment>();
                segmentSpawn.transform.position = segments[segments.Count - 1].transform.position;
                segments.Add(segmentSpawn);
            }

            if (segments.Count > totalSegments)
            {
                var lastSegment = segments[segments.Count - 1];
                Destroy(segments[segments.Count - 1].gameObject);
                segments.RemoveAt(segments.Count - 1);
                segments.Remove(lastSegment);
            }
        }

        [TargetRpc(channel = Channels.Reliable)]
        public void TargetSynchronize(NetworkConnection target)
        {
            Debug.Log($"Synchronizing data for player: {GetComponent<PlayerInfo>().name}");
            StartCoroutine(Synchronize());
        }

        private IEnumerator Synchronize()
        {
            var delay = new WaitForSeconds(0.05f);
            while (pendingSynchronization) yield return delay;
            
            for (var i = 0; i < segments.Count; i++)
            {
                Debug.Log($"Synchronizing segment data for index {i}.");
                segments[i].transform.position = segmentLocations[i];
                segments[i].transform.rotation = segmentRotations[i];
            }
            yield break;
        }
    }
}
