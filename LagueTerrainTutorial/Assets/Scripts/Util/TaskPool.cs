using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class TaskPool<T> {
    private Queue<Task<T>> _taskQueue;
    private ConcurrentQueue<(T, Action<T>)> _resultQueue;

    private volatile int _availableReservations;

    public static TaskPool<T> FixedSize(int maxParallelTasks) {
        return new TaskPool<T>(maxParallelTasks, new Queue<Task<T>>(), new ConcurrentQueue<(T, Action<T>)>());
    }

    private TaskPool(int maxParallelTasks, Queue<Task<T>> taskQueue, ConcurrentQueue<(T, Action<T>)> resultQueue) {
        _availableReservations = maxParallelTasks;
        _taskQueue = taskQueue;
        _resultQueue = resultQueue;
    }

    // Be careful to call from the correct thread e.g. the Unity main thread.
    public void CallbackOnResults() {
        while (_resultQueue.TryDequeue(out var resultWithCallback)) {
            T result = resultWithCallback.Item1;
            Action<T> callback = resultWithCallback.Item2;
            callback(result);
        }
    }

    public void Submit(Func<T> taskFunction, Action<T> callback) {
        _taskQueue.Enqueue(new Task<T>(taskFunction, callback));
        RunTasksToCapacity();
    }

    private void RunTasksToCapacity() {
        for (int i = 0; i < _taskQueue.Count && _availableReservations > 0; i++) {
            Task<T> task = _taskQueue.Dequeue();
            ThreadPool.QueueUserWorkItem((i) => {
                T result = task._taskFunction();
                _resultQueue.Enqueue((result, task._callback));
                RunTasksToCapacity();
            });
        }
    }
}

struct Task<T> {
    public readonly Func<T> _taskFunction;
    public readonly Action<T> _callback;

    public Task(Func<T> taskFunction, Action<T> callback) {
        _taskFunction = taskFunction;
        _callback = callback;
    }
}