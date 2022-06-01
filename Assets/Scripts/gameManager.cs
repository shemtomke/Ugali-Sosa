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

        leveltxt.text = "LEVEL :" + " " + Level;
    }

    public void GameOver()
    {
        if(player.isGameOver)
        {
            pauseGameObject.SetActive(false);
            //show gameover screen
            GameOverScreen.SetActive(true);
            //stop food from falling
            

            player.isMove = false;
            Time.timeScale = 0;
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

            player.isMove = false;
            Time.timeScale = 0;
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
