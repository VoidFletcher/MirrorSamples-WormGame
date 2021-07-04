using System;
using System.Collections;
using System.Collections.Generic;
using Core.Player;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private PlayerInfo _playerInfo;

    private void Start()
    {
        _playerInfo = GetComponent<PlayerInfo>();
        if (_playerInfo.HasAuthority()) _playerInfo.cameraContainer.SetActive(true);
        if (!_playerInfo.HasAuthority()) enabled = false;
    }

    private void Update()
    {
        _playerInfo.cameraContainer.transform.position = _playerInfo.playerBody.head.position;
    }
}
