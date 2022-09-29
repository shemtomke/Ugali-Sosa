using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 40f;
    gameManager game_Manager;
    player _player;

    // Start is called before the first frame update
    void Start()
    {
        game_Manager = GameObject.Find("GameManager").GetComponent<gameManager>();
        _player = GameObject.Find("Level_Player").GetComponent<player>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * Time.deltaTime * speed);

        //if is game over
        if(_player.isGameOver)
        {
            Destroy(gameObject);
        }
    }

    //increase their speed in every level
}