using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodDestroy : MonoBehaviour
{
    public Rigidbody2D rb;
    public int level;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Nyama") || collision.CompareTag("Ugali") || collision.CompareTag("Mkorogo") || collision.CompareTag("Chillie"))
        {
            Destroy(gameObject);
        }

    }

    private void FixedUpdate()
    {
        
    }

    public void SpeedUp()
    {
        level = PlayerPrefs.GetInt("level");

        switch (level)
        {
            case 1:
                rb.gravityScale = rb.gravityScale + 0.05f;
                break;
            case 2:
                rb.gravityScale = rb.gravityScale + 0.1f;
                break;
            case 3:
                rb.gravityScale = rb.gravityScale + 0.15f;
                break;
            case 4:
                rb.gravityScale = rb.gravityScale + 0.2f;
                break;
            case 5:
                rb.gravityScale = rb.gravityScale + 0.25f;
                break;

                //proceed to endless
        }

    }
}
