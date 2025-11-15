using UnityEngine;

public interface I_Animal
{
  public int hp { get; set; }
  public int ap { get; set; }
  public int mp { get; set; }
  public void Walk();
  public void TakeDamage();

}
