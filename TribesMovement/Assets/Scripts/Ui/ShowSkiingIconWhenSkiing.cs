using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSkiingIconWhenSkiing : MonoBehaviour {

    [SerializeField] private GameObject _icon;
    [SerializeField] private bool _startHidden = true;

    void Awake() {
        GameManager.Instance.Events.PlayerIsSkiingStateChangedEvent += isSkiing => _icon.SetActive(isSkiing);
    }

    void Start() {
        _icon.SetActive(!_startHidden);
    }
}
