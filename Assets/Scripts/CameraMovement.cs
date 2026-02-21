using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static bool FollowEnabled;

    public Transform Target;
    public Vector3 Offset;
    public float SmoothSpeed = 0.125f;

    public Bounds CameraBounds;

    private Vector2 currentPosition;

    public Vector2 CurrentPosition
    {
        get => currentPosition;
        set
        {
            Vector2 fixedPos = value;

            if (value.x > CameraBounds.MaxX)
                fixedPos = new Vector2(CameraBounds.MaxX, fixedPos.y);

            if (value.x < CameraBounds.MinX)
                fixedPos = new Vector2(CameraBounds.MinX, fixedPos.y);

            if (value.y > CameraBounds.MaxY)
                fixedPos = new Vector2(fixedPos.x, CameraBounds.MaxY);

            if (value.y < CameraBounds.MinY)
                fixedPos = new Vector2(fixedPos.x, CameraBounds.MinY);

            currentPosition = fixedPos;
        }
    }


    [Serializable]
    public struct Bounds
    {
        public float MaxX;
        public float MaxY;
        public float MinX;
        public float MinY;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = Target.position + Offset;
        CurrentPosition = Vector3.Lerp(transform.position, desiredPosition, SmoothSpeed * Time.deltaTime);
        transform.position = new Vector3(CurrentPosition.x, CurrentPosition.y, -10);
    }
}
