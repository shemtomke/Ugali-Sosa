using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    public player player;
    public food food;

    public bool gameIsPaused;

    public GameObject GameOverScreen;
    public GameObject WinScreen;
    public GameObject pauseGameObject;

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
            //stop food from falling

            //things to continue falling

            //

            player.playerSprite.sprite = player.playerWin;

            player.isMove = false;
            Time.timeScale = 0;
        }
    }

    public void ResumeOnRewardAds()
    {
        player.isGameOver = false;
        player.isMove = true;
        Time.timeScale = 1;
        //StartCoroutine(countdownAfterAds());
    }

    public IEnumerator countdownAfterAds()
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
