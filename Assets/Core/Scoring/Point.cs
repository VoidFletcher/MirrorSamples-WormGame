using System;
using Mirror;
using UnityEngine;

namespace Core.Scoring
{
    public class Point : NetworkBehaviour
    {
        public bool useObjectPool;
        public int value;
        public Sprite sprite;
        public float scale = 1f;
        public SpriteRenderer spriteRenderer;
        public Collider collisionArea;
        public PointCollector target;
        private float _speed;
        private bool _triggered;

        [SyncVar] 
        public Vector4 color;

        [SyncVar] 
        public int spriteId;

        public override void OnStartClient()
        {
            transform.localScale *= scale;
            spriteRenderer.color = new Color32((byte)color.x, (byte)color.y, (byte)color.z, (byte)color.w);
        }
        
        public void Tractor(PointCollector destination)
        {
            target = destination;
            _speed = destination.playerInfo.baseSpeed * destination.playerInfo.speedModifier + 1f;
            _triggered = true;
        }

        private void Update()
        {
            if (connectionToServer != null) return;
            if (!_triggered) return;
            if (target == null)
            {
                _triggered = false;
                return;
            }
            transform.position = Vector3.Lerp(transform.position, target.transform.position, _speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.transform.position) < 1f)
            {
                _triggered = false;
                ScoreSpawner.instance.ClaimPoint(this, target.playerInfo);
            }
        }
    }
}
