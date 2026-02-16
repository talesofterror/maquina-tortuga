using UnityEngine;

public class PlayerFightMode : MonoBehaviour
{
    public void Activate()
    {
        PLAYERSingleton.i.playerWeapons.weapon.SetActive(true);
        PLAYERSingleton.i.playerMode = PlayerMode.Fight;
        PLAYERSingleton.i.animations.animator.SetBool(
            PLAYERSingleton.i.animations.FightStance,
            true
        );
        UISingleton.i.debug.pushMessage("** Fight Mode activated!");
    }

    public void Deactivate(PlayerMode mode)
    {
        PLAYERSingleton.i.playerWeapons.weapon.SetActive(false);
        PLAYERSingleton.i.playerMode = mode;
        PLAYERSingleton.i.animations.animator.SetBool(
            PLAYERSingleton.i.animations.FightStance,
            false
        );
        UISingleton.i.debug.pushMessage("** Normal Mode activated.");
    }
}
