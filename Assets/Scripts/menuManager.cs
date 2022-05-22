using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menuManager : MonoBehaviour
{
    public Image[] gameLevels;
    public int index;
    public Button previousButton;
    public Button nextButton;
    public Text levelName;

    // Start is called before the first frame update
    void Start()
    {
        //automatically we can't previous to level 0
        previousButton.gameObject.SetActive(false);
        index = 0;
        levelName.text = "Level" + " " + 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (index >= 4)
        {
            index = 4;
            nextButton.gameObject.SetActive(false);
        }

        if(index<4)
        {
            nextButton.gameObject.SetActive(true);
        }

        if(index > 0)
        {
            previousButton.gameObject.SetActive(true);
        }

        if(index<0)
        {
            index = 0;
        }

        if(index == 0)
        {
            gameLevels[0].gameObject.SetActive(true);
            previousButton.gameObject.SetActive(false);
        }
    }

    public void StartLevel()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    //go next and previous and choose levels
    public void onClickNext()
    {
        index += 1;
        for (int i = 0; i < gameLevels.Length; i++)
        {
            gameLevels[i].gameObject.SetActive(false);
            gameLevels[index].gameObject.SetActive(true);
            levelName.text = "Level" + " " + i;
        }

        //level name increase
        levelName.text = "Level" + " " + (index + 1);
    }

    public void onClickPrevious()
    {
        index -= 1;
        for (int i = 0; i < gameLevels.Length; i++)
        {
            gameLevels[i].gameObject.SetActive(false);
            gameLevels[index].gameObject.SetActive(true);
            levelName.text = "Level" + " " + i;
        }

        levelName.text = "Level" + " " + ((index + 2) - 1);
    }

    //tracking the levels- i.e level 1 is automatically open
    //level 2 requires 40 points to unlock 

}
