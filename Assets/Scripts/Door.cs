using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public string MinigameName;

    private bool minigameStarted;


    private void Awake()
    {
    }

    private void Update()
    {
        if (PlayerInRange())
        {
            if (!minigameStarted && Keyboard.current.jKey.wasPressedThisFrame)
            {
                Debug.Log("Starting Minigame: " + MinigameName);
                minigameStarted = true;
                UIManager.CreateMinigame(MinigameName);
            }
        }
    }

    private bool PlayerInRange() => Vector3.Distance(transform.position, DepotController.Position) < 1f;
}
