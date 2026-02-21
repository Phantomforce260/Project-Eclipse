using UnityEngine;
using TMPro;

public class Keycap : MonoBehaviour
{
	private RectTransform rectTransform;
	private Animator animator;
	private TMP_Text textMeshPro;

	public bool pressed;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
		animator = GetComponent<Animator>();
		textMeshPro = GetComponentInChildren<TMP_Text>();

		animator.Play(pressed ? "Press" : "Unpress");
    }

	public void SetPressed(bool status)
	{
		if(pressed != status)
		{
			animator.Play(status ? "Press" : "Unpress");
		}
		pressed = status;
	}
}
