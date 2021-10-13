using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnLoad : MonoBehaviour {
    [SerializeField] private bool _setActive;

    void Start() {
        gameObject.SetActive(_setActive);
    }
}
