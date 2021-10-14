using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookY : MonoBehaviour {

    [SerializeField] private PlayerProperties _playerProperties;
    private float _currentVerticalRotation = 0;

    void Update() {
        float mouseVerticalInput = Input.GetAxis("Mouse Y");
        if (Mathf.Approximately(mouseVerticalInput, 0)) return;

        float verticalRotation = -mouseVerticalInput * _playerProperties.LookSensitivityVertical * Time.deltaTime;

        float newVerticalRotation = Mathf.Clamp(_currentVerticalRotation + verticalRotation, -90, 90);
        float rotationDelta = newVerticalRotation - _currentVerticalRotation;
        transform.Rotate(rotationDelta, 0, 0);

        _currentVerticalRotation = newVerticalRotation;
    }
}
