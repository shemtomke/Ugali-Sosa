using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Button[] levelButtons;

    private void Start()
    {
        int levelAt = PlayerPrefs.GetInt("levels", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if(i + 2 > levelAt)
            {
                levelButtons[i].interactable = false;
            }
        }
    }
    //load levels
    public void level1()
    {
        SceneManager.LoadScene(1);
    }
    public void level2()
    {
        SceneManager.LoadScene(2);
    }
    public void level3()
    {
        SceneManager.LoadScene(3);
    }
    public void level4()
    {
        SceneManager.LoadScene(4);
    }
    public void level5()
    {
        SceneManager.LoadScene(5);
    }

    //rest levels to 1
    public void resetLvls()
    {
        PlayerPrefs.DeleteAll();
    }
}
