using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shaker : MonoBehaviour
{
    public RectTransform rectTransform;
    public Spices spices;

    public ParticleSystem shakerParticles;

    public float amplitude;
    public float flakesPerShake;
    private bool shakerLeft = false;

    public Keycap leftKeycap;
    public Keycap rightKeycap;
    private int keycapPresses = 0;

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
			|| Keyboard.current.wKey.isPressed;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame
			|| Keyboard.current.wKey.wasPressedThisFrame)
        {
            AudioManager.PlaySFX("Blowing");
        }

        // Shaker must be opposite of the inputted direction
        if ((pressedLeft && !shakerLeft) || (pressedRight && shakerLeft))
        {
            animator.Play(pressedLeft ? "ShakerLeft" : "ShakerRight");
            
            if(pressedLeft)
                AudioManager.PlaySFX("Shaker1");
            else
                AudioManager.PlaySFX("Shaker2");


            if (keycapPresses < 10)
            {
                leftKeycap.SetPressed(pressedLeft);
                rightKeycap.SetPressed(pressedRight);
            }
            else
            {
                leftKeycap.SetPressed(true);
                rightKeycap.SetPressed(true);
            }
            keycapPresses++;

            shakerLeft = pressedLeft;

            spices.SetRightWeight(spices.rightWeight + Random.Range(0f, flakesPerShake));
            shakerParticles.Play();
        }

		if (blowSpices) 
		{
			spices.SetRightWeight(Mathf.Clamp(spices.rightWeight -= Time.deltaTime * 50, 0, 45));
		}	
    }
}
