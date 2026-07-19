using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable, VolumeComponentMenu("Custom Effects/Screen Distortion")]
public class CustomPostProcessVolume : VolumeComponent, IPostProcessComponent
{
    public ClampedFloatParameter intensity = new ClampedFloatParameter(1f, 0f, 1f);

    public ColorParameter tintColor = new ColorParameter(Color.white);

    public ClampedFloatParameter distance = new ClampedFloatParameter(5f, 0f, 100f);

    // Tells Unity if the effect should actively render
    public bool IsActive() => intensity.value > 0f;

    public bool IsTileCompatible() => false;
}
