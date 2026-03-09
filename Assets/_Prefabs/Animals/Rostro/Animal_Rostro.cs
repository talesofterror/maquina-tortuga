using UnityEngine;

public class Animal_Rostro : MonoBehaviour
{
  public SO_Rostro stats;

  [HideInInspector] public Animation anim;

  public int currentHP;
  public int currentAP;
  public float currentRotationSpeed;

  LaserGenerator laserGenerator;

  void Start()
  {

    //health setttings
    currentHP = stats.maxHP;
    currentAP = stats.maxAP;
    currentRotationSpeed = stats.rotationSpeed;

    // animation settings
    
    anim = GetComponent<Animation>();
    foreach (AnimationState state in anim)
    {
      state.speed = currentRotationSpeed; 
      // Debug.Log("Animation: " + state.name + " Length: " + state.length);
    }

    // laser settings
    laserGenerator = GetComponent<LaserGenerator>();
    laserGenerator.laserScale = stats.laserScale;
    laserGenerator.laserLength = stats.laserLength;
    laserGenerator.extendSpeed = stats.laserExtendSpeed;
    laserGenerator.retractSpeed = stats.laserRetractSpeed;
  }

}
