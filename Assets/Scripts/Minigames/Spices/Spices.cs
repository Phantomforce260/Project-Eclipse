using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Spices : Minigame 
{
    public Button OKButton;

    public Scale scale;

    private void Awake() => Initialize();

    void Start()
    {
        OKButton.onClick.AddListener(() => Finish());
    }
}
