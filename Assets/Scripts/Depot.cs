using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class Depot : MonoBehaviour
{
    public float DetectionRange = 3f;

    private bool inRange;

    private List<string> allMinigames;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Door.DoorRanges.Add(() => inRange);
        allMinigames = UIManager.GetAllMinigames();
    }

    // Update is called once per frame
    void Update()
    {
        inRange = PlayerInRange();
        if (inRange)
        {
            UIManager.JKeyHint.text = "J - Receive Package";
            if (Keyboard.current.jKey.wasPressedThisFrame)
            {

                AssignPackage();
            }
        }
    }

    private bool PlayerInRange() => Vector3.Distance(transform.position, DepotController.Position) < DetectionRange;

    public void AssignPackage()
    {
        if (DepotController.PlayerInventory.Packages.Count < 3)
        {
            UIManager.SetNotif("Package Received!");
            DepotController.PlayerInventory.Packages.Add(allMinigames[Random.Range(0, allMinigames.Count)]);
        }
        else
        {
            UIManager.SetNotif("Can't hold any more packages!");
        }
    }
}
