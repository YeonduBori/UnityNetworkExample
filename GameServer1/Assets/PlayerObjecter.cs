using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjecter : MonoBehaviour
{
    public bool IsEnding = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("EndingPoint"))
        {
            IsEnding = true;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("EndingPoint"))
        {
            IsEnding = true;
        }
    }
}
