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

public class GameEvent<T, U> {
    public static GameEvent<T, U> operator +(GameEvent<T, U> gameEvent, Action<T, U> listener) {
        gameEvent.Register(listener);
        return gameEvent;
    }

    private event Action<T, U> delegateEvent;

    public void Register(Action<T, U> listener) {
        delegateEvent += listener;
    }

    public void Unregister(Action<T, U> listener) {
        delegateEvent -= listener;
    }

    public void SafeInvoke(T arg1, U arg2) {
        delegateEvent.SafeInvoke(arg1, arg2);
    }
}
