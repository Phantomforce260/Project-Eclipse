using UnityEngine;
using UnityEngine.InputSystem;

public class Scale : MonoBehaviour
{
    public RectTransform rectTransform;

    public float leftWeight;
    public float rightWeight;

    public float scaleWidth;
    public float inertia;
    public float damping = 1.0f;
    public float gravity;

    private float velocity = 0.0f;

    void Start()
    {
        
    }

    void Update()
    {
        float angle = rectTransform.rotation.eulerAngles.z * Mathf.Deg2Rad;

        // Torque from mass difference acting at half the scale width; cos(angle) projects the lever arm
        float torque = (rightWeight - leftWeight) 
            * gravity 
            * (scaleWidth / 2f) 
            * Mathf.Cos(angle);

        // Integrate angular acceleration -> angular velocity (rad/s)
        float acceleration = torque / inertia;
        velocity += acceleration * Time.deltaTime;
        angle += velocity * Time.deltaTime;

        float newAngleDeg = Mathf.Clamp(angle * Mathf.Rad2Deg, -45f, 45f);
        rectTransform.rotation = Quaternion.Euler(0f, 0f, newAngleDeg);
    }
}