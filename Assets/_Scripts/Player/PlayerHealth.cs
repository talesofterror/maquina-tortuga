using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField]
    private int _maxHealth = 100;
    public int currentHealth { get; private set; }

    public int hp
    {
        get { return currentHealth; }
    }

    [Header("Invulnerability Settings")]
    [SerializeField]
    private float _invulnerabilityDuration = 1f;
    private bool _isInvulnerable = false;

    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = _maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (_isInvulnerable || IsDead)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, _maxHealth);

        UISingleton.i.debug.pushMessage(
            $"Player took {amount} damage. Current Health: {currentHealth}",
            "#cc1122"
        );

        UISingleton.i.RefreshUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    public void DamageKnockback(Vector3 source, float force)
    {
        Debug.Log("Player was knocked back!");
        StartCoroutine(KnockbackCoroutine(source, force));
    }

    IEnumerator KnockbackCoroutine(Vector3 source, float force)
    {
        PLAYERSingleton.i.freezeMovement = true;
        Vector3 direction = (source - PLAYERSingleton.i.rB.position);
        direction.y = 0;
        direction.Normalize();
        PLAYERSingleton.i.rB.AddForce(direction * -force, ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
        PLAYERSingleton.i.freezeMovement = false;
    }

    public void Heal(int amount)
    {
        if (IsDead)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, _maxHealth);
        UISingleton.i.debug.pushMessage(
            $"Player healed {amount}. Current Health: {currentHealth}",
            "#22cc22"
        );
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // TODO: Add Death Logic (UI, Scene Reload, Animation, etc.)
        // For now, we can maybe disable movement or trigger an animation if one exists
        // if (PLAYERSingleton.i != null)
        // {
        //     // Example: Disable movement
        //     PLAYERSingleton.i.movementEnabled = false;
        // }

        UISingleton.i.debug.pushMessage("Player Died!", "#cc1122");
        ResetHealth();
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        _isInvulnerable = true;
        // Optional: Visual feedback for invulnerability (blinking, etc.)

        yield return new WaitForSeconds(_invulnerabilityDuration);

        _isInvulnerable = false;
    }

    // Helper to fully restore health
    public void ResetHealth()
    {
        currentHealth = _maxHealth;
        _isInvulnerable = false;
        UISingleton.i.RefreshUI();
        UISingleton.i.debug.pushMessage("Player Health Reset!", "#22cc22");
    }
}
