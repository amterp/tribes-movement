using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackFuelUi : MonoBehaviour {

    [SerializeField] public Transform uiBarToShrink;

    void Start() {
        GameManager.Instance.Events.PlayerFuelChangedEvent += OnFuelChanged;
    }

    private void OnFuelChanged(float newFuelRatio, float deltaFuelRatio) {
        uiBarToShrink.localScale = new Vector3(newFuelRatio, uiBarToShrink.localScale.y, uiBarToShrink.localScale.z);
    }
}
