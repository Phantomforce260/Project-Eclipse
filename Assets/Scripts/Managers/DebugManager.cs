using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManager : MonoBehaviour
{
    // This script is designed for testing features through button inputs, update, etc.
    private int delKeyCount = 5;

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.commaKey.wasPressedThisFrame)
        {
            delKeyCount++;
            if (delKeyCount > 5)
            {
                Debug.Log("Save Data removed");
                SaveManager.DeleteSave();
            }
        }

        if (Keyboard.current.periodKey.wasPressedThisFrame)
        {
            Debug.Log("key pressed");
            string mg = "Pizza";
            Debug.Log("Starting minigame: " + mg);
            UIManager.CreateMinigame(mg);
        }
    }
}
