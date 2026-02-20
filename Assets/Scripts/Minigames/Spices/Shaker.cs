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

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool pressedLeft = Keyboard.current.leftArrowKey.wasPressedThisFrame
            || Keyboard.current.aKey.wasPressedThisFrame;

        bool pressedRight = Keyboard.current.rightArrowKey.wasPressedThisFrame
            || Keyboard.current.dKey.wasPressedThisFrame;

		bool blowSpices = Keyboard.current.upArrowKey.isPressed 
			|| Keyboard.current.wKey.wasPressedThisFrame;
			

        // Shaker must be opposite of the inputted direction
        if ((pressedLeft && !shakerLeft) || (pressedRight && shakerLeft))
        {
            animator.Play(pressedLeft ? "ShakerLeft" : "ShakerRight");

            shakerLeft = pressedLeft;

            scale.SetRightWeight(scale.rightWeight + Random.Range(0f, flakesPerShake));
            shakerParticles.Play();
        }

		if (blowSpices) 
		{
			scale.SetRightWeight(Mathf.Clamp(scale.rightWeight -= Time.deltaTime * 50, 0, 45));
		}	
    }
}
