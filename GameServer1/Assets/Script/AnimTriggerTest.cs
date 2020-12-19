using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimTriggerTest : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Space Press!");
            animator.SetBool("IsMove", true);
        }
        else
        {
            Debug.Log("Idle");
            animator.SetBool("IsMove", false);
        }
    }
}
