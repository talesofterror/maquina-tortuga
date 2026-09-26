using System.Collections;
using UnityEngine;

public class UTILITY
{
  public static IEnumerator InterpolateDirections(Transform targetTransform,
    Vector3 startDirection,
    Vector3 endDirection,
    float duration,
    bool stopCondition = false)
  {
    if (targetTransform == null)
    {
      yield break;
    }

    if (startDirection.sqrMagnitude <= 0.01f || endDirection.sqrMagnitude <= 0.01f)
    {
      yield break;
    }

    Quaternion fromRotation = Quaternion.LookRotation(startDirection.normalized, Vector3.up);
    Quaternion toRotation = Quaternion.LookRotation(endDirection.normalized, Vector3.up);

    float elapsed = 0f;
    while (elapsed < duration)
    {
      if (stopCondition)
      {
        yield break;
      }

      float t = elapsed / duration;
      targetTransform.rotation = Quaternion.Slerp(fromRotation, toRotation, t);
      elapsed += Time.deltaTime;
      yield return null;
    }

    targetTransform.rotation = toRotation;
  }

  public static class VectorPacker
  {
    private const float Scale = 100f;          // Preserves two decimal places
    private const int Offset = 32768;          // Shifts -327.68 to 0 (removes negative values)
    private const int BitsPerAxis = 16;        // 16 bits * 3 axes = 48 bits total (fits in 64-bit long)
    private const long Mask = 0xFFFF;          // Isolates 16 bits

    // Encodes Vector3 to long
    public static long PackVector3(Vector3 v)
    {
      // 1. Scale and offset to create positive integers
      long x = (long)Mathf.RoundToInt((v.x * Scale) + Offset);
      long y = (long)Mathf.RoundToInt((v.y * Scale) + Offset);
      long z = (long)Mathf.RoundToInt((v.z * Scale) + Offset);

      // 2. Pack into a single 64-bit long using bitwise OR
      return x | (y << BitsPerAxis) | (z << (BitsPerAxis * 2));
    }

    // Decodes long back to Vector3
    public static Vector3 UnpackVector3(long packed)
    {
      // 1. Extract the 16-bit chunks using bit shifts and masks
      long x = packed & Mask;
      long y = (packed >> BitsPerAxis) & Mask;
      long z = (packed >> (BitsPerAxis * 2)) & Mask;

      // 2. Reverse the offset and scale
      float vx = (x - Offset) / Scale;
      float vy = (y - Offset) / Scale;
      float vz = (z - Offset) / Scale;

      return new Vector3(vx, vy, vz);
    }
  }


}
