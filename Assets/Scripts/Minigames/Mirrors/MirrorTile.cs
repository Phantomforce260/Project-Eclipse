using UnityEngine;

public class MirrorTile : MonoBehaviour
{
    private static float easing = 0.05f;

    public Vector2Int direction = Vector2Int.up;

    public RectTransform rectTransform;

    public bool isInPath;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        for(int i = 0; i < UnityEngine.Random.Range(0, 4); i++)
        {
            RotateClockwise(true);
        }
    }

    void Update()
    {
        SeekTargetDirection();
    }

    public void SetDirection(Vector2Int dir)
    {
        direction = dir;
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, DirectionToRotation(dir));
    }

    public Vector2Int GetReflection(Vector2Int dir)
    {
        Vector2Int u = Vector2Int.up;
        Vector2Int l = Vector2Int.left;
        Vector2Int r = Vector2Int.right;
        Vector2Int d = Vector2Int.down;

        if (direction == u)
        {
            if(dir == l) return u;
            if(dir == u) return l;
        }
        else if (direction == r)
        {
            if(dir == u) return r;    
            if(dir == r) return u;
        }
        else if (direction == d)
        {
            if(dir == r) return d;
            if(dir == d) return r;
        }
        else if (direction == l)
        {
            if (dir == l) return d;
            if (dir == d) return l;
        }
        return Vector2Int.zero;
    }

    void SeekTargetDirection()
    {
        float current = rectTransform.localEulerAngles.z;
        float target = DirectionToRotation(direction);

        float newRotation = Mathf.LerpAngle(current, target, easing);
        rectTransform.localRotation = Quaternion.Euler(0f, 0f, newRotation);
    }

    public void RotateClockwise(bool c)
    {
        if(direction == Vector2Int.up) 
            direction = (c ? Vector2Int.right : Vector2Int.left);
        else if(direction == Vector2Int.right)
            direction = (c ? Vector2Int.down : Vector2Int.up);
        else if(direction == Vector2Int.down)
             direction = (c ? Vector2Int.left : Vector2Int.right);
        else if(direction == Vector2Int.left)
             direction = (c ? Vector2Int.up: Vector2Int.down);
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
