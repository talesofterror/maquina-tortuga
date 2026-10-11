using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.ProBuilder.MeshOperations;
using Invector.vCharacterController;

public class FSM_PlayerState_Sliding : FSM_PlayerStateBase
{

    public FSM_PlayerState_Sliding(FSM_PlayerStateController c) : base(c) { }

    [Header("Slide Settings")]
    [SerializeField] private float slideDistance = 3f;
    [SerializeField] private float slideDuration = 0.4f;
    [SerializeField] private float slideCooldown = 1.5f;

    private vThirdPersonController cc;
    private bool isSliding;
    private float nextSlideTime;

    public override void Enter()
    {
        cc = controller.gameObject.GetComponent<vThirdPersonController>();
        Debug.Log("Slide initiated");
        controller.StartCoroutine(PerformSlide());
        PLAYERSingleton.i.playerHealth._isInvulnerable = true;
        PLAYERSingleton.i.playerHealth.canBeKnockedback = false;
    }

    public override void Loop()
    {
    }

    public override void Exit()
    {
        PLAYERSingleton.i.animations.animator.SetBool(PLAYERSingleton.i.animations.Sliding, false);
        controller.cachedState = null;
        PLAYERSingleton.i.playerHealth._isInvulnerable = false;
        PLAYERSingleton.i.playerHealth.canBeKnockedback = true;
    }

    private bool CanSlide()
    {
        // Don't slide if already sliding, on cooldown, or in ragdoll/custom action
        return !isSliding && Time.time >= nextSlideTime && cc.isGrounded;
    }

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;

    private IEnumerator PerformSlide()
    {
        isSliding = true;
        nextSlideTime = Time.time + slideCooldown;

        cc.lockMovement = true;
        cc.lockRotation = true;

        yield return new WaitForFixedUpdate();

        Vector3 slideDir = cc.moveDirection.normalized;
        if (slideDir == Vector3.zero)
        {
            slideDir = PLAYERSingleton.i.transform.forward;
        }

        if (PLAYERSingleton.i.slideParticles != null)
        {
            for (int i = 0; i < PLAYERSingleton.i.slideParticles.Length; i++)
            {
                PLAYERSingleton.i.slideParticles[i].Play();
            }
        }


        PLAYERSingleton.i.animations.animator.SetBool(PLAYERSingleton.i.animations.Sliding, true);

        float elapsed = 0f;
        float speed = slideDistance / slideDuration;

        while (elapsed < slideDuration)
        {


            Vector3 rayOrigin = cc._rigidbody.position + Vector3.up * 0.5f;
            Vector3 adjustedSlideDir = slideDir;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 1.5f, PLAYERSingleton.i.vController.groundLayer))
            {
                adjustedSlideDir = Vector3.ProjectOnPlane(slideDir, hit.normal).normalized;
            }

            float moveDistance = speed * Time.fixedDeltaTime;
            Vector3 targetPos = cc._rigidbody.position + adjustedSlideDir * moveDistance;
            Vector3 movementDirection = (targetPos - cc._rigidbody.position).normalized;
            if (cc._rigidbody.SweepTest(adjustedSlideDir, out _, moveDistance, QueryTriggerInteraction.Collide)
                || Physics.Raycast(rayOrigin, movementDirection, moveDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
            {
                break;
            }

            cc._rigidbody.MovePosition(targetPos);

            elapsed += Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (PLAYERSingleton.i.slideParticles != null)
        {
            for (int i = 0; i < PLAYERSingleton.i.slideParticles.Length; i++)
            {
                PLAYERSingleton.i.slideParticles[i].Stop();
            }
        }

        isSliding = false;
        cc.lockMovement = false;
        cc.lockRotation = false;
        PLAYERSingleton.i.animations.animator.SetBool(PLAYERSingleton.i.animations.Sliding, false);
        controller.SwitchState(controller.cachedState ?? controller.cachedState);
    }

}
