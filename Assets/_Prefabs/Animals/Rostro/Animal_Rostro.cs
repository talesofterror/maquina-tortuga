using PixelCrushers.DialogueSystem.Demo;
using UnityEngine;

public class Animal_Rostro : MonoBehaviour, I_Animal
{
  public SO_Rostro stats;

  [HideInInspector] public Animation anim;


  public int hp
  {
    get { return _hp; }
    set { _hp = value; }
  }

  public int ap
  {
    get { return _ap; }
    set { _ap = value; }
  }

  public int mp
  {
    get { return _mp; }
    set { _mp = value; }
  }

  public int _hp;
  public int _mp;
  public int _ap;
  public float currentRotationSpeed;

  LaserGenerator laserGenerator;

  void Start()
  {

    //health setttings
    _hp = stats.maxHP;
    _ap = stats.maxAP;
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

  public void TakeDamage (int amount, GameObject focus)
  {
    Debug.Log($"{transform.name} called TakeDamage()");
  }

}
