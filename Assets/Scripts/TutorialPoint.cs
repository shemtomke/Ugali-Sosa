using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TutorialPoint : MonoBehaviour
{
    public GameObject MovePlayer, How_To_Win, FoodManager, HealthDeplete;
    public string endlessScene;

    Scene scene;
    private void Start()
    {
        scene = SceneManager.GetActiveScene();

        
        if (PlayerPrefs.GetInt("LVL") >= 2 || PlayerPrefs.GetInt("level") >= 2)
        {
            StopCoroutine(startTutorial());
            FoodManager.SetActive(true);

            if (scene.name == endlessScene)
            {
                HealthDeplete.SetActive(true);//only in endless
            }
        }
        else
        {
            StartCoroutine(startTutorial());
        }
        
    }

    IEnumerator startTutorial()
    {
        yield return new WaitForSeconds(0.1f);

        MovePlayer.SetActive(true);
        How_To_Win.SetActive(false);

        yield return new WaitForSeconds(3);

        MovePlayer.SetActive(false);
        How_To_Win.SetActive(true);

        yield return new WaitForSeconds(3);

        MovePlayer.SetActive(false);
        How_To_Win.SetActive(false);
        FoodManager.SetActive(true);

        if (scene.name == endlessScene)
        {
            HealthDeplete.SetActive(true);//only in endless
        }
    }

}
