using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(Rigidbody))]
public class AirCharacterMover : MonoBehaviour, ICharacterMover {

    private CharacterProperties _characterProperties;
    private Rigidbody _rb;

    void Awake() {
        _characterProperties = GetComponent<CharacterProperties>();
        _rb = GetComponent<Rigidbody>();
    }

    public void Accelerate(Vector3 vector) {
        Vector3 acceleration = Vector3.zero;
        acceleration += vector.ZeroY().normalized * _characterProperties.AccelerationAmount * _characterProperties.MidAirAccelerationFactor;
        acceleration += new Vector3(0, vector.y.normalized() * _characterProperties.JetpackPower, 0);
        _rb.velocity += acceleration * Time.deltaTime;
    }

    public void UpdateMover() {
        // no-op
    }

    public void LateUpdateMover() {
        // no-op
    }
}
