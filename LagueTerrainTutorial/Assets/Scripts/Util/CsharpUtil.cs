using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CsharpUtil {
    private static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0);

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
        foreach (var item in enumerable)
            action(item);
    }

    public static long CurrentTimeMillis() {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    public static long CurrentTimeNanos() {
        return (long)((DateTime.UtcNow - EPOCH).TotalMilliseconds * 1000000.0);
    }

    public static T Last<T>(this T[] array) {
        return array[array.Length - 1];
    }

    public static Vector2 ToVector2DroppingY(this Vector3 vector3) {
        return new Vector2(vector3.x, vector3.z);
    }

    public static Vector3 ZeroY(this Vector3 vector3) {
        return new Vector3(vector3.x, 0, vector3.z);
    }

    public static string ToDetailedString(this Vector3 vector3) {
        return $"({vector3.x}, {vector3.y}, {vector3.z})";
    }
}
