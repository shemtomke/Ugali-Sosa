using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class foodDestroy : MonoBehaviour
{
    public Rigidbody2D rb;

    public float CurrentSpeed;
    public float MaxSpeed = 4f;
    public float speed = 1f;

    private void Start()
    {
        CurrentSpeed = rb.gravityScale;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Nyama") || collision.CompareTag("Ugali") || collision.CompareTag("Mkorogo") || collision.CompareTag("Chillie"))
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        SpeedUp();
    }

    //as time goes by the food increase in speed

    public void SpeedUp()
    {
        CurrentSpeed += speed * Time.deltaTime;

        if(CurrentSpeed>MaxSpeed)
        {
            CurrentSpeed = MaxSpeed;
        }
    }
}
