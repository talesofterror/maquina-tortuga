using System.Drawing;
using UnityEditor.ShaderGraph;

public enum InteractionType
{
  Friend,
  Enemy,
  Warp,
  Landmark
}

public enum PlayerMode
{
  Normal,
  Fight,
  Pause
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
  Retreat
}