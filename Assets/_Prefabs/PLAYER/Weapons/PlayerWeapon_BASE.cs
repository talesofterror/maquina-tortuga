using UnityEngine;

public abstract class PlayerWeapon_BASE : MonoBehaviour
{
    [Header("data")]
    public abstract ScriptableObject data {get; }

    public abstract void Draw();
    public abstract void Withdraw();
    public abstract void Attack();
    public abstract void Animate();

}
