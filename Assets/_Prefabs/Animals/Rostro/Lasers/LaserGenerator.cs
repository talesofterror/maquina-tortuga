using System.Collections;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;

public class LaserGenerator : MonoBehaviour
{
  public float extendSpeed = 1f;
  public float retractSpeed = 1f;

  public bool isExtended = false;

  public float laserScale = 0.3f;
  public float laserLength = 10f;

  public Transform[] laserTransforms;
  GameObject[] laserObjects;

  void Start()
  {
    // laserTransforms = GetComponentsInChildren<Transform>();

    laserObjects = new GameObject[laserTransforms.Length];

    for (int i = 0; i < laserTransforms.Length; i++)
    {
      Debug.Log("Laser Transform: " + laserTransforms[i].name + " Position: " + laserTransforms[i].position);
      laserObjects[i] = GMSingleton.i.prefabManager.InstantiatePrefab("Laser Mesh", laserTransforms[i].position, laserTransforms[i].rotation);
      laserObjects[i].transform.parent = laserTransforms[i];
      laserObjects[i].SetActive(false);
      laserObjects[i].transform.localScale = new Vector3(0, 0, 0); // Start with scale 0 in Y
      Debug.Log("Laser Object: " + laserObjects[i].name + " Position: " + laserObjects[i].transform.position + " Scale: " + laserObjects[i].transform.localScale);
    }
  }

  public IEnumerator ExtendLaser()
  {
    for (int i = 0; i < laserObjects.Length; i++)
    {
      laserObjects[i].SetActive(true);
      for (float t = 0; t < 1; t += Time.deltaTime * extendSpeed)
      {
        float scaleZ = Mathf.Lerp(0, laserLength, t);
        laserObjects[i].transform.localScale = new Vector3(laserScale, laserScale, scaleZ);
        laserObjects[i].transform.localPosition = new Vector3(0, 0, scaleZ / 2); // Adjust position to extend from the base
        yield return null;
      }
    }
    // yield return null;
  }

  public IEnumerator RetractLaser()
  {
    for (int i = 0; i < laserObjects.Length; i++)
    {
      for (float t = 0; t < 1; t += Time.deltaTime * retractSpeed)
      {
        float scaleZ = Mathf.Lerp(laserLength, 0, t);
        laserObjects[i].transform.localScale = new Vector3(laserScale, laserScale, scaleZ);
        laserObjects[i].transform.localPosition = new Vector3(0, 0, scaleZ / 2); // Adjust position to retract towards the base
        yield return null;
      }
      laserObjects[i].SetActive(false);
    }
    // yield return null;
  }
}
