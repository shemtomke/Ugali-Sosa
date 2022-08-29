 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour
{
    //array of foods
    public GameObject[] foods;
    public float minValueX, maxValueX, yBound, timeForTutorial;
    private float startDelay = 0.2f;
    private float spawnInterval = 0.8f;

    private void OnEnable()
    {
        //StartCoroutine(FallingFood());
        InvokeRepeating("SpawnRandomFood", startDelay, spawnInterval);
    }

    /*public IEnumerator FallingFood()
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));

        var wanted = Random.Range(minValueX, maxValueX);
        var position = new Vector2(wanted, yBound);
        int randomFood = Random.Range(0, foods.Length);

        GameObject gameObject = Instantiate(foods[randomFood], position, Quaternion.identity);
        
        StartCoroutine(FallingFood());
    }*/

    void SpawnRandomFood()
    {
        Vector2 spawnPos = new Vector2(Random.Range(minValueX, maxValueX), yBound);
        int foodIndex = Random.Range(0, foods.Length);
        Instantiate(foods[foodIndex], spawnPos, foods[foodIndex].transform.rotation);
    }
}
