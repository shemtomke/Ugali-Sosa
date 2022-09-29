using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public player player;
    public food food;

    CatchUp catchUp;

    public bool gameIsPaused, isLose = false;

    public GameObject GameOverScreen;
    public GameObject WinScreen;
    public GameObject pauseGameObject;

    GameObject[] highlight;

    public Text leveltxt;
    public int Level = 1;

    private void Awake()
    {
        Level = PlayerPrefs.GetInt("level", Level);
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        highlight = GameObject.FindGameObjectsWithTag("FloatingText");
        catchUp = GameObject.Find("Level_Player").GetComponent<CatchUp>();
    }

    // Update is called once per frame
    void Update()
    {
        GameOver();
        WinGame();

        leveltxt.text = "" + Level;
    }

    public void GameOver()
    {
        if(player.isGameOver)
        {
            pauseGameObject.SetActive(false);
            //show gameover screen
            GameOverScreen.SetActive(true);
            //stop food from falling

            player.playerSprite.sprite = player.playerLose;

            player.isMove = false;
            Time.timeScale = 0;

            for (int i = 0; i < highlight.Length; i++)
            {
                highlight[i].SetActive(false);
            }
        }
        else
        {
            GameOverScreen.SetActive(false);
        }
    }
    public void WinGame()
    {
        if(player.score >= player.maxScore)
        {
            pauseGameObject.SetActive(false);

            //win game 
            WinScreen.SetActive(true);

            player.playerSprite.sprite = player.playerWin;

            player.isMove = false;
            Time.timeScale = 0;

            for (int i = 0; i < highlight.Length; i++)
            {
                highlight[i].SetActive(false);
            }
        }
    }

    public void ResumeOnRewardAds()
    {
        player.isGameOver = false;
        player.isMove = true;
        Time.timeScale = 1;

        //add some points to the endless scene
        catchUp.Gain_Slider.maxValue += 5;
        catchUp.Gain_Slider.value += 5;
        catchUp.amountToDeplete += catchUp.depleteValue;

        //isLose = true;
        //StartCoroutine(countdownAfterAds());
    }

    public IEnumerator countdownAfterAds()
    {
        while(isLose)
        {
            yield return new WaitForSeconds(0.1f);
            //countdown 3
            //is game over to false
            Debug.Log("3");
            player.isGameOver = false;

            yield return new WaitForSeconds(1);
            //countdown 2
            Debug.Log("2");

            yield return new WaitForSeconds(1);
            //countdown 1
            Debug.Log("1");

            yield return new WaitForSeconds(0.1f);
            //resume
            Debug.Log("Resume");
            //move player
            //time scale to 1
            player.isMove = true;
            Time.timeScale = 1;
            isLose = false;
        }
        

    }
    public void nextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Level++;
        PlayerPrefs.SetInt("level", Level);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
