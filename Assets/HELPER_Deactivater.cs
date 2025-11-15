using UnityEngine;

public class HELPER_Deactivater : MonoBehaviour
{
  void Awake()
  {
    if (CAMERASingleton.i.primaryFreelook)
    {
      Destroy(gameObject);
    }
  }
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
