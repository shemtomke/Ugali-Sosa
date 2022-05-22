using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public player player;
    public food food;

    public bool gameIsPaused;
    public GameObject GameOverScreen;
    public GameObject WinScreen;
    public GameObject pauseGameObject;

    public Text scoretxt;
    //reach this score to proceed to next level
    public Text TargetScoreTxt;

    public int TargetScore;
    public int score = 1;

    public Slider slider;

    private void Awake()
    {
        score = PlayerPrefs.GetInt("score", 20);

        slider.maxValue = score;
        scoretxt.text = "" + score;
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

        slider.value = score;

        //target to unlock new level
        scoretxt.text = "SCORE" + " " + score;
    }

    public void GameOver()
    {
        if (player.isGameOver)
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
        if (player.score >= player.maxScore)
        {
            pauseGameObject.SetActive(false);
            //win game 
            WinScreen.SetActive(true);
            //stop food from falling


            player.isMove = false;
            Time.timeScale = 0;
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    //unlock next level
    public void NextLevel()
    {
        //which current level is the player in?

        if(score == PlayerPrefs.GetInt("score"))
        {

        }
        //ASSIGNED level
        SceneManager.LoadScene(3);
        score = score + 15;
        PlayerPrefs.SetInt("score", score);
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
