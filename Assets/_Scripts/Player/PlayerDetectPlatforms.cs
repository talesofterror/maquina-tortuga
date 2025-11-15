using Invector.vCharacterController;
using UnityEngine;
using System;
using System.Collections;

public class PlayerDetectPlatforms : MonoBehaviour
{

  vThirdPersonController cc;
  Rigidbody rB;
  GameObject PlayerGameObject;
  MovingPlatform oldMovingPlatformsClass;
  Platform platform;
  Vector3 platformContactPoint;
  [HideInInspector] public bool active;
  public float yModifier;


  void Start()
  {
    cc = GetComponent<vThirdPersonController>();
    rB = GetComponent<Rigidbody>();
    PlayerGameObject = this.gameObject;
  }

  public bool oldMovingPlatform = false;

  void OnCollisionEnter(Collision collision)
  {
    if (oldMovingPlatform && collision.gameObject.CompareTag("Platform") && cc.isGrounded)
    {
      oldMovingPlatformsClass = collision.gameObject.GetComponent<MovingPlatform>();
      platformContactPoint = collision.contacts[0].point;
      active = true;
      UISingleton.i.debug.pushMessage("grounded = " + cc.isGrounded);
      UISingleton.i.debug.pushMessage(collision.transform.name + "active?: " + active);
    }
    if (!oldMovingPlatform && collision.gameObject.CompareTag("Platform") && cc.isGrounded)
    {
      platform = collision.gameObject.GetComponent<Platform>();
      platformContactPoint = collision.contacts[0].point;
      active = true;
      UISingleton.i.debug.pushMessage("grounded = " + cc.isGrounded);
      UISingleton.i.debug.pushMessage(collision.transform.name + "active?: " + active);
    }
  }

  void OnCollisionExit(Collision collision)
  {
    if (collision.gameObject.CompareTag("Platform"))
    {
      platform = null;
      oldMovingPlatformsClass = null;
      active = false;
      UISingleton.i.debug.pushMessage(collision.transform.name + "active?: " + active);
    }
  }

  void FixedUpdate()
  {
    Vector3 calculatedVelocity = Vector3.zero;
    if (active)
    {
      if (oldMovingPlatform)
      {
        calculatedVelocity = new Vector3(
          oldMovingPlatformsClass.CalculateVelocity().x,
          0,
          oldMovingPlatformsClass.CalculateVelocity().z);
      }
      if (!oldMovingPlatform)
      {
        calculatedVelocity = new Vector3(
          platform.CalculateVelocity().x,
          platform.CalculateVelocity().y >= 0 ? 0 : platform.CalculateVelocity().y,
          platform.CalculateVelocity().z);
      }

      if (!cc.isJumping)
      {
        rB.MovePosition(new Vector3(rB.position.x, platform.yFloor.position.y, rB.position.z));
      }

      cc.platformVelocity = calculatedVelocity;

    }
    else if (!active && cc.isGrounded)
    {
      cc.platformVelocity = Vector3.zero;
    }
  }

}
