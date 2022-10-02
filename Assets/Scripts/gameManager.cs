using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{
    player _player;
    public food food;

    CatchUp catchUp;

    public bool gameIsPaused, isLose = false;

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
        _player = GameObject.Find("Level_Player").GetComponent<player>();
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
        if(_player.isGameOver)
        {
            pauseGameObject.SetActive(false);
            //show gameover screen
            GameOverScreen.SetActive(true);
            //stop food from falling

            _player.playerSprite.sprite = _player.playerLose;

            _player.isMove = false;
            Time.timeScale = 0;
        }
        else
        {
            GameOverScreen.SetActive(false);
        }
    }
    public void WinGame()
    {
        if(_player.score >= _player.maxScore)
        {
            _player.BgMusic.Stop();

            pauseGameObject.SetActive(false);

            //win game 
            WinScreen.SetActive(true);

            _player.playerSprite.sprite = _player.playerWin;

            _player.isMove = false;
            Time.timeScale = 0;
        }
    }

    public void ResumeOnRewardAds() //level 1 to 5
    {
        _player.isGameOver = false;
        _player.isMove = true;
        _player.BgMusic.Play();
        Time.timeScale = 1;
    }

    public void ResumeInEndless() //endless
    {
        _player.isGameOver = false;
        _player.isMove = true;
        _player.BgMusic.Play();
        Time.timeScale = 1;

        //add some points to the endless scene
        catchUp.Gain_Slider.maxValue += 5;
        catchUp.Gain_Slider.value += 5;
        catchUp.amountToDeplete += catchUp.depleteValue;
    }

    public IEnumerator countdownAfterAds()
    {
        while(isLose)
        {
            yield return new WaitForSeconds(0.1f);
            //countdown 3
            //is game over to false
            Debug.Log("3");
            _player.isGameOver = false;

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
            _player.isMove = true;
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
