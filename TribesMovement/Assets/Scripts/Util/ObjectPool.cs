using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

public class ObjectPool<T> {
    private readonly ConcurrentQueue<T> _pool;
    private readonly Func<T> _instantiationFunction;

    public static ObjectPool<T> Create(Func<T> instantiationFunction) {
        return new ObjectPool<T>(instantiationFunction);
    }

    private ObjectPool(Func<T> instantiationFunction) {
        this._instantiationFunction = instantiationFunction;
        _pool = new ConcurrentQueue<T>();
    }

    public V Lease<V>(Func<T, V> func) {
        T instance;
        if (!_pool.TryDequeue(out instance)) {
            instance = _instantiationFunction();
        }
        V output = func(instance);
        _pool.Enqueue(instance);
        return output;
    }
}
