using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Pizza : Minigame
{
    public PizzaSelectable[] toppings;

    public int selector = 0;

    public GameObject listContainer;
    private Image[] listImages;

    public int toppingCount;

    public PizzaSelectable[] chosen;

    private void Awake()
    {
        UIManager.SetNotif(
            "WASD to Move\nZ to Select",
            Color.black,
            20
        );
        Initialize();
        OnGameFinish.AddListener(() => {
            DepotController.PlayerInventory.Packages.Remove("Pizza");
            Depot.UpdatePackagesUI();
        });
        OnGameFinish.AddListener(() => Door.MinigameStarted = false);
    }

    void Start()
    {
        listImages = listContainer.GetComponentsInChildren<Image>();

        Restart();
    }

    void Restart()
    {
        for(int i = 0; i < toppings.Length; i++)
            toppings[i].Restart();
        
        chosen = new PizzaSelectable[toppings.Length];

        Array.Copy(toppings, chosen, toppings.Length);
        Shuffle(chosen);
        chosen = Take(chosen, toppingCount);
        
        for(int i = 0; i < toppingCount; i++)
        {
            listImages[i].sprite = chosen[i].listSprite;
            listImages[i].enabled = true;
        }
    }

    void Update()
    {
        toppings[selector].SetSelection(true);

        bool leftPressed = Keyboard.current.leftArrowKey.wasPressedThisFrame
            || Keyboard.current.downArrowKey.wasPressedThisFrame
            || Keyboard.current.aKey.wasPressedThisFrame
            || Keyboard.current.sKey.wasPressedThisFrame;

        bool rightPressed = Keyboard.current.rightArrowKey.wasPressedThisFrame
            || Keyboard.current.upArrowKey.wasPressedThisFrame
            || Keyboard.current.dKey.wasPressedThisFrame
            || Keyboard.current.wKey.wasPressedThisFrame;


        if(leftPressed || rightPressed)
        {
            AudioManager.PlaySFX("Placing" + UnityEngine.Random.Range(1, 4));
            toppings[selector].SetSelection(false);
            selector = Math.Clamp(selector + (rightPressed ? 1 : 0) + (leftPressed ? -1 : 0), 0, toppings.Length - 1);
        }

        bool selectPressed = Keyboard.current.jKey.wasPressedThisFrame 
            || Keyboard.current.zKey.wasPressedThisFrame;
        if(selectPressed)
        {
            bool foundTopping = false;
            for(int i = 0; i < chosen.Length; i++)
            {
                if (chosen[i].name == toppings[selector].name)
                {
                    foundTopping = true;
                    chosen[i].SetTopping(true);
                    AudioManager.PlaySFX(chosen[i].soundEffectName);
                    chosen = CopyWithout(chosen, i);
                    break;
                }
            }

            if(!foundTopping)
                Restart();

            if(chosen.Length == 0)
                Invoke(nameof(Finish), 1f);
        }
    }

    void Shuffle(PizzaSelectable[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1); // random index from 0 to i
            (array[i], array[j]) = (array[j], array[i]); // swap
        }
    }

    PizzaSelectable[] CopyWithout(PizzaSelectable[] array, int element)
    {
        PizzaSelectable[] output = new PizzaSelectable[array.Length - 1];
        for(int i = 0; i < array.Length; i++)
        {
            if(i != element)
            {
                output[i - (i >= element ? 1 : 0)] = array[i];
            }
        }
        return output;
    }

    PizzaSelectable[] Take(PizzaSelectable[] array, int count)
    {
        PizzaSelectable[] output = new PizzaSelectable[count];
        for(int i = 0; i < count; i++)
        {
            output[i] = array[i];
        }
        return output;
    }
}