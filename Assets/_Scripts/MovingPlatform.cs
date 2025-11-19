using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
  private Vector3 _previousPosition;
  private Vector3 _velocity;
  [HideInInspector] public Rigidbody rB;

  public float speed = 0.5f;
  public float distanceX;
  public float distanceY;
  public float distanceZ;
  private Vector3 initPosition;
  [Range(0, 10)] public float offset = 1;
  public bool randomOffset = false;
  public float intensifier = 1.07f;
  public Transform yFloor;
  public bool platformEnabled = true;
  void Awake()
  {
    rB = GetComponent<Rigidbody>();
  }

  void Start()
  {
    initPosition = transform.position;
    rB.position = rB.position;
    _previousPosition = rB.position;
    platformEnabled = true;
    if (randomOffset) offset = Random.Range(1, 10);
  }

  private void FixedUpdate()
  {

    // to normalize with distance: 
    //  scaledValue = (rawValue - min) / (max - min);

    if (platformEnabled)
    {
      float calculateX = distanceX * Mathf.Sin(Time.fixedTime - (Mathf.PI / offset));
      float calculateY = distanceY * Mathf.Sin(Time.fixedTime - (Mathf.PI / offset));
      float calculateZ = distanceZ * Mathf.Sin(Time.fixedTime - (Mathf.PI / offset));

      rB.MovePosition(new Vector3(
        rB.position.x + (calculateX * (speed * Time.deltaTime)),
         rB.position.y + (calculateY * (speed * Time.deltaTime)),
          rB.position.z + (calculateZ * (speed * Time.deltaTime))));

      _velocity = (rB.position - _previousPosition) / Time.fixedDeltaTime;
      _previousPosition = rB.position;
    }

    // Debug.Log(transform.name + " velocity: " + rB.linearVelocity);
  }

  // player script gets the platform's velocity from here
  public Vector3 CalculateVelocity()
  {
    // return new Vector3(_velocity.x * intensifier, _velocity.y, _velocity.z * intensifier);
    return new Vector3(_velocity.x, _velocity.y, _velocity.z);
    // return new Vector3(_velocity.x * intensifier, 0, _velocity.z * intensifier);
    // return _velocity; 
  }

}
