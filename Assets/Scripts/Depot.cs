using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Depot : MonoBehaviour
{
    public static Image[] PackageImages => instance.packageImages;

    private static Depot instance;

    public float DetectionRange = 3f;

    private bool inRange;

    public Image[] packageImages;

    private List<string> allMinigames;

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

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
            UIManager.JKeyHint.text = "Z - Receive Package";
            if (Keyboard.current.jKey.wasPressedThisFrame || Keyboard.current.zKey.wasPressedThisFrame)
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
            UpdatePackagesUI();
        }
        else
        {
            UIManager.SetNotif("Can't hold any more packages!");
        }
    }

    public static void UpdatePackagesUI()
    {
        for(int i = 0; i < 3; i++)
        {
            PackageImages[i].enabled = i < DepotController.PlayerInventory.Packages.Count;
        }       
    }
}
