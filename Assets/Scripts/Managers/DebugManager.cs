using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    // This script is designed for testing features through button inputs, update, etc.
    private PlayerInputActions inputActions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputActions = new PlayerInputActions();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.commaKey.wasPressedThisFrame)
        {
            AudioManager.PlaySFX("CrunchyTopping");
        }

        if (Keyboard.current.periodKey.wasPressedThisFrame)
        {
            Debug.Log("key pressed");
            string mg = "Mirrors";
            Debug.Log("Starting minigame: " + mg);
            UIManager.CreateMinigame(mg);
        }
    }
}
