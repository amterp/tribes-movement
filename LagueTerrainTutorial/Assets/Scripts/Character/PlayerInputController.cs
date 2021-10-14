using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMover))]
public class PlayerInputController : MonoBehaviour {
    private CharacterMover _characterMover;

    void Awake() {
        _characterMover = GetComponent<CharacterMover>();
    }

    void Update() {
        RunPlayerInput();
    }

    private void RunPlayerInput() {
        Vector3 targetDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) targetDirection += -transform.right;
        if (Input.GetKey(KeyCode.D)) targetDirection += transform.right;

        if (Input.GetKey(KeyCode.W)) targetDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) targetDirection += -transform.forward;

        if (targetDirection == Vector3.zero) return;

        _characterMover.Accelerate(targetDirection.normalized);
    }
}
