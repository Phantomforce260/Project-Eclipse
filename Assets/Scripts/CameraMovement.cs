using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static bool FollowEnabled;

    public Transform Target;
    public Vector3 Offset;
    public float SmoothSpeed = 0.125f;

    public Bounds CameraBounds;

    private Vector3 currentPosition;

    public Vector3 CurrentPosition
    {
        get => currentPosition;
        set
        {
            Vector3 fixedPos = value;

            if (value.x > CameraBounds.MaxX)
                fixedPos = new Vector2(CameraBounds.MaxX, fixedPos.y);

            if (value.x < CameraBounds.MinX)
                fixedPos = new Vector2(CameraBounds.MinX, fixedPos.y);

            if (value.y > CameraBounds.MaxY)
                fixedPos = new Vector2(fixedPos.x, CameraBounds.MaxY);

            if (value.y < CameraBounds.MinY)
                fixedPos = new Vector2(fixedPos.x, CameraBounds.MinY);

            currentPosition = new Vector3(fixedPos.x, fixedPos.y, -10);
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
        CurrentPosition = new Vector3(
            Target.position.x + Offset.x,
            Target.position.y + Offset.y,
            -10
        );
        transform.position = CurrentPosition;
    }
}
