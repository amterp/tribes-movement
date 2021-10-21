using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent<T> {
    public static GameEvent<T> operator +(GameEvent<T> gameEvent, Action<T> listener) {
        gameEvent.Register(listener);
        return gameEvent;
    }

    private event Action<T> delegateEvent;

    public void Register(Action<T> listener) {
        delegateEvent += listener;
    }

    public void Unregister(Action<T> listener) {
        delegateEvent -= listener;
    }

    public void SafeInvoke(T arg) {
        delegateEvent.SafeInvoke(arg);
    }
}
