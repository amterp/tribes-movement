using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerProperties))]
public class LookX : MonoBehaviour {

    private PlayerProperties _playerProperties;

    void Awake() {
        _playerProperties = GetComponent<PlayerProperties>();
    }

    void Update() {
        float mouseHorizontalInput = Input.GetAxis("Mouse X");
        if (Mathf.Approximately(mouseHorizontalInput, 0)) return;

        float horizontalRotationDelta = mouseHorizontalInput * _playerProperties.LookSensitivityHorizontal * Time.deltaTime;
        transform.Rotate(0, horizontalRotationDelta, 0);
    }
}
