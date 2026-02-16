using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Period))
        {
            UIManager.CreateMinigame("TemplateGame");
        }
    }
}
