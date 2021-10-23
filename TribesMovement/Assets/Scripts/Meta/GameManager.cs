using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance {
        get {
            Debug.Assert(_backingInstance != null, "GameManager instance accessed but not defined!");
            return _backingInstance;
        }
        private set { }
    }
    private static GameManager? _backingInstance;

    public Events Events { get; private set; }
    public AudioManager AudioManager { get { return _audioManager; } private set { } }

    [SerializeField] private AudioManager _audioManager;

    void Awake() {
        Initialize();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Initialize() {
        if (_backingInstance != null) {
            Destroy(gameObject);
            return;
        }

        _backingInstance = this;
        DontDestroyOnLoad(this);
        Events = new Events();
    }
}
