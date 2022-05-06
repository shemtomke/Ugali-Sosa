 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour
{
    //array of foods
    public GameObject[] foods;
    public float minValueX, maxValueX, yBound, timeForTutorial;


    //distance between 

    //speed at which the foods should fall //this should increase overtime and become harder
    public float speedToFall = 3f;
    public float maxSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FallingFood());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move()
    {
        //increase speed of the items

    }

    public IEnumerator FallingFood()
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));

        var wanted = Random.Range(minValueX, maxValueX);
        var position = new Vector2(wanted, yBound);
        int randomFood = Random.Range(0, foods.Length);

        GameObject gameObject = Instantiate(foods[randomFood], position, Quaternion.identity);

        //speed of the things falling
        /*if (speedToFall< maxSpeed) 
        {
            speedToFall += 0.1f * Time.deltaTime;

        }*/
        
        StartCoroutine(FallingFood());
    }
}
