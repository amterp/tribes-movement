using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(JetpackTank))]
public class Jetpack : MonoBehaviour {

    private CharacterProperties _characterProps;
    private JetpackTank _jetpackTank;

    void Awake() {
        _characterProps = GetComponent<CharacterProperties>();
        _jetpackTank = GetComponent<JetpackTank>();
        GameManager.Instance.Events.PlayerFuelChangedEvent += OnFuelUsed;
    }

    public Vector3 UseAndGetVector() {
        float efficiency = _jetpackTank.TryUseFuelAndReturnEfficiency();
        return transform.up * efficiency * _characterProps.JetpackAcceleration;
    }

    private void OnFuelUsed(float newFuelRatio, float deltaFuelRatio) {
        if (FuelWasUsed(deltaFuelRatio)) {
            GameManager.Instance.AudioManager.PlayEffect(SoundEffectEnum.Thrust);
        } else {
            GameManager.Instance.AudioManager.StopEffect(SoundEffectEnum.Thrust);
        }
    }

    private static bool FuelWasUsed(float deltaFuelRatio) {
        return deltaFuelRatio < 0;
    }
}
