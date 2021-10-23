using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour {
    private const int RIGHT_CLICK = 1;

    [SerializeField] private MonoBehaviour _characterMoverGameObject;
    private ICharacterMover _characterMover;
    private bool _skiiedPreviousFrame = false;

    void Awake() {
        _characterMover = _characterMoverGameObject as ICharacterMover;
    }

    void Update() {
        RunPlayerInput();
        _characterMover.UpdateMover();
    }

    void LateUpdate() {
        _characterMover.LateUpdateMover();
    }

    private void RunPlayerInput() {
        bool pressJetpack = Input.GetMouseButton(RIGHT_CLICK);
        if (!pressJetpack && Input.GetKey(KeyCode.Space)) {
            _characterMover.Ski();
            UpdateIsSkiingStatus(true);
            return;
        }
        UpdateIsSkiingStatus(false);

        Vector3 targetDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) targetDirection += -transform.right;
        if (Input.GetKey(KeyCode.D)) targetDirection += transform.right;

        if (Input.GetKey(KeyCode.W)) targetDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) targetDirection += -transform.forward;

        if (pressJetpack) targetDirection += transform.up;

        if (targetDirection == Vector3.zero) return;

        _characterMover.Accelerate(targetDirection.normalized);
    }

    private void UpdateIsSkiingStatus(bool isSkiing) {
        if (_skiiedPreviousFrame != isSkiing) {
            _skiiedPreviousFrame = isSkiing;
            GameManager.Instance.Events.PlayerIsSkiingStateChangedEvent.SafeInvoke(isSkiing);
        }
    }
}
