using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spices : Minigame 
{
    public Button OKButton;

    // public WeightedItem[] weightedItems;

    private void Awake()
    {
        Initialize();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OKButton.onClick.AddListener(() => Finish());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
