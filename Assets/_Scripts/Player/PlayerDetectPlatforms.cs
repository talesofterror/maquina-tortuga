using System;
using System.Collections;
using Invector.vCharacterController;
using UnityEngine;

public class PlayerDetectPlatforms : MonoBehaviour
{
    vThirdPersonController cc;
    Rigidbody rB;
    GameObject PlayerGameObject;
    Platform platform;
    Vector3 platformContactPoint;

    [HideInInspector]
    public bool active;
    public float yModifier;

    void Start()
    {
        cc = GetComponent<vThirdPersonController>();
        rB = GetComponent<Rigidbody>();
        PlayerGameObject = this.gameObject;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform") && cc.isGrounded)
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
            active = false;
            UISingleton.i.debug.pushMessage(collision.transform.name + "active?: " + active);
        }
    }

    void FixedUpdate()
    {
        Vector3 calculatedVelocity = Vector3.zero;
        if (active)
        {
            if (platform != null)
            {
                calculatedVelocity = new Vector3(
                    platform.CalculateVelocity().x,
                    platform.CalculateVelocity().y >= 0 ? 0 : platform.CalculateVelocity().y,
                    platform.CalculateVelocity().z
                );
            }

            if (!cc.isJumping)
            {
                if (platform != null && platform.yFloor != null)
                {
                    rB.MovePosition(
                        new Vector3(rB.position.x, platform.yFloor.position.y, rB.position.z)
                    );
                }
                else
                {
                    active = false;
                }
            }

            cc.platformVelocity = calculatedVelocity;
        }
        else if (!active && cc.isGrounded)
        {
            cc.platformVelocity = Vector3.zero;
        }
    }
}
