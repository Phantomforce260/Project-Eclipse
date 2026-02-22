using UnityEngine;
using GemType = Gems.GemType;

public class Gem : MonoBehaviour
{
    public GemType type;
    public bool collected = false;

	private Animator animator;
	public RectTransform rectTransform;
	public EaseToOrigin position;

    void Awake() 
	{ 
		rectTransform = GetComponent<RectTransform>();
		animator = GetComponent<Animator>();
		position = GetComponent<EaseToOrigin>();
	}

	public void Remove()
	{
		animator.Play("Gem");
	}
}
