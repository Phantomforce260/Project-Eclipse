using UnityEngine;
using UnityEngine.InputSystem;

public class Shaker : MonoBehaviour
{
    public RectTransform rectTransform;
    public Scale scale;

    public ParticleSystem shakerParticles;

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

        // Shaker must be opposite of the inputted direction
        if ((pressedLeft && !shakerLeft) || (pressedRight && shakerLeft))
        {
            shakerLeft = pressedLeft;
            rectTransform.localPosition += new Vector3(amplitude * (pressedLeft ? -1 : 1), 0f, 0f); // TODO: Change this to a nice animation

            scale.SetRightWeight(scale.rightWeight + Random.Range(0f, flakesPerShake));
            shakerParticles.Play();
        }
    }
}
