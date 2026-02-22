using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum SealColor
{
    Purple,
    Yellow
}
public class Sort : Minigame
{
    public Slide slide;

    // visible seals
    public Image[] slots;

    // seal types
    public Sprite purpSeal;
    public Sprite yellSeal;

    // make list of seals
    public List<SealColor> queue = new List<SealColor>();
    public int numSeals = 20;
    int count = 0;

    private void Awake()
    {
        UIManager.SetNotif(
            "A | D to Sort",
            Color.black
        );
        Initialize();
        OnGameFinish.AddListener(() => DepotController.PlayerInventory.Packages.Remove("Sort"));
        OnGameFinish.AddListener(() => Door.MinigameStarted = false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SealColor newSeal;
        // generate seals for seal list 
        for (int i = 0; i < numSeals; i++)
        {
            newSeal = (Random.value > 0.5f) ? SealColor.Purple : SealColor.Yellow;
            queue.Add(newSeal);
        }
        Rerender();
    }

    // Update is called once per frame
    void Update()
    {
        // handle input
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (queue[0] == SealColor.Purple)
                slide.SlideLeft(purpSeal);
            else    
                slide.SlideLeft(yellSeal);
            Yoink(true);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (queue[0] == SealColor.Purple)
                slide.SlideRight(purpSeal);
            else    
                slide.SlideRight(yellSeal);
            Yoink(false);
        }
    }

    // remove seal
    public void Yoink(bool wentLeft)
    {
        SealColor seal = queue[0];
        queue.RemoveAt(0);
        if ((wentLeft && seal == SealColor.Purple) || (!wentLeft && seal == SealColor.Yellow)) // if wrong
        {
            AudioManager.PlaySFX("CorrectSeal");
            count++;
        }
        else
        {
            AudioManager.PlaySFX("WrongSeal");
            queue.Add(seal);
        }
        
        if (count == numSeals)
            Finish();

        Rerender();
    }

    // update render on seal removal
    void Rerender()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < queue.Count)
            {
                // Enable image
                slots[i].color = Color.white;

                if (queue[i] == SealColor.Purple)
                    slots[i].sprite = purpSeal;
                else
                    slots[i].sprite = yellSeal;
            }
            else
            {
                // Show blank
                slots[i].sprite = null;
                slots[i].color = new Color(1, 1, 1, 0); // fully transparent
            }
        }
    }

    void AnimateSlide()
    {
        
    }
}