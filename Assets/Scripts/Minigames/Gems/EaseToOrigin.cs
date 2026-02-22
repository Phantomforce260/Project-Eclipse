using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class EaseToOrigin : MonoBehaviour
{
    private RectTransform rectTransform;
    public float easing = 0.95f;
    public Vector3 targetPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        easing = 0.90f;
}

    void FixedUpdate()
    {
        rectTransform.localPosition += easing * (targetPosition - rectTransform.localPosition);
    }

	public void SetMovement(Vector2 movement)
    {
        targetPosition += (Vector3)movement;
    }

	public void SetPosition(Vector2 position)
	{
        targetPosition = position;
	}
}
