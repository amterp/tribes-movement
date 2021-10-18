using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGroundedChecker : MonoBehaviour {

    public bool IsGrounded { get { return _isGrounded.Get(); } private set { } }

    private PerFrameCache<bool> _isGrounded;

    void Awake() {
        _isGrounded = new PerFrameCache<bool>(CalculateIsGrounded);
    }

    private bool CalculateIsGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, transform.localScale.y * 1.05f);
    }
}
