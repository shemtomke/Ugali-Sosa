using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatchUp : MonoBehaviour
{
    public Slider Gain_Slider;
    [SerializeField] player player;

    //the slider depletes 

    //how many values should the slider hold (maximum) //or the more the player collects more foods the more the slider increases

    //so on start the slider will have 5/random number from 5 to 10, which then depletes downwards

    //if the player collects a nyama then the slider adds the points to that
}
