using System.Collections.Generic;
using UnityEngine;

namespace Core.Scoring
{
    public class PointPool : MonoBehaviour
    {
        public ScoreSpawner spawner;
        public List<Point> availablePool = new List<Point>();
        private Point _checkedOutPoint;
        
        public Point GetAvailablePooledPoint()
        {
            if (availablePool.Count > 0)
            {
                _checkedOutPoint = availablePool[0];
                availablePool.RemoveAt(0);
                return _checkedOutPoint;
            }

            _checkedOutPoint = spawner.SpawnPoint();
            return _checkedOutPoint;
        }

        public void ReturnPointToPool(Point point)
        {
            point.target = null;
            point.gameObject.SetActive(false);
            availablePool.Add(point);
        }
    }
}
