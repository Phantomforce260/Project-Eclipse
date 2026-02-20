using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System;

public class Scale : MonoBehaviour
{
    public RectTransform rectTransform;

    public Sprite[] spicePileSprites;
    public Image spicePile;

    [Range(0.0f, 45.0f)]
    public float leftWeight;

    [Range(0.0f, 45.0f)]
    public float rightWeight;

    public float accelerationCoefficient;
    public float damping;

    private float velocity = 0.0f;

    public float spiceTimerLength;
    public float spiceTolerance;

    void Start()
    {
        leftWeight = UnityEngine.Random.Range(25f, 45f);
    }

    void Update()
    {
        float angle = rectTransform.localEulerAngles.z;
        if(angle > 180f) angle -= 360.0f;

        float desiredAngle = leftWeight - rightWeight;
        float difference = desiredAngle - angle;
        float acceleration = difference * accelerationCoefficient;

        velocity += acceleration;
        velocity *= damping;

        float newAngleDeg = Mathf.Clamp(angle + velocity * Time.deltaTime, -45f, 45f);
        rectTransform.localEulerAngles = new Vector3(0f, 0f, newAngleDeg);
    }

    public void SetRightWeight(float value)
    {
        if (value == 0)
        {
            spicePile.enabled = false;
            spicePile.sprite = null;
        }
        else if (value <= 11)
        {
            spicePile.enabled = true;
            spicePile.sprite = spicePileSprites[0];
        }
        else if (value <= 22)
        {
            spicePile.enabled = true;
            spicePile.sprite = spicePileSprites[1];
        }
        else if (value <= 33)
        {
            spicePile.enabled = true;
            spicePile.sprite = spicePileSprites[2];
        }
        else
        {
            spicePile.enabled = true;
            spicePile.sprite = spicePileSprites[3];
        }

        spicePile.SetNativeSize();
        rightWeight = value;

        if (Math.Abs(leftWeight - rightWeight) < spiceTolerance)
            StartCoroutine(SpiceTimer());
    }

    IEnumerator SpiceTimer()
    {
        float timer = spiceTimerLength;
        float initialWeight = rightWeight;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (initialWeight == rightWeight)
        {
            Debug.Log("Spices WON");
        }
    } 
}