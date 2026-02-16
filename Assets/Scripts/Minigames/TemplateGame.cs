using UnityEngine;
using UnityEngine.UI;

public class TemplateGame : Minigame 
{
    public Button OKButton;

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
