using UnityEngine;
using UnityEngine.UI;

public class Slide : MonoBehaviour
{
    private Animator anim;
    private Image img;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        img = GetComponent<Image>();
    }

    public void SlideLeft(Sprite seal)
    {
        img.sprite = seal;
        anim.Play("SlideLeft", 0, 0);
    }

    public void SlideRight(Sprite seal)
    {
        img.sprite = seal;
        anim.Play("SlideRight", 0, 0);
    }
}
