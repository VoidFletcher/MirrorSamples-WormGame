using System;
using UnityEngine;

namespace Core.Player
{
    public class InputProvider : MonoBehaviour
    {
        private PlayerInfo _playerInfo;
        private PlayerMovement _playerMovement;
        private Vector2 _newMovement = new Vector2();

        private void Start()
        {
            _playerInfo = GetComponent<PlayerInfo>();
            _playerMovement = GetComponent<PlayerMovement>();
        }
        
        
        
        private void Update()
        {
            if (!_playerInfo.HasAuthority()) return;

            var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (Mathf.Abs(input.x) + Mathf.Abs(input.y) != 0)
            {
                _newMovement = input;
            }
        }

        public Vector2 GetMovementInput()
        {
            return _newMovement;
        }
    }
}
