using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodDestroy : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Nyama") || collision.CompareTag("Ugali") || collision.CompareTag("Mkorogo") || collision.CompareTag("Chillie"))
        {
            Destroy(gameObject);
        }

    }
}
