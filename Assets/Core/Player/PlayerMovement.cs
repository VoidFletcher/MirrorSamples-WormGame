using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Core.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        public Transform navmeshTargetController;
        public Transform navmeshTarget;
        private PlayerInfo _playerInfo;
        private InputProvider _inputProvider;


        private void ChangeMovementVector(Vector2 newMovementTarget)
        {
            newMovementTarget.x *= -1;
            var angle = Vector2.SignedAngle(Vector2.up, newMovementTarget);
            navmeshTargetController.rotation = Quaternion.Euler(0,angle,0);
        }

        public void SetTarget(Vector3 location)
        {
            navmeshTarget.position = location;
        }
        
        private void Start()
        {
            _playerInfo = GetComponent<PlayerInfo>();
            _inputProvider = GetComponent<InputProvider>();
        }

        private void Update()
        {
            if (_playerInfo.HasAuthority())
            {
                ChangeMovementVector(_inputProvider.GetMovementInput());
                var playerHeadTransform = _playerInfo.playerBody.head.transform;

                var headSegment = _playerInfo.playerBody.segments[0];
                var headSegmentTransform = headSegment.transform;
                headSegment.previousPosition = headSegmentTransform.position;
                headSegment.previousRotation = headSegmentTransform.rotation;

                playerHeadTransform.rotation = Quaternion.RotateTowards (
                    playerHeadTransform.rotation,
                    Quaternion.LookRotation(navmeshTarget.position - playerHeadTransform.position ),
                    Time.deltaTime * _playerInfo.turningAngleRadians );
                
                playerHeadTransform.Translate(Vector3.forward * 
                                              (_playerInfo.baseSpeed * 
                                               _playerInfo.speedModifier * 
                                               Time.deltaTime));
            }

            for (int i = 0; i < _playerInfo.playerBody.totalSegments; i++)
            {
                if (i == 0) continue;
                if (i > _playerInfo.playerBody.segments.Count - 1) return;
                var segment = _playerInfo.playerBody.segments[i];
                var segmentTransform = segment.transform;

                var previousSegment = _playerInfo.playerBody.segments[i - 1];
                var previousSegmentTransform = previousSegment.transform;
                
                var distance = Vector3.Distance(segmentTransform.position, previousSegment.transform.position);
                if (!segment.isSpawned)
                {
                    if (distance < _playerInfo.playerBody.segmentDistance * Time.deltaTime) break;
                    segment.isSpawned = true;
                }
                
                segmentTransform.position = Vector3.Lerp(
                    segmentTransform.position, 
                    previousSegmentTransform.position, 
                    _playerInfo.playerBody.segmentDistance * (_playerInfo.baseSpeed * _playerInfo.speedModifier) * Time.deltaTime);
                
                segmentTransform.rotation = Quaternion.Lerp(
                    segmentTransform.rotation, 
                    previousSegmentTransform.rotation, 
                    _playerInfo.turningAngleRadians * (_playerInfo.baseSpeed * _playerInfo.speedModifier) * Time.deltaTime);
            }
            
            if (!_playerInfo.HasAuthority())
            {
                var headSegment = _playerInfo.playerBody.segments[0];
                var headSegmentTransform = headSegment.transform;
                headSegment.previousPosition = headSegmentTransform.position;
                headSegment.previousRotation = headSegmentTransform.rotation;
            }
        }
        
    }
}
