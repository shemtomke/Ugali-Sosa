using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchUp : MonoBehaviour
{
    public Slider Gain_Slider;

    [SerializeField] player player;

    public float amountToDeplete;
    public float depleteValue;

    public TutorialPoint tutorialPoint;

    private void Start()
    {
        Gain_Slider.value = 1;
        Gain_Slider.maxValue = 1;
    }

    private void Update()
    {
        SliderDeplete();
        SliderRestrict();

        if(tutorialPoint.barStart)
        {
            gameObject.GetComponent<CatchUp>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<CatchUp>().enabled = false;
        }
    }
    public void SliderDeplete()
    {
        //the slider depletes 
        Gain_Slider.value -= amountToDeplete * Time.deltaTime; 
    }

    public void SliderRestrict()
    {
        if(Gain_Slider.value <= 0)
        {
            //death
            player.isGameOver = true;
            //remain at 0
            Gain_Slider.value = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //ugali 1 point
        if (collision.CompareTag("Ugali"))
        {
            Gain_Slider.maxValue += 1;
            Gain_Slider.value += 1;
            amountToDeplete += depleteValue;
        }
        //nyama +5
        else if (collision.CompareTag("Nyama"))
        {
            Gain_Slider.maxValue += 5;
            Gain_Slider.value += 5;
            amountToDeplete += depleteValue;
        }
        //chillie -1
        else if (collision.CompareTag("Chillie"))
        {
            Gain_Slider.maxValue -= 1;
            Gain_Slider.value -= 1;
        }
        //mkorogo - instant kill - game over
        else if (collision.CompareTag("Mkorogo"))
        {
            Gain_Slider.maxValue -= 5;
            Gain_Slider.value -= 5;
        }
    }
}
