using UnityEngine;
using TMPro;

public class Keycap : MonoBehaviour
{
	private RectTransform rectTransform;
	private Animator animator;
	private Anim
	private TMP_Text textMeshPro;

	public string contents;
	public bool pressed;

    void Start()
    {
        rectTransfor = GetComponent<RectTransform>();
		animator = GetComponent<Animator>();
		textMeshPro = GetComponent<TextMeshPro>();

		textMeshPro.text = contents;
    }

	public void SetPressed(bool status)
	{
		if(status != pressed)
		{
			pressed = status;
		}
		animator.SetBool("Pressed", pressed)
	}
}
