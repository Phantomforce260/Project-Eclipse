using UnityEngine;
using GemType = Gems.GemType;

public class Gem : MonoBehaviour
{
    public GemType type;
    public bool collected = false;

	private float easing = 0.99f;
	private RectTransform rectTransform;

    void Start() 
	{ 
		rectTransform = transform.GetComponent<RectTransform>();
	}

	void FixedUpdate()
	{
		rectTransform.localPosition *= easing;
	}

	public void SetMovement(Vector2 movement) {
		Debug.Log(movement);
		rectTransform.localPosition += new Vector3(movement.x, movement.y, 0f);
	}
}
