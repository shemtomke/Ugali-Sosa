using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchUp : MonoBehaviour
{
    public Slider Gain_Slider;
    [SerializeField] player player;
    public float amountToDeplete;

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

        switch (Gain_Slider.maxValue)
        {
            case 1:
                amountToDeplete = 0.1f;
                break;
            case 2:
                amountToDeplete = 0.2f;
                break;
            case 3:
                amountToDeplete = 0.3f;
                break;
            case 4:
                amountToDeplete = 0.4f;
                break;
            case 5:
                amountToDeplete = 0.5f;
                break;
            case 6:
                amountToDeplete = 0.6f;
                break;
            case 7:
                amountToDeplete = 0.7f;
                break;
            case 8:
                amountToDeplete = 0.8f;
                break;
            case 9:
                amountToDeplete = 0.9f;
                break;
            case 10:
                amountToDeplete = 1.0f;
                break;
            case 11:
                amountToDeplete = 1.1f;
                break;
            case 12:
                amountToDeplete = 1.2f;
                break;
            case 13:
                amountToDeplete = 1.3f;
                break;
            case 14:
                amountToDeplete = 1.4f;
                break;
            case 15:
                amountToDeplete = 1.5f;
                break;
            case 16:
                amountToDeplete = 1.6f;
                break;
            case 17:
                amountToDeplete = 1.7f;
                break;
            case 18:
                amountToDeplete = 1.8f;
                break;
            case 19:
                amountToDeplete = 1.9f;
                break;
            case 20:
                amountToDeplete = 2.0f;
                break;
        }
    }
}
