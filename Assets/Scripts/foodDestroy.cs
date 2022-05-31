using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodDestroy : MonoBehaviour
{
    public GameObject floatingTxt;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void showFloatingText()
    {
        Instantiate(floatingTxt, transform.position, Quaternion.identity, transform);
    }
}
