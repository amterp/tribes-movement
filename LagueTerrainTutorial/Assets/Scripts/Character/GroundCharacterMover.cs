using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterProperties))]
[RequireComponent(typeof(Rigidbody))]
public class GroundCharacterMover : MonoBehaviour, ICharacterMover {

    private CharacterProperties _characterProperties;
    private Rigidbody _rb;
    private int _lastFramedAccelerated;

    void Awake() {
        _characterProperties = GetComponent<CharacterProperties>();
        _rb = GetComponent<Rigidbody>();
    }

    public void UpdateMover() {
        // no-op
    }

    public void LateUpdateMover() {
        SlowDownIfNotAccelerated();
    }

    public void Accelerate(Vector3 direction) {
        if (_rb.velocity.magnitude < _characterProperties.MaxSpeed) {
            Vector3 changeInVelocity = Vector3.ClampMagnitude(direction * _characterProperties.AccelerationAmount, _characterProperties.AccelerationAmount) * Time.deltaTime;
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity + changeInVelocity, _characterProperties.MaxSpeed);
        }
        _lastFramedAccelerated = Time.frameCount;
    }

    public void Ski() {
        Debug.Log("Skiing!" + Physics.gravity.y);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.y * 1.05f)) {
            float x = hit.normal.x;
            float y = hit.normal.y;

            float ay = x * Physics.gravity.y / Mathf.Sqrt(1 + Mathf.Pow(y, 2) / Mathf.Pow(x, 2));
            float ax = y / x * ay;
            Vector3 skiingAcceleration = new Vector3(ax, ay, 0); // todo calculate z...
            Debug.Log(skiingAcceleration.ToDetailedString());
            _rb.velocity += skiingAcceleration * Time.deltaTime;

            _lastFramedAccelerated = Time.frameCount;
        }
    }

    private void SlowDownIfNotAccelerated() {
        if (Time.frameCount == _lastFramedAccelerated) return;
        if (_rb.velocity.sqrMagnitude < 0.06) {
            _rb.velocity = Vector3.zero;
            return;
        };

        Accelerate(-_rb.velocity.ZeroY() / 2);
    }
}
