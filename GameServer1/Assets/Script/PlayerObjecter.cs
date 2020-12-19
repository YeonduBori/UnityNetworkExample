using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjecter : MonoBehaviour
{
    public bool IsEnding = false;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("EndingPoint"))
        {
            IsEnding = true;
            Debug.Log($"IsEnding? : {IsEnding}");
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("EndingPoint"))
        {
            IsEnding = true;
            Debug.Log($"IsEnding? : {IsEnding}");
        }
    }
}
