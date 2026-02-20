using UnityEditor.Rendering;
using UnityEngine;
using GemType = Gems.GemType;

public class Gem : MonoBehaviour
{
    public GemType type;
    public bool collected = false;

	private float easing = 0.99f;
	private RectTransform rectTransform;
	private Animator animator;

    void Start() 
	{ 
		rectTransform = GetComponent<RectTransform>();
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		rectTransform.localPosition *= easing;
		rectTransform.localScale = Vector2.one;
	}

	public void SetMovement(Vector2 movement) {
		Debug.Log(movement);
		rectTransform.localPosition += new Vector3(movement.x, movement.y, 0f);
	}

	public void Remove()
	{
		animator.Play("Gem");
	}
}
