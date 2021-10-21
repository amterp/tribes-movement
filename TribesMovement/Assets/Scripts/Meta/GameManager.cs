using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static readonly Events Events = new Events();

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
