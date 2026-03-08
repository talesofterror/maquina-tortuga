using UnityEngine;

public class Animal_Rostro : MonoBehaviour
{
  public SO_Rostro stats;

  public int currentHP;
  public int currentAP;
  
  LaserGenerator laserGenerator;

  void Start()
  {
    currentHP = stats.maxHP;
    currentAP = stats.maxAP;

    laserGenerator = GetComponent<LaserGenerator>();
    laserGenerator.laserScale = stats.laserScale;
    laserGenerator.laserLength = stats.laserLength;  
    laserGenerator.extendSpeed = stats.laserExtendSpeed;
    laserGenerator.retractSpeed = stats.laserRetractSpeed;
  }

}
