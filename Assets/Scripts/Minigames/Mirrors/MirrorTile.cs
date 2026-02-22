using UnityEngine;

public class MirrorTile : MonoBehaviour
{
    private static float easing = 0.05f;

    public Vector2Int direction;
    public Vector2Int targetDirection;

    public RectTransform rectTransform;

    public bool isInPath;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        SeekTargetDirection();
    }

    public void SetDirection(Vector2Int dir)
    {
        direction = dir;
    }

    void SeekTargetDirection()
    {
        float targetRotation = DirectionToRotation(direction);
        float currentRotation = rectTransform.localEulerAngles.z;

        float diff = targetRotation - currentRotation;

        rectTransform.rotation = Quaternion.Euler(0f, 0f, currentRotation + diff * easing);
    }

    public static float DirectionToRotation(Vector2Int dir)
    {
        if(dir == Vector2Int.up) return 0f;
        if(dir == Vector2Int.left) return 90f;
        if(dir == Vector2Int.down) return 180f;
        if(dir == Vector2Int.right) return 180f + 90f; // Me when I don't want to do mental math
        return 0f;
    }
}
