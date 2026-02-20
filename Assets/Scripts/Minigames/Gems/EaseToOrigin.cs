using UnityEngine;

public class EaseToOrigin : MonoBehaviour
{
    private RectTransform rectTransform;
    public float easing = 0.05f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void FixedUpdate()
    {
        rectTransform.localPosition *= easing;       
    }
}
