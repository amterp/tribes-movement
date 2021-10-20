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
    private Vector3 _latestDownSlopeAcceleration = Vector3.down;

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
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.y * 2.05f)) {
            float x = hit.normal.x;
            float y = hit.normal.y;
            float z = hit.normal.z;

            Vector3 awayFromSlopeOnXZ = hit.normal.ZeroY();
            Vector3 normalForwardTangent = Vector3.Cross(hit.normal, awayFromSlopeOnXZ);
            Vector3 downSlopeVector = -Vector3.Cross(hit.normal, normalForwardTangent);


            // Let this magnitude be a.
            // Let theta be the angle between the flat xz plane and the normal of the surface.
            // Let x, y, z be the Vector3 components of the surface normal.
            //
            // tan(theta) = y / sqrt(x^2 + z^2)            (toa part of soh,cah,toa)
            //     theta = atan(sqrt(x^2 + z^2) / y)       (rearrange)
            // cos(theta) = g / a                          (via trigonometry laws)
            //     a = g / cos(theta)                      (rearrange)
            //       = g / cos(atan(sqrt(x^2 + z^2) / y))  (substitute theta)
            float downSlopeMagnitude = -Physics.gravity.y / Mathf.Cos(Mathf.Atan(Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(z, 2)) / y));

            Vector3 downSlopeAcceleration = downSlopeMagnitude * downSlopeVector;
            Debug.Log("Skiing " + "|" + downSlopeAcceleration.magnitude + "| " + downSlopeAcceleration.ToDetailedString());

            _rb.velocity += downSlopeAcceleration * Time.deltaTime;
            _lastFramedAccelerated = Time.frameCount;
            _latestDownSlopeAcceleration = downSlopeAcceleration;
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

    void OnDrawGizmos() {
        if (_latestDownSlopeAcceleration == Vector3.zero) return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, _latestDownSlopeAcceleration);
    }
}
