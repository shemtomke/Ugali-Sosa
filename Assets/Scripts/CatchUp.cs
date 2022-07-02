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

    private void Start()
    {
        Gain_Slider.value = 1;
        Gain_Slider.maxValue = 1;
    }
    private void Update()
    {
        SliderDeplete();
        SliderRestrict();
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

        /*for (int i = Gain_Slider.maxValue; i <= Gain_Slider.maxValue; i++)
        {
            amountToDeplete = amountToDeplete + 0.01f;
        }*/
    }
}
