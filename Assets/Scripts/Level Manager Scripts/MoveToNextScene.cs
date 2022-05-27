using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoveToNextScene : MonoBehaviour
{
    public int nextSceneLoad;
    public player player;
    public Button NextButton;

    // Start is called before the first frame update
    void Start()
    {
        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            //disable the next level button
            NextButton.gameObject.SetActive(false);
        }
    }

    //use it on the Next level Button
    public void moveToNxtScn()
    {
        if (player.score >= player.maxScore)
        {
            SceneManager.LoadScene(nextSceneLoad);

            if(nextSceneLoad > PlayerPrefs.GetInt("levels"))
            {
                PlayerPrefs.SetInt("levels", nextSceneLoad);
            }
        }
    }
}
