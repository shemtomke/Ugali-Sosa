using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public Button l2_closed, l2_open, l3_closed, l3_open, l4_closed, l4_open, l5_closed, l5_open, nextButton, previousButton;
    public int level = 1;
    private int currentLevel;

    private void Awake()
    {
        SelectLevel(0);
    }
    private void Start()
    {
        level = PlayerPrefs.GetInt("LVL", 1);

        switch (level)
        {
            case 1:
                break;
            case 2:
                l2_closed.gameObject.SetActive(false);
                l2_open.gameObject.SetActive(true);
                break;
            case 3:
                l3_closed.gameObject.SetActive(false);
                l3_open.gameObject.SetActive(true);
                break;
            case 4:
                l4_closed.gameObject.SetActive(false);
                l4_open.gameObject.SetActive(true);
                break;
            case 5:
                l5_closed.gameObject.SetActive(false);
                l5_open.gameObject.SetActive(true);
                break;
        }
    }

    private void SelectLevel(int _index)
    {
        previousButton.interactable = (_index != 0);
        nextButton.interactable = (_index != transform.childCount - 1);

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _index);
        }
    }

    public void ChangeLevel(int _change)
    {
        currentLevel += _change;
        SelectLevel(currentLevel);
    }

    //load levels
    public void level1()
    {
        SceneManager.LoadScene(1);
    }
    public void level2()
    {
        //if level prefs reach 2 then set to true in the play button
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
        PlayerPrefs.DeleteKey("LVL");
    }
}
