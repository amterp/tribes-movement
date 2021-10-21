using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedTextUpdater : MonoBehaviour {

    private const float UNITS_PER_SECOND_TO_KILO_UNITS_PER_HOUR = 3.6f;

    [SerializeField] private Rigidbody _subject;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _speedMultiplier = 1f;

    void LateUpdate() {
        float unitsPerSecond = _subject.velocity.magnitude;
        float kiloUnitsPerHour = unitsPerSecond * UNITS_PER_SECOND_TO_KILO_UNITS_PER_HOUR;
        float adjustedSpeed = kiloUnitsPerHour * _speedMultiplier;
        _text.text = string.Format("{0:0} KPH", adjustedSpeed);
    }
}
