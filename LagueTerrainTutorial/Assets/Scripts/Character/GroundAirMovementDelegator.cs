using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IsGroundedChecker))]
public class GroundAirMovementDelegator : MonoBehaviour, ICharacterMover {

    [SerializeField] private MonoBehaviour _groundCharacterMoverObject;
    [SerializeField] private MonoBehaviour _airCharacterMoverObject;
    private ICharacterMover _groundCharacterMover;
    private ICharacterMover _airCharacterMover;

    private IsGroundedChecker _isGroundedChecker;
    private Vector3 _latestAcceleration = Vector3.zero;

    void Awake() {
        _groundCharacterMover = _groundCharacterMoverObject as ICharacterMover;
        _airCharacterMover = _airCharacterMoverObject as ICharacterMover;
        _isGroundedChecker = GetComponent<IsGroundedChecker>();
    }

    public void UpdateMover() {
        if (ShouldDelegateToGroundMover()) {
            _groundCharacterMover.UpdateMover();
        } else {
            _airCharacterMover.UpdateMover();
        }
    }

    public void LateUpdateMover() {
        if (ShouldDelegateToGroundMover()) {
            _groundCharacterMover.LateUpdateMover();
        } else {
            _airCharacterMover.LateUpdateMover();
        }
    }

    public void Accelerate(Vector3 vector) {
        _latestAcceleration = vector;
        if (ShouldDelegateToGroundMover()) {
            _groundCharacterMover.Accelerate(vector);
        } else {
            _airCharacterMover.Accelerate(vector);
        }
    }

    private bool ShouldDelegateToGroundMover() {
        return _isGroundedChecker.IsGrounded && _latestAcceleration.y == 0;
    }
}
