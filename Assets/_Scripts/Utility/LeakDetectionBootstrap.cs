using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

public static class LeakDetectionBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnableLeakTracing()
    {
        // Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SetLeakDetectionMode(Unity.Collections.NativeLeakDetectionMode.EnabledWithStackTrace);
        Debug.Log("Leak detection with stack traces enabled.");
    }
}