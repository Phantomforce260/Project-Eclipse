using UnityEngine.UI;
using UnityEngine;

public class PizzaSelectable : MonoBehaviour
{
    public RectTransform rectTransform;
    public Image selection;
    public Sprite listSprite;

    public GameObject topping;
    public string soundEffectName;

    void Start()
    {
        Restart();
    }

    public void Restart()
    {
        topping.SetActive(false);
        selection.enabled = false;
    }

    public void SetSelection(bool status)
    {
        selection.enabled = status;
    }

    public void SetTopping(bool status)
    {
        topping.SetActive(status);
        topping.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }
}
