using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoveToNextScene : MonoBehaviour
{
    public int level;
    public int nextSceneLoad;

    public Text levelTxt;

    public player player;
    public Button NextButton;

    // Start is called before the first frame update
    void Start()
    {
        //on start levl 1
        level = PlayerPrefs.GetInt("level", level);

        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1;
    }

    // Update is called once per frame
    void Update()
    {
        levelTxt.text = "" + level;

        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            //disable the next level button
            NextButton.gameObject.SetActive(false);
        }
    }

    //use it on the Next level Button
    public void moveToNxtScn()
    {
        level++;
        PlayerPrefs.SetInt("LVL", level);

        if (player.score >= player.maxScore)
        {
            SceneManager.LoadScene(nextSceneLoad);

            if(nextSceneLoad > PlayerPrefs.GetInt("LVL"))
            {
                PlayerPrefs.SetInt("LVL", nextSceneLoad);
            }
        }
    }
}
