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

    [SerializeField]
    private float _knockbackDuration = 1f;

    [Tooltip("0 = Start at 0 velocity. 1 = Start at Max velocity.")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _startKnockbackPercentage = 0f;

    public bool IsDead => currentHealth <= 0;

    private WaitForSeconds _invulnerabilityWait;
    private WaitForFixedUpdate _waitForFixedUpdate;
    private Coroutine _knockbackCoroutine;

    private void Awake()
    {
        currentHealth = _maxHealth;
        _waitForFixedUpdate = new WaitForFixedUpdate();
        _invulnerabilityWait = new WaitForSeconds(_invulnerabilityDuration);
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
        PLAYERSingleton.i.isTakingDamage = true;
        Debug.Log("Player was knocked back!");
        if (_knockbackCoroutine == null)
            _knockbackCoroutine = StartCoroutine(KnockbackCoroutine(source, force * 12));
    }

    IEnumerator KnockbackCoroutine(Vector3 source, float peakForce)
    {
        Vector3 direction = (PLAYERSingleton.i.rB.position - source);
        direction.y = 0;
        direction.Normalize();

        peakForce = peakForce / 20;
        Vector3 peakVelocity = direction * peakForce;

        // Calculate starting angle based on desired initial percentage
        // Asin(0) = 0, Asin(1) = PI/2
        float startAngle = Mathf.Asin(Mathf.Clamp01(_startKnockbackPercentage));
        float endAngle = Mathf.PI;

        float timer = 0f;
        while (timer < _knockbackDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / _knockbackDuration);

            // Interpolate angle from Start to PI
            float currentAngle = Mathf.Lerp(startAngle, endAngle, progress);

            // Calculate curve using Sin of the interpolated angle
            float curve = Mathf.Sin(currentAngle);

            if (PLAYERSingleton.i.vController != null)
            {
                PLAYERSingleton.i.vController.knockbackVelocity = peakVelocity * curve;
            }

            yield return null;
        }

        if (PLAYERSingleton.i.vController != null)
            PLAYERSingleton.i.vController.knockbackVelocity = Vector3.zero;

        PLAYERSingleton.i.isTakingDamage = false;
        _knockbackCoroutine = null;
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

        UISingleton.i.debug.pushMessage("Player Died!", "#cc1122");
        ResetHealth();
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        _isInvulnerable = true;

        yield return _invulnerabilityWait;

        _isInvulnerable = false;
    }

    public void ResetHealth()
    {
        currentHealth = _maxHealth;
        _isInvulnerable = false;
        UISingleton.i.RefreshUI();
        UISingleton.i.debug.pushMessage("Player Health Reset!", "#22cc22");
    }
}
