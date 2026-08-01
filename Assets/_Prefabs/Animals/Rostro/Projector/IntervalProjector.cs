using System.Collections;
using UnityEngine;

public class IntervalProjector : MonoBehaviour
{
  public int poolSize = 10;
  public float scale;
  private GameObject[] projectileArray;
  public float intervalOffset = 0f;
  public float projectileDistance = 15f;
  public float projectileTravelTime = 0.5f;
  public float projectileSpeed = 20f;
  public int projectileDamage = 10;
  private int nextProjectileIndex = 0;
  public string PrefabName;

  void Start()
  {
    projectileArray = new GameObject[poolSize];
    // for (int i = 0; i < poolSize; i++)
    // {
    //   projectileArray[i] = GMSingleton.i.prefabManager.InstantiatePrefab(PrefabName, transform.position, transform.rotation);
    //   projectileArray[i].transform.parent = null;
    //   projectileArray[i].SetActive(false);
    //   projectileArray[i].transform.localScale = new Vector3(scale, scale, scale);
    //   projectileArray[i].GetComponent<ProjectileDamageListener>().damage = projectileDamage;
    // }
  }

  public void InitProjectiles(int damage, float force)
  {
    projectileArray = new GameObject[poolSize];
    for (int i = 0; i < poolSize; i++)
    {
      projectileArray[i] = GMSingleton.i.prefabManager.InstantiatePrefab(PrefabName, transform.position, transform.rotation);
      projectileArray[i].transform.parent = null;
      projectileArray[i].SetActive(false);
      projectileArray[i].transform.localScale = new Vector3(scale, scale, scale);
      ProjectileDamageListener damageListener = projectileArray[i].GetComponentInChildren<ProjectileDamageListener>();
      damageListener.damage = damage;
      damageListener.force = force;
    }
  }

  public void FireProjectile()
  {
    if (projectileArray == null || projectileArray.Length == 0)
    {
      return;
    }

    GameObject availableProjectile = null;
    for (int i = 0; i < projectileArray.Length; i++)
    {
      int index = (nextProjectileIndex + i) % projectileArray.Length;
      GameObject candidate = projectileArray[index];

      if (candidate != null && !candidate.activeSelf)
      {
        availableProjectile = candidate;
        nextProjectileIndex = (index + 1) % projectileArray.Length;
        break;
      }
    }

    if (availableProjectile == null)
    {
      return;
    }

    availableProjectile.transform.position = transform.position;
    availableProjectile.transform.rotation = transform.rotation;
    availableProjectile.SetActive(true);

    StartCoroutine(TravelProjectile(availableProjectile));
  }

  private IEnumerator TravelProjectile(GameObject activeProjectile)
  {
    Vector3 startPosition = activeProjectile.transform.position;
    Vector3 direction = transform.forward.normalized;
    Vector3 targetPosition = startPosition + (direction * projectileDistance);

    float distance = Vector3.Distance(startPosition, targetPosition);
    float travelTime = distance / Mathf.Max(projectileSpeed, 0.0001f);

    float elapsed = 0f;

    while (elapsed < travelTime)
    {
      elapsed += Time.deltaTime;
      float t = Mathf.Clamp01(elapsed / travelTime);
      activeProjectile.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
      yield return null;
    }

    activeProjectile.transform.position = targetPosition;
    activeProjectile.SetActive(false);
  }
}
