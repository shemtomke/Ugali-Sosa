using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableCatchUp : MonoBehaviour
{
    public CatchUp catchUp;
    public TutorialPoint tutorialPoint;

    // Update is called once per frame
    void Update()
    {
        if (tutorialPoint.barStart)
        {
            catchUp.enabled = true;
        }
        else
        {
            catchUp.enabled = false;
        }
    }
}
