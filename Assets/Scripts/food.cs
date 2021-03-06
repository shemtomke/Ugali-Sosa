 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour
{
    //array of foods
    public GameObject[] foods;
    public float minValueX, maxValueX, yBound, timeForTutorial;

    private void OnEnable()
    {
        StartCoroutine(FallingFood());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator FallingFood()
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));

        var wanted = Random.Range(minValueX, maxValueX);
        var position = new Vector2(wanted, yBound);
        int randomFood = Random.Range(0, foods.Length);

        GameObject gameObject = Instantiate(foods[randomFood], position, Quaternion.identity);
        
        StartCoroutine(FallingFood());
    }
}
