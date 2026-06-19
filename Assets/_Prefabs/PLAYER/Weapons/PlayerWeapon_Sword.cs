using System;
using Unity.VisualScripting;
using UnityEngine;

// [System.Serializable]
public class PlayerWeapon_Sword : PlayerWeapon_BASE
{
  // * weird trick to access scriptable object data
  public ScriptableObject _data;
  public override ScriptableObject data => _data;

  private bool _attacking;
  public override bool attacking
  {
    get => _attacking;
    set => _attacking = value;
  }

  bool listeningForDamage = false;
  float lastHitTime = 0f;

  public GameObject rayStart;
  public GameObject rayEnd;

  public override void Draw()
  {
    Debug.Log("You drew your sword.");
    this.gameObject.SetActive(true);
  }
  public override void Withdraw()
  {
    Debug.Log("You withdrew your sword.");
    this.gameObject.SetActive(false);
  }

  private float attackTimeStart = 0;

  public override void Attack()
  {
    // Debug.Log("Player is attacking with the sword!!");
    attacking = true;
    StartAnimation();
    float animLength = PLAYERSingleton.i.animations.stateInfo.length;
    attackTimeStart = Time.time;
    listeningForDamage = true;
    Invoke("StopAttacking", PLAYERSingleton.i.animations.stateInfo.length);
  }

  Vector3 prevSwordPos;
  void listenForDamage()
  {
    // Debug.Log("The sword is listening for damage");
    RaycastHit hit;
    if (prevSwordPos == null) { prevSwordPos = rayEnd.transform.position; }

    // float raycastDistance = Vector3.Distance(PLAYERSingleton.i.cameraTargetGameobject.transform.position, rayEnd.transform.position);
    float raycastDistance = Vector3.Distance(PLAYERSingleton.i.cameraTargetGameobject.transform.position, prevSwordPos);
    prevSwordPos = rayEnd.transform.position;
    // bool rayHit = Physics.Raycast(rayStart.transform.position,
    //               rayStart.transform.up,
    //               out hit,
    //               raycastDistance);
    bool rayHit = Physics.SphereCast(rayStart.transform.position,
                  0.05f,
                  rayStart.transform.up,
                  out hit,
                  raycastDistance,
                  PLAYERSingleton.i.LayerMask_Attackable);
    
    float animLength = PLAYERSingleton.i.animations.stateInfo.length;
    // if (Time.time >= lastHitTime + animLength)
    if (Time.time >= lastHitTime + animLength && Time.time >= attackTimeStart + animLength / 2f)
    {
      if (rayHit)
      {
        if (hit.transform.CompareTag("Interactable"))
        {
          Interactable interactable = hit.transform.GetComponent<Interactable>();

          Debug.Log("The sword struck " + interactable._name);
          lastHitTime = Time.time;

          if (interactable.type == InteractionType.Enemy)
          {
            interactable.gameObject.GetComponent<I_Animal>().TakeDamage(10, PLAYERSingleton.i.gameObject);
          }
        }
      }
    }
  }

  void StopAttacking()
  {
    // Debug.Log("The sword attack has ended.");
    attacking = false;
    listeningForDamage = false;
    attackTimeStart = 0;
    PLAYERSingleton.i.playerIsAttacking = false;
  }

  void FixedUpdate()
  {
    if (listeningForDamage)
    {
      listenForDamage();
    }
  }

  public override void StartAnimation()
  {
    string id = "SlashTrigger";
    PLAYERSingleton.i.StartCoroutine(
        PLAYERSingleton.i.animations.PlayAndFreeze(PLAYERSingleton.i.animations.stateInfo.length, id)
    );
  }
  public override void StopAnimation()
  {

  }
}
