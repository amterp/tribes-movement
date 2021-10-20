using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerFrameCache<T> {

    private readonly Func<T> _valueUpdater;
    private T _value;
    private int _lastFrameUpdated;

    public PerFrameCache(Func<T> valueUpdater) {
        _valueUpdater = valueUpdater;
    }

    public T Get() {
        int currentFrame = Time.frameCount;
        if (currentFrame != _lastFrameUpdated) {
            _value = _valueUpdater();
            _lastFrameUpdated = currentFrame;
        }
        return _value;
    }
}
