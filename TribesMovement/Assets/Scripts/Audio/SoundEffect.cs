using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SoundEffect {
    public SoundEffectEnum SoundEffectEnum;
    public AudioClip Clip;

    [Range(0, 1)]
    public float Volume = 0.8f;
    [Range(0, 1)]
    public float Pitch = 0.5f;

    [HideInInspector]
    public AudioSource Source;
}
