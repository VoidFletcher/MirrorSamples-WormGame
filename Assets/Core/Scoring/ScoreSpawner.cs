using System;
using System.Collections;
using System.Collections.Generic;
using Core.Player;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Scoring
{
    public class ScoreSpawner : NetworkBehaviour
    {
        public static ScoreSpawner instance;
        public int spawnTarget;
        public int spawnRate;
        
        public Point pointPrefab;
        public List<Color32> pointColors = new List<Color32>();
        public BoxCollider spawnRegion;
        public PointPool pointPool;
        

        [SyncVar]
        private int _activePoints;

        private Point _spawningPoint;
        private GameObject _spawningPointObject;
        private Vector3 _spawnerTarget = Vector3.zero;
        private Vector3 _spawnRegionPosition;
        private Vector3 _spawnRegionBounds;

        private bool _isActive;

        public override void OnStartServer()
        {
            if (instance == null) instance = this;
            if (instance != this) NetworkServer.Destroy(this.gameObject);
            _isActive = true;
        }

        private void FixedUpdate()
        {
            if (!_isActive) return;
            if (netIdentity.connectionToServer != null) return;
            for (var i = 0; i < spawnRate; i++)
            {
                if (_activePoints > spawnTarget) continue;
                
                _spawningPoint = pointPool.GetAvailablePooledPoint();
                _spawningPoint.transform.position = GetRandomValidPosition();
                _spawningPointObject = _spawningPoint.gameObject;
                _spawningPointObject.SetActive(true);
                _activePoints += _spawningPoint.value;
                NetworkServer.Spawn(_spawningPointObject);
            }
        }

        public Vector3 GetRandomValidPosition()
        {
            _spawnRegionPosition = spawnRegion.transform.position;
            _spawnRegionBounds = spawnRegion.bounds.extents;
            
            _spawnerTarget.x = Random.Range(_spawnRegionPosition.x - _spawnRegionBounds.x,
                _spawnRegionPosition.x + _spawnRegionBounds.x);
            
            _spawnerTarget.z = Random.Range(_spawnRegionPosition.z - _spawnRegionBounds.z,
                _spawnRegionPosition.z + _spawnRegionBounds.z);

            return _spawnerTarget;
        }

        public Point SpawnPoint()
        {
            var newPoint = Instantiate(pointPrefab, pointPool.transform);
            var colorIndex = Random.Range(0, pointColors.Count);
            
            newPoint.color = new Vector4(
                pointColors[colorIndex].r,
                pointColors[colorIndex].g,
                pointColors[colorIndex].b,
                pointColors[colorIndex].a);
            
            return newPoint;
        }

        public void ClaimPoint(Point point, PlayerInfo player)
        {
            player.score += point.value;
            if (point.useObjectPool)
            {
                NetworkServer.UnSpawn(point.gameObject);
                pointPool.ReturnPointToPool(point);
                _activePoints -= point.value;
            }

            if (!point.useObjectPool)
            {
                NetworkServer.Destroy(point.gameObject);
                _activePoints -= point.value;
            }
        }
    }
}
