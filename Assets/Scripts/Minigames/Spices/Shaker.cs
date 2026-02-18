using UnityEngine;
using UnityEngine.InputSystem;

public class Shaker : MonoBehaviour
{
    public RectTransform rectTransform;
    public Scale scale;

    public float amplitude;
    public float flakesPerShake;
    private bool shakerLeft = false;

    void Start() { }

    void Update()
    {
        bool pressedLeft = Keyboard.current.leftArrowKey.wasPressedThisFrame
            || Keyboard.current.aKey.wasPressedThisFrame;

        bool pressedRight = Keyboard.current.rightArrowKey.wasPressedThisFrame
            || Keyboard.current.dKey.wasPressedThisFrame;

        // TODO: Have this animate instead of snapping
        if (pressedLeft && !shakerLeft)
        {
            shakerLeft = true;
            rectTransform.localPosition += new Vector3(-amplitude, 0f, 0f);
            scale.SetRightWeight(scale.rightWeight + Random.Range(0f, flakesPerShake));
        }
        else if (pressedRight && shakerLeft)
        {
            shakerLeft = false;
            rectTransform.localPosition += new Vector3(amplitude, 0f, 0f);
            scale.SetRightWeight(scale.rightWeight + Random.Range(0f, flakesPerShake));
        }
    }
}
