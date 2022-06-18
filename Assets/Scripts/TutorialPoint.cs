using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialPoint : MonoBehaviour
{
    public GameObject MovePlayer, How_To_Win, FoodManager;

    private void Start()
    {
        if (PlayerPrefs.GetInt("LVL") >= 2 || PlayerPrefs.GetInt("level") >= 2)
        {
            StopCoroutine(startTutorial());
            FoodManager.SetActive(true);
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
    }

}
