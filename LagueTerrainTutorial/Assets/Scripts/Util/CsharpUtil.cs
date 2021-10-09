using System;
using System.Collections;
using System.Collections.Generic;

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
}
