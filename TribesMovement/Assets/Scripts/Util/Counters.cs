using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class Counters : MonoBehaviour {
    private const int MILLIS_TO_NANOS = 1_000_000;
    private const double NANOS_TO_SECONDS = 1e-9;

    private static ConcurrentDictionary<Counter, CounterInfo> _counters = new ConcurrentDictionary<Counter, CounterInfo>();

    public static void CountMillis(Counter counter, long millis) {
        CounterInfo counterInfo = LoadCounter(counter);
        counterInfo.Counts++;
        counterInfo.TotalNanos += millis * MILLIS_TO_NANOS;
    }

    public static void CountNanos(Counter counter, long nanos) {
        CounterInfo counterInfo = LoadCounter(counter);
        counterInfo.Counts++;
        counterInfo.TotalNanos += nanos;
    }

    public static void DebugLog(Counter counter) {
        CounterInfo counterInfo = LoadCounter(counter);
        double avgNanos = counterInfo.TotalNanos / (float)counterInfo.Counts;
        double avgMillis = avgNanos / MILLIS_TO_NANOS;
        double totalSeconds = counterInfo.TotalNanos * NANOS_TO_SECONDS;
        Debug.Log($"Counter {counter}: Count {counterInfo.Counts} | TotalNanos {counterInfo.TotalNanos} | TotalSeconds {totalSeconds} | AvgNanos {avgNanos} | AvgMillis {avgMillis}");
    }

    private static CounterInfo LoadCounter(Counter counter) {
        CounterInfo counterInfo;
        if (!_counters.ContainsKey(counter)) {
            counterInfo = new CounterInfo();
            _counters[counter] = counterInfo;
        } else {
            counterInfo = _counters[counter];
        }
        return counterInfo;
    }
}

public class CounterInfo {
    public long Counts;
    public long TotalNanos;
}

public enum Counter {
    TerrainGeneration,
    NoiseMapGeneration,
    MeshGeneration,
}