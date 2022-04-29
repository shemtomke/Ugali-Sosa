using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class player : MonoBehaviour
{
    public Text scoretxt;
    //reach this score to proceed to next level
    public Text maxScoreTxt;


    public int maxScore, minPoint, maxPoint; //increase for new levels
    public int score = 1;
    public float speed;
    public float xBound;
    private Vector3 direction;
    private Vector3 touchPosition;

    public Slider slider;
    public RectTransform fill;

    public bool isGameOver;

    //move the player
    public bool isMove;

    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        //new Score
        maxScore = Random.Range(minPoint, maxPoint);

        score = 1;
        Vector3 pos = fill.position;

        isMove = true;
        rb = GetComponent<Rigidbody2D>();

        slider.maxValue = maxScore;
        maxScoreTxt.text = "" + maxScore;
    }

    // Update is called once per frame
    void Update()
    {
        ScoreDeath();
        Movement();
        slider.value = score;
    }

    public void Movement()
    {
        if(isMove)
        {
            //touch point for android devices
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                touchPosition.z = 0;
                direction = (touchPosition - transform.position);
                rb.velocity = new Vector2(direction.x, direction.y) * speed;
                Debug.Log("touch");

                if (touch.phase == TouchPhase.Ended)
                    rb.velocity = Vector2.zero;
                Debug.Log("ended");
            }
        }
        

        if(isMove)
        {
            //wasd/arrow keys for standalone
            float hmove = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            transform.Translate(Vector3.right * hmove, Space.World);
        }
        

        //make it bound to something //not make it go past the screen
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -xBound, xBound), transform.position.y);
    }

    //how to acquire or get certain foods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ugali 1 point
        if (collision.CompareTag("Ugali"))
        {
            score++;
            scoretxt.text = "" + score;

            Destroy(collision.gameObject);
        }
        //nyama +5
        else if(collision.CompareTag("Nyama"))
        {
            score= score + 5;
            scoretxt.text = "" + score;

            Destroy(collision.gameObject);
        }
        //chillie -1
        else if(collision.CompareTag("Chillie"))
        {
            score = score - 1;
            scoretxt.text = "" + score;

            Destroy(collision.gameObject);
        }
        //mkorogo - instant kill - game over
        else
        {
            isGameOver = true;
        }
    }

    //score less than 1 then die
    public void ScoreDeath()
    {
        if(score < 1)
        {
            isGameOver = true;
        }
    }
    //next level increase/random range the max amount of score
}
