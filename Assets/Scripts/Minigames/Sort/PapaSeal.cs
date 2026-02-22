using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class PapaSeal : MonoBehaviour
{
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            animator.Play("RaisePurple", 0, 0f);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            animator.Play("RaiseYellow", 0, 0f);
        }
    }
}
