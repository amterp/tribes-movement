using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackFuelUi : MonoBehaviour {

    [SerializeField] public Transform uiBarToShrink;

    void Awake() {
        GameManager.Events.PlayerFuelChangedEvent += OnFuelChanged;
    }

    private void OnFuelChanged(float newFuelRatio) {
        uiBarToShrink.localScale = new Vector3(newFuelRatio, uiBarToShrink.localScale.y, uiBarToShrink.localScale.z);
    }
}
