using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;
    public Rigidbody rigidbody;

    public bool IsGround = false;
    public float JumpPower = 1300f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGround)
        {
            rigidbody.AddForce(Vector3.up * JumpPower);
            Debug.Log("Jump!");
        }
        transform.localRotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            IsGround = true;
            Debug.Log($"OnCollisionEnter Ground : {IsGround}");
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGround = true;
            Debug.Log($"OnCollisionStay Ground : {IsGround}");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGround = false;
            Debug.Log($"OnCollisionStay Exit : {IsGround}");
        }
    }
}
