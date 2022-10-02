using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTxt : MonoBehaviour
{
    public float destroyTime = 3f;
    public Vector3 offset = new Vector3(0, 2, 0);
    player _player;

    private void Start()
    {
        _player = GameObject.Find("Level_Player").GetComponent<player>();

        Destroy(gameObject, destroyTime);

        transform.localPosition += offset;
    }

    private void Update()
    {
        //if game is over or win then destroy
        if(_player.score >= _player.maxScore || _player.isGameOver)
        {
            Destroy(gameObject);
        }
    }
}
