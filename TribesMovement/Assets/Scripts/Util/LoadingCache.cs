using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

public class LoadingCache<K, V> {
    private ConcurrentDictionary<K, V> _backingDict;
    private Action<K, Action<V>> _loadingFunction;

    public static LoadingCache<K, V> Create(Action<K, Action<V>> loadingFunction) {
        return new LoadingCache<K, V>(new ConcurrentDictionary<K, V>(), loadingFunction);
    }

    private LoadingCache(ConcurrentDictionary<K, V> backingDict, Action<K, Action<V>> loadingFunction) {
        _backingDict = backingDict;
        _loadingFunction = loadingFunction;
    }

    public void Load(K key, Action<V> callback) {
        if (!_backingDict.ContainsKey(key)) {
            _loadingFunction(key, OnValueLoad(key, callback));
            return;
        }
        callback(_backingDict[key]);
    }

    public V Get(K key, V defaultValue = default) {
        V value;
        if (_backingDict.TryGetValue(key, out value)) {
            return value;
        } else {
            return defaultValue;
        }
    }

    private Action<V> OnValueLoad(K key, Action<V> callback) {
        return value => {
            _backingDict.TryAdd(key, value);
            callback(value);
        };
    }
}
