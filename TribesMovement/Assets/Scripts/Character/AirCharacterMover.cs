using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Jetpack))]
public class AirCharacterMover : MonoBehaviour, ICharacterMover {

    private CharacterProperties _characterProperties;
    private Rigidbody _rb;
    private Jetpack _jetpack;

    void Awake() {
        _characterProperties = GetComponent<CharacterProperties>();
        _rb = GetComponent<Rigidbody>();
        _jetpack = GetComponent<Jetpack>();
    }

    public void Accelerate(Vector3 vector) {
        Vector3 acceleration = Vector3.zero;
        acceleration += vector.ZeroY().normalized * _characterProperties.AccelerationAmount * _characterProperties.MidAirAccelerationFactor;
        acceleration += vector.y.isAboutZero() ? Vector3.zero : _jetpack.UseAndGetVector();
        _rb.velocity += acceleration * Time.deltaTime;
    }

    public void UpdateMover() {
        // no-op
    }

    public void LateUpdateMover() {
        // no-op
    }

    public void Ski() {
        throw new NotImplementedException($"{typeof(AirCharacterMover).Name} does not know how to Ski!");
    }
}
