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
    }

    public Vector3 UseAndGetVector() {
        float efficiency = _jetpackTank.TryUseFuelAndReturnEfficiency();
        return transform.up * efficiency * _characterProps.JetpackAcceleration;
    }
}
