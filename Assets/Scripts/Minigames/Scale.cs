using UnityEngine;
using UnityEngine.InputSystem;

public class Scale : MonoBehaviour
{
    public RectTransform rectTransform;

    [Range(0.0f, 45.0f)]
    public float leftWeight;

    [Range(0.0f, 45.0f)]
    public float rightWeight;

    public float scaleWidth;
    public float inertia;
    public float damping;

    private float velocity = 0.0f;

    void Start() 
	{

	}

    void Update()
    {
        // float desiredAngle = leftWeight - rightWeight;
        // float difference = desiredAngle - rectTransform.localEulerAngle.y;

        // Integrate angular acceleration -> angular velocity (rad/s)
        // float acceleration = difference;
        // velocity += acceleration * Time.deltaTime;
        // angle += velocity * Time.deltaTime;
        //
        // float newAngleDeg = Mathf.Clamp(angle * Mathf.Rad2Deg, -45f, 45f);
        // rectTransform.rotation = Quaternion.Euler(0f, 0f, newAngleDeg);
    }
}
