using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FSM_PlayerState_Casting : FSM_PlayerStateBase
{
  public ComputerController computerController;
  [SerializeField] private float hueShiftSpeed = 200f;
  private float hueShiftValue = 0f;
  private bool hueShiftIncreasing = true;

  public FSM_PlayerState_Casting(FSM_PlayerStateController c) : base(c) { }

  public override void Enter()
  {
    Debug.Log("Entering Casting SubState");

    hueShiftValue = 0f;
    hueShiftIncreasing = true;

    PLAYERSingleton.i.inputDisabled = true;
    PLAYERSingleton.i.rB.linearVelocity = Vector3.zero;
    CAMERASingleton.i.effectVolume.SetActive(true);
    computerController.Opening();
  }

  public override void Exit()
  {
    PLAYERSingleton.i.animations.animator.SetBool("CastStance", false);
    PLAYERSingleton.i.inputDisabled = false;
    CAMERASingleton.i.effectVolume.SetActive(false);
  }

  public override void Loop()
  {
    UpdateHueShiftOverride();

    if (GMSingleton.i.inputManager.cast.WasReleasedThisFrame())
    {
      computerController.Closing();
      _superState?.SetSubState(controller.state_Looking);
    }
  }

  private void UpdateHueShiftOverride()
  {
    if (CAMERASingleton.i == null || CAMERASingleton.i.effectVolumeComponent == null)
    {
      return;
    }

    if (CAMERASingleton.i.effectVolumeComponent.profile == null)
    {
      return;
    }

    if (!CAMERASingleton.i.effectVolumeComponent.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
    {
      return;
    }

    float delta = hueShiftSpeed * Time.deltaTime;

    if (hueShiftIncreasing)
    {
      hueShiftValue += delta;
      if (hueShiftValue >= 180f)
      {
        hueShiftValue = 180f;
        hueShiftIncreasing = false;
      }
    }
    else
    {
      hueShiftValue -= delta;
      if (hueShiftValue <= -180f)
      {
        hueShiftValue = -180f;
        hueShiftIncreasing = true;
      }
    }

    colorAdjustments.hueShift.overrideState = true;
    colorAdjustments.hueShift.value = hueShiftValue;
  }
}
