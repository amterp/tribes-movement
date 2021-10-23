using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    [SerializeField] private AudioMixerGroup _soundGroup;
    [SerializeField] private SoundEffect[] _effects;

    private Dictionary<SoundEffectEnum, SoundEffect> _effectsByEnum;

    void Awake() {
        _effectsByEnum = new Dictionary<SoundEffectEnum, SoundEffect>();
        SetupSounds();
    }

    public void PlayEffect(SoundEffectEnum effectEnum) {
        SoundEffect? sound = _effectsByEnum[effectEnum];
        if (sound != null && !sound.Source.isPlaying) {
            sound.Source.Play();
        }
    }

    public void StopEffect(SoundEffectEnum effectEnum) {
        SoundEffect? sound = _effectsByEnum[effectEnum];
        sound?.Source.Stop();
    }

    private void SetupSounds() {
        foreach (SoundEffect effect in _effects) {
            effect.Source = gameObject.AddComponent<AudioSource>();
            effect.Source.clip = effect.Clip;
            effect.Source.volume = effect.Volume;
            effect.Source.pitch = effect.Pitch;
            effect.Source.outputAudioMixerGroup = _soundGroup;
            effect.Source.playOnAwake = false;

            if (_effectsByEnum.ContainsKey(effect.SoundEffectEnum)) {
                Debug.LogWarning($"Multiple effects of enum {effect.SoundEffectEnum} - only one allowed! Using the first one.");
            } else {
                Debug.Log("Loaded effect: " + effect.SoundEffectEnum);
                _effectsByEnum.Add(effect.SoundEffectEnum, effect);
            }
        }
    }
}
