using System.Drawing;
using UnityEditor.ShaderGraph;

public enum InteractionType
{
  Friend,
  Enemy,
  Warp,
  Landmark
}

public enum WaypointSystemMode
{
  Loop,
  Bounce
}

public enum PlayerWeapon
{
  Sword,
  Gun
}

public enum EnemyMode
{
  Idle,
  Patrol,
  Alert,
  Pursue,
  Attack,
  Retreat,
  Die
}